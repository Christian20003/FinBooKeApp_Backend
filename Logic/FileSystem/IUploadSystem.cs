namespace FinBooKeAPI.Logic.FileSystem;

public interface IUploadSystem
{
    public string UploadImage(Guid userId, IFormFile image);

    public void DeleteImage(string fileName);

    public byte[] GetImageContent(Guid userId, string fileName);
}
