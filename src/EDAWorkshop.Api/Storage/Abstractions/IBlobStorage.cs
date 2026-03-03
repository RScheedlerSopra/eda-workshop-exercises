public interface IBlobStorage
{
    Task<string> SaveAsync(byte[] data, string fileName);
    Task<byte[]> GetAsync(string blobKey);
}