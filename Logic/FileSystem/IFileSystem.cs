namespace FinBooKeAPI.Logic.FileSystem;

public interface IFileSystem
{
    public string ReadAllText(string file);
    public string CombinePath(string path, string filename);
    public bool FileExists(string filename);
    public string GetFileExtension(string filename);
}
