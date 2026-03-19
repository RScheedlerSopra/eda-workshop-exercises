# Exercise: Giants
In this exercise we will learn a robust solution for dealing with large messages flowing through your event-driven architecture. 

The goal of this exercise is to build an endpoint that processes large documents in an event-driven fashion.

## Context
This application is simple API that is prepared for adding event-driven features. It uses NServiceBus for this: a framework for building distributed systems using messaging. It gives us a messaging framework out of the box and and accompanying transport mechanism (the Learning Transport) that we can use for local development. 

Messages can be send using the `IMessageSession` interface. This interface has a `Send(Local)` method that we can use to send messages to (other) endpoints. The message is then processed by a handler in another endpoint.

## Part 1: The naive approach

### 0) Open the Swagger page
Open a terminal and cd to `src/EDAWorkshop.Api` and run `dotnet run`. Then open a browser and navigate to `http://localhost:5027/swagger/index.html`. You should see the Swagger page for the API.

### 1) Defining a command
First, we define a command for uploading a document. The command is a way for (other) systems to request an action in an asynchronous way. 

Create a new file `src/EDAWorkshop.Api/Commands/ProcessDocument.cs` with the following content:

```csharp
public class ProcessDocument : ICommand
{
    public Guid DocumentId { get; set; }
    public string FileName { get; set; }
    public byte[] Content { get; set; }   
}
```

### 2) Creating an API endpoint
Create a new file `src/EDAWorkshop.Api/Controllers/DocumentsController.cs` with the following content:
```csharp
[ApiController]
[Route("documents")]
public class DocumentsController : ControllerBase
{
    private readonly IMessageSession messageSession;

    public DocumentsController(IMessageSession messageSession)
    {
        this.messageSession = messageSession;
    }

    [HttpPost]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        using var ms = new MemoryStream();
        await file.CopyToAsync(ms);

        var command = new ProcessDocument
        {
            DocumentId = Guid.NewGuid(),
            FileName = file.FileName,
            Content = ms.ToArray()
        };

        await messageSession.SendLocal(command);

        return Accepted();
    }
}
```

### 3) Creating a handler
Create a new file `src/EDAWorkshop.Api/Handlers/ProcessDocumentHandler.cs` with the following content:

```csharp
public class ProcessDocumentHandler : IHandleMessages<ProcessDocument>
{
    public async Task Handle(ProcessDocument message, IMessageHandlerContext context)
    {
        Console.WriteLine(
            $"Processing {message.FileName}, size: {message.Content.Length}"
        );

        await Task.Delay(500, context.CancellationToken);
    }
}
```

### 4) Test the endpoint
Open the Swagger page and upload a file using the `POST /documents` endpoint. You should see a log message printed in the terminal after a short delay.

Notice that the file content is embedded directly in the command. For small files this is fine, but what happens when a file is large? The entire file content is serialized into the message payload, which puts pressure on your transport and makes messages slow and expensive to route.

## Part 2: The Claim Check approach

In the previous part we embedded the file content directly inside the command. While this works for small payloads, it quickly becomes a problem in practice:

- Most message transports impose a **size limit** on messages (often a few hundred kilobytes). Embedding raw binary content blows past this limit for any real-world document.
- Large messages consume more memory, increase serialization time, and slow down your entire messaging pipeline.

The **Claim Check pattern** solves this by keeping large payloads out of the message entirely. Instead of carrying the content, the message carries only a *reference* — a key — that the handler can use to fetch the content from a dedicated blob store. The message stays small; the data lives where it belongs.

### 1) Define the blob storage abstraction
Create a new file `src/EDAWorkshop.Api/Storage/Abstractions/IBlobStorage.cs`:

```csharp
public interface IBlobStorage
{
    Task<string> SaveAsync(byte[] data, string fileName);
    Task<byte[]> GetAsync(string blobKey);
}
```

`SaveAsync` stores the raw bytes and returns an opaque key. `GetAsync` retrieves the bytes given that key. Hiding the storage mechanism behind an interface keeps the rest of the code decoupled from the concrete storage technology.

### 2) Implement the blob storage
Create a new file `src/EDAWorkshop.Api/Storage/FileSystemBlobStorage.cs`:

```csharp
public class FileSystemBlobStorage : IBlobStorage
{
    private readonly string basePath = "blob-storage";

    public FileSystemBlobStorage()
    {
        Directory.CreateDirectory(basePath);
    }

    public async Task<string> SaveAsync(byte[] data, string fileName)
    {
        var key = $"{Guid.NewGuid()}_{fileName}";
        var fullPath = Path.Combine(basePath, key);

        await File.WriteAllBytesAsync(fullPath, data);

        return key;
    }

    public async Task<byte[]> GetAsync(string blobKey)
    {
        var fullPath = Path.Combine(basePath, blobKey);
        return await File.ReadAllBytesAsync(fullPath);
    }
}
```

This implementation writes files to a local `blob-storage/` directory. The key is a GUID prefix combined with the original filename, which makes it unique while still human-readable.

### 3) Register the blob storage
Open `src/EDAWorkshop.Api/Program.cs` and add the following line after `builder.Services.AddControllers()`:

```csharp
builder.Services.AddSingleton<IBlobStorage, FileSystemBlobStorage>();
```

### 4) Update the command
The command no longer needs to carry the file content — only the key that points to it. Update `src/EDAWorkshop.Api/Commands/ProcessDocument.cs`:

```csharp
public class ProcessDocument : ICommand
{
    public Guid DocumentId { get; set; }
    public string FileName { get; set; } = null!;
    public string BlobKey { get; set; } = null!;
}
```

`Content` is gone. `BlobKey` takes its place.

### 5) Update the controller
The controller is now responsible for saving the file to blob storage *before* sending the command. Update `src/EDAWorkshop.Api/Controllers/DocumentsController.cs`:

```csharp
[ApiController]
[Route("documents")]
public class DocumentsController : ControllerBase
{
    private readonly IMessageSession messageSession;

    public DocumentsController(IMessageSession messageSession)
    {
        this.messageSession = messageSession;
    }

    [HttpPost]
    public async Task<IActionResult> Upload(
        IFormFile file,
        [FromServices] IBlobStorage blobStorage)
    {
        using var ms = new MemoryStream();
        await file.CopyToAsync(ms);

        var blobKey = await blobStorage.SaveAsync(
            ms.ToArray(),
            file.FileName);

        var command = new ProcessDocument
        {
            DocumentId = Guid.NewGuid(),
            FileName = file.FileName,
            BlobKey = blobKey
        };

        await messageSession.SendLocal(command);

        return Accepted();
    }
}
```

The `IBlobStorage` dependency is injected via `[FromServices]` directly on the action method, keeping the constructor focused on the messaging dependency. The file content is stored, and only the key travels with the message.

### 6) Update the handler
The handler now fetches the document content from blob storage using the key from the message. Update `src/EDAWorkshop.Api/Handlers/ProcessDocumentHandler.cs`:

```csharp
public class ProcessDocumentHandler(IBlobStorage blobStorage)
    : IHandleMessages<ProcessDocument>
{
    private readonly IBlobStorage blobStorage = blobStorage;

    public async Task Handle(ProcessDocument message,
                             IMessageHandlerContext context)
    {
        var content = await blobStorage.GetAsync(message.BlobKey);

        Console.WriteLine(
            $"Processing {message.FileName}, size: {content.Length}"
        );

        await Task.Delay(500, context.CancellationToken);
    }
}
```

The handler receives `IBlobStorage` through primary constructor injection. It uses `message.BlobKey` to retrieve the actual file bytes and then processes them.

### 7) Test the endpoint
Run the application and upload a file through Swagger. You should see the same log message as before, confirming nothing changed from the caller's perspective. Now inspect the `blob-storage/` directory inside `src/EDAWorkshop.Api` — the file is stored there, and the message that traveled through NServiceBus contained only its key.