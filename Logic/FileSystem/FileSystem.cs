using System.Runtime.InteropServices;
using System.Security.AccessControl;

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

    public void DeleteFile(string filename)
    {
        File.Delete(filename);
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

    public byte[] ReadAllBytes(string file)
    {
        return File.ReadAllBytes(file);
    }

    public void SetFilePermission(string path, IEnumerable<FilePermission> permissions)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            SetFilePermissionWindows(path, permissions);
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            SetFilePermissionLinux(path, permissions);
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            SetFilePermissionLinux(path, permissions);
        else
            throw new NotImplementedException(
                "Your operating system is not supported to modify file permissions"
            );
    }

    private static void SetFilePermissionLinux(string path, IEnumerable<FilePermission> permissions)
    {
        if (
            !RuntimeInformation.IsOSPlatform(OSPlatform.Linux)
            || !RuntimeInformation.IsOSPlatform(OSPlatform.OSX)
        )
            return;
        UnixFileMode mode = UnixFileMode.None;
        foreach (var permission in permissions)
        {
            switch (permission)
            {
                case FilePermission.READ:
                    mode |= UnixFileMode.UserRead;
                    break;
                case FilePermission.WRITE:
                    mode |= UnixFileMode.UserWrite;
                    break;
                case FilePermission.EXECUTE:
                    mode |= UnixFileMode.UserExecute;
                    break;
            }
        }
        File.SetUnixFileMode(path, mode);
    }

    private static void SetFilePermissionWindows(
        string path,
        IEnumerable<FilePermission> permissions
    )
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return;
        var fileInfo = new FileInfo(path);
        var filePermissions = fileInfo.GetAccessControl();

        var readControlType = AccessControlType.Deny;
        var writeControlType = AccessControlType.Deny;
        var executeControlType = AccessControlType.Deny;
        if (permissions.Contains(FilePermission.READ))
            readControlType = AccessControlType.Allow;
        if (permissions.Contains(FilePermission.WRITE))
            writeControlType = AccessControlType.Allow;
        if (permissions.Contains(FilePermission.EXECUTE))
            executeControlType = AccessControlType.Allow;

        filePermissions.AddAccessRule(
            new FileSystemAccessRule("Everyone", FileSystemRights.Read, readControlType)
        );
        filePermissions.AddAccessRule(
            new FileSystemAccessRule("Everyone", FileSystemRights.Write, writeControlType)
        );
        filePermissions.AddAccessRule(
            new FileSystemAccessRule("Everyone", FileSystemRights.ExecuteFile, executeControlType)
        );
        fileInfo.SetAccessControl(filePermissions);
    }
}
