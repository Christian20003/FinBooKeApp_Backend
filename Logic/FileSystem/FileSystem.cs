namespace FinBooKeAPI.Logic.FileSystem;

public class FileSystem : IFileSystem
{
    public string CombinePath(string path, string filename)
    {
        return Path.Combine(path, filename);
    }

    public void CreateDirectory(string path)
    {
        if (!Path.Exists(path))
            Directory.CreateDirectory(path);
    }

    public bool FileExists(string filename)
    {
        return Path.Exists(filename);
    }

    public string GetFileExtension(string filename)
    {
        return Path.GetExtension(filename);
    }

    public string GetFileName(string filename)
    {
        return Path.GetFileNameWithoutExtension(filename);
    }

    public string ReadAllText(string file)
    {
        return File.ReadAllText(file);
    }

    public void WriteAllBytes(IFormFile file, string path)
    {
        using var stream = new FileStream(path, FileMode.OpenOrCreate);
        file.CopyTo(stream);
    }
}
