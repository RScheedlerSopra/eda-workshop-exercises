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