namespace FinBooKeAPI.Logic.FileSystem;

public class FileSystem : IFileSystem
{
    public string CombinePath(string path, string filename)
    {
        return Path.Combine(path, filename);
    }

    public bool FileExists(string filename)
    {
        return Path.Exists(filename);
    }

    public string GetFileExtension(string filename)
    {
        return Path.GetExtension(filename);
    }

    public string ReadAllText(string file)
    {
        return File.ReadAllText(file);
    }
}
