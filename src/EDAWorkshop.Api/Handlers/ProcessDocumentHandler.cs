public class ProcessDocumentHandler 
    : IHandleMessages<ProcessDocument>
{
    public async Task Handle(ProcessDocument message, 
                             IMessageHandlerContext context)
    {
        Console.WriteLine(
            $"Processing {message.FileName}, size: {message.Content.Length}"
        );

        await Task.Delay(500, context.CancellationToken);
    }
}