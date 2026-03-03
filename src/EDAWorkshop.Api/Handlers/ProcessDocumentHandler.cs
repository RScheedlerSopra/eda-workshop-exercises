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