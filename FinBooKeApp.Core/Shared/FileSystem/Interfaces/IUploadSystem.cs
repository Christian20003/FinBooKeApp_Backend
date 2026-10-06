namespace FinBooKeApp.Core.Shared.FileSystem.Interfaces;

public interface IUploadSystem
{
    public string UploadImage(Guid userId, IFile image);

    public void DeleteImage(string fileName);

    public byte[] GetImageContent(Guid userId, string fileName);
}
