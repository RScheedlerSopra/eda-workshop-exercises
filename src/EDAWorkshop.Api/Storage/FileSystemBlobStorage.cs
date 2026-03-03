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