namespace FinBooKeAPI.Logic.FileSystem;

public interface IFileSystem
{
    public string ReadAllText(string file);
    public void WriteAllBytes(IFormFile file, string path);
    public string CombinePath(string path, string filename);
    public bool FileExists(string filename);
    public string GetFileExtension(string filename);
    public string GetFileName(string filename);
    public void CreateDirectory(string path);
}
