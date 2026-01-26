using FinBookeAPI.AppConfig.Authentication;
using FinBookeAPI.DTO.Error;
using FinBookeAPI.DTO.Upload;
using FinBookeAPI.Models.Configuration;
using FinBookeAPI.Models.Upload;
using FinBookeAPI.Services.Upload;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace FinBookeAPI.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public partial class UploadController(
    IUploadService service,
    IOptions<FileStorage> options,
    ILogger<UploadController> logger
) : ControllerBase
{
    private readonly IUploadService _service = service;
    private readonly IOptions<FileStorage> _options = options;
    private readonly ILogger<UploadController> _logger = logger;

    /// <summary>
    /// This method uploads a file to the server.
    /// </summary>
    /// <param name="upload">
    /// The upload object.
    /// </param>
    /// <param name="type">
    /// The type of upload.
    /// </param>
    /// <returns>
    /// The name of the file on the server.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// <see cref="IUploadService.UploadFile(FileUpload, Guid, UploadType)"/>
    /// </exception>
    /// /// <exception cref="FormatException">
    /// <see cref="IUploadService.UploadFile(FileUpload, Guid, UploadType)"/>
    /// </exception>
    /// /// <exception cref="UnauthorizedAccessException">
    /// <see cref="IUploadService.UploadFile(FileUpload, Guid, UploadType)"/>
    /// </exception>
    private async Task<string> Upload(UploadDTO upload, UploadType type)
    {
        var userId = HttpContext.User.GetUserId();
        var file = upload.GetFileUpload();
        LogPostFile(userId, file.File.FileName, type);
        return await _service.UploadFile(file, userId, type);
    }

    /// <summary>
    /// This method downloads a file.
    /// </summary>
    /// <param name="name">
    /// The file name.
    /// </param>
    /// <param name="type">
    /// The type of upload.
    /// </param>
    /// <returns>
    /// The uploaded file.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// <see cref="IUploadService.UploadFile(FileUpload, Guid, UploadType)"/>
    /// </exception>
    /// <exception cref="FileNotFoundException">
    /// <see cref="IUploadService.UploadFile(FileUpload, Guid, UploadType)"/>
    /// </exception>
    /// <exception cref="FormatException">
    /// If the provided file extension is not supported.
    /// </exception>
    private async Task<FileContentResult> Download(string name, UploadType type)
    {
        var userId = HttpContext.User.GetUserId();
        LogGetFile(userId, name, type);
        var file = await _service.GetFile(userId, UploadType.IMAGE, name);
        var extension = Path.GetExtension(name).ToLowerInvariant();
        try
        {
            var contentType = _options.Value.FileFormats[extension];
            return File(file, contentType);
        }
        catch (KeyNotFoundException exception)
        {
            throw new FormatException($"File format {extension} is not supported", exception);
        }
    }

    /// <summary>
    /// This method allows a client to upload a profile image.
    /// </summary>
    /// <param name="upload">
    /// The data to upload a profile image.
    /// </param>
    /// <response code="201">The name of file on the server.</response>
    /// <response code="400">If the filename or extension is not supported.</response>
    /// <response code="500">If any other kind of server error occur.</response>
    [HttpPost("image")]
    [ProducesResponseType(typeof(string), 201)]
    [ProducesResponseType(typeof(BadRequestDTO), 400)]
    [ProducesResponseType(typeof(ErrorDTO), 500)]
    public async Task<ActionResult> PostImage(UploadDTO upload)
    {
        var name = await Upload(upload, UploadType.IMAGE);

        return CreatedAtAction(nameof(GetImage), new { name }, name);
    }

    /// <summary>
    /// This method allows a client to upload a bank statement file.
    /// </summary>
    /// <param name="upload">
    /// The data to upload a bank statement file.
    /// </param>
    /// <response code="201">The name of the file on the server.</response>
    /// <response code="400">If the filename or extension is not supported.</response>
    /// <response code="500">If any kind of server error occur.</response>
    [HttpPost("bank_statement")]
    [ProducesResponseType(typeof(string), 201)]
    [ProducesResponseType(typeof(BadRequestDTO), 400)]
    [ProducesResponseType(typeof(ErrorDTO), 500)]
    public async Task<ActionResult> PostBankStatement(UploadDTO upload)
    {
        var name = await Upload(upload, UploadType.BANK_STATEMENT);

        return CreatedAtAction(nameof(GetBankStatement), new { name }, name);
    }

    /// <summary>
    /// This method allows a client to upload a receipt file.
    /// </summary>
    /// <param name="upload">
    /// The data to upload a receipt file.
    /// </param>
    /// <response code="201">The name of the file on the server.</response>
    /// <response code="400">If the filename or extension is not supported.</response>
    /// <response code="500">If any kind of server error occur.</response>
    [HttpPost("receipt")]
    [ProducesResponseType(typeof(string), 201)]
    [ProducesResponseType(typeof(BadRequestDTO), 400)]
    [ProducesResponseType(typeof(ErrorDTO), 500)]
    public async Task<ActionResult> PostReceipt(UploadDTO upload)
    {
        var name = await Upload(upload, UploadType.RECEIPT);

        return CreatedAtAction(nameof(GetReceipt), new { name }, name);
    }

    /// <summary>
    /// This method downloads an uploaded profile image.
    /// </summary>
    /// <param name="name">
    /// The name of the profile image.
    /// </param>
    /// <response code="200">The uploaded file.</response>
    /// <response code="400">If filename or extension is not supported.</response>
    /// <response code="404">If requested file does not exist.</response>
    /// <response code="500">If any kind of server error occur.</response>
    [HttpGet("image/{name}")]
    [ProducesResponseType(typeof(FileContentResult), 200)]
    [ProducesResponseType(typeof(BadRequestDTO), 400)]
    [ProducesResponseType(typeof(ErrorDTO), 404)]
    [ProducesResponseType(typeof(ErrorDTO), 500)]
    public async Task<ActionResult> GetImage(string name)
    {
        return await Download(name, UploadType.IMAGE);
    }

    /// <summary>
    /// This method downloads an uploaded bank statement file.
    /// </summary>
    /// <param name="name">
    /// The name of the bank statement file.
    /// </param>
    /// <response code="200">The uploaded file.</response>
    /// <response code="400">If filename or extension is not supported.</response>
    /// <response code="404">If requested file does not exist.</response>
    /// <response code="500">If any kind of server error occur.</response>
    [HttpGet("bank_statement/{name}")]
    [ProducesResponseType(typeof(FileContentResult), 200)]
    [ProducesResponseType(typeof(BadRequestDTO), 400)]
    [ProducesResponseType(typeof(ErrorDTO), 404)]
    [ProducesResponseType(typeof(ErrorDTO), 500)]
    public async Task<ActionResult> GetBankStatement(string name)
    {
        return await Download(name, UploadType.BANK_STATEMENT);
    }

    /// <summary>
    /// This method downloads an uploaded receipt file.
    /// </summary>
    /// <param name="name">
    /// The name of the receipt file.
    /// </param>
    /// <response code="200">The uploaded file.</response>
    /// <response code="400">If filename or extension is not supported.</response>
    /// <response code="404">If requested file does not exist.</response>
    /// <response code="500">If any kind of server error occur.</response>
    [HttpGet("receipt/{name}")]
    [ProducesResponseType(typeof(FileContentResult), 200)]
    [ProducesResponseType(typeof(BadRequestDTO), 400)]
    [ProducesResponseType(typeof(ErrorDTO), 404)]
    [ProducesResponseType(typeof(ErrorDTO), 500)]
    public async Task<ActionResult> GetReceipt(string name)
    {
        return await Download(name, UploadType.RECEIPT);
    }

    [LoggerMessage(
        EventId = LogEvents.UploadPostRequest,
        Level = LogLevel.Information,
        Message = "Received upload - User: {UserId}, Name: {Name}, Type: {Type}"
    )]
    private partial void LogPostFile(Guid userId, string name, UploadType type);

    [LoggerMessage(
        EventId = LogEvents.UploadGetRequest,
        Level = LogLevel.Information,
        Message = "Received download - User: {UserId}, Name: {Name}, Type: {Type}"
    )]
    private partial void LogGetFile(Guid userId, string name, UploadType type);
}
