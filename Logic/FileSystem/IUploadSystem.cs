namespace FinBooKeAPI.Logic.FileSystem;

public interface IUploadSystem
{
    public string UploadImage(Guid userId, IFormFile image);
}
