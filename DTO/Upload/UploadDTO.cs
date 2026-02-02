using System.ComponentModel.DataAnnotations;
using FinBookeAPI.Models.Upload;

namespace FinBookeAPI.DTO.Upload;

/// <summary>
/// This record represents a data transfer object for uploading
/// files to this application.
/// </summary>
public record UploadDTO
{
    [Required(ErrorMessage = "Uploaded file is missing")]
    public required IFormFile File { get; set; }

    [Required(ErrorMessage = "Created at date is missing")]
    public DateTime CreateAt { get; set; }

    public FileUpload GetFileUpload()
    {
        return new FileUpload { File = File, CreatedAt = CreateAt };
    }
}
