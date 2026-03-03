public class ProcessDocument : ICommand
{
    public Guid DocumentId { get; set; }
    public string FileName { get; set; } = null!;
    public string BlobKey { get; set; }  = null!;
}