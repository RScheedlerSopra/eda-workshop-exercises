public class ProcessDocument : ICommand
{
    public Guid DocumentId { get; set; }
    public string FileName { get; set; } = null!;
    public byte[] Content { get; set; } = null!;
}