using Microsoft.AspNetCore.Mvc;

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