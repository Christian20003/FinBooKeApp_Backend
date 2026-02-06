using FinBookeAPI.AppConfig.Authentication;
using FinBookeAPI.Attributes;
using FinBookeAPI.Controllers.Helper;
using FinBookeAPI.DTO.Error;
using FinBookeAPI.DTO.Payment;
using FinBookeAPI.Models.Configuration;
using FinBookeAPI.Models.Payment;
using FinBookeAPI.Services.Payment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinBookeAPI.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public partial class PaymentMethodController(
    IPaymentMethodService service,
    ILogger<PaymentMethodController> logger
) : ControllerBase
{
    private readonly IPaymentMethodService _service = service;
    private readonly ILogger<PaymentMethodController> _logger = logger;

    /// <summary>
    /// This method creates the response message for a payment method
    /// request.
    /// </summary>
    /// <param name="method">
    /// The payment method which should be returned.
    /// </param>
    /// <param name="location">
    /// If a URL should be included.
    /// </param>
    /// <returns>
    /// The repsonse message.
    /// </returns>
    private GetPaymentMethodDTO GetDTO(PaymentMethod method, bool location = true)
    {
        var url = location
            ? UrlGenerator.GenerateUrl(
                Request,
                Url,
                nameof(GetPaymentMethod),
                new { id = method.Id }
            )
            : null;
        var instances = method.Instances.Select(instance =>
        {
            var url = location
                ? UrlGenerator.GenerateUrl(
                    Request,
                    Url,
                    nameof(GetPaymentInstance),
                    new { id = instance.Id }
                )
                : null;
            return new GetPaymentInstanceDTO(instance, url);
        });
        return new GetPaymentMethodDTO(method, instances, url);
    }

    /// <summary>
    /// This method creates a new payment method.
    /// </summary>
    /// <param name="method">
    /// The payment method data.
    /// </param>
    /// <response code="201">If the payment method could be added successfully</response>
    /// <response code="400">If the received data does not fulfill the requirements</response>
    /// <response code="500">If a server error occur</response>
    [HttpPost]
    [ProducesResponseType(typeof(GetPaymentMethodDTO), 201)]
    [ProducesResponseType(typeof(BadRequestDTO), 400)]
    [ProducesResponseType(typeof(ErrorDTO), 500)]
    public async Task<ActionResult> CreatePaymentMethod(PostPaymentMethodDTO method)
    {
        LogCreatePaymentMethod(method);
        var userId = HttpContext.User.GetUserId();
        var result = await _service.CreatePaymentMethod(method.GetPaymentMethod(userId));
        return CreatedAtAction(nameof(GetPaymentMethod), new { id = result.Id }, GetDTO(result));
    }

    /// <summary>
    /// This method updates an existing payment method.
    /// </summary>
    /// <param name="id">
    /// The id of the payment method.
    /// </param>
    /// <param name="method">
    /// The updated data of the payment method.
    /// </param>
    /// <response code="200">If the payment method could be updated successfully</response>
    /// <response code="400">If the received data does not fulfill the requirements</response>
    /// <response code="401">If the user does not have access to the specified payment method</response>
    /// <response code="404">If the payment method does not exist</response>
    /// <response code="500">If a server error occur</response>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(GetPaymentMethodDTO), 200)]
    [ProducesResponseType(typeof(BadRequestDTO), 400)]
    [ProducesResponseType(typeof(ErrorDTO), 401)]
    [ProducesResponseType(typeof(ErrorDTO), 404)]
    [ProducesResponseType(typeof(ErrorDTO), 500)]
    public async Task<ActionResult> UpdatePaymentMethod(
        [Guid(ErrorMessage = "Id is not a valid GUID")] string id,
        PostPaymentMethodDTO method
    )
    {
        LogUpdatePaymentMethod(id, method);
        var methodId = Guid.Parse(id);
        var userId = HttpContext.User.GetUserId();
        var data = method.GetPaymentMethod(userId);
        data.Id = methodId;
        var result = await _service.UpdatePaymentMethod(data);
        return Ok(GetDTO(result));
    }

    /// <summary>
    /// This method deletes an existing payment method.
    /// </summary>
    /// <param name="id">
    /// The id of the payment method which should be deleted.
    /// </param>
    /// <response code="200">If the payment method could be removed successfully</response>
    /// <response code="400">If the received data does not fulfill the requirements</response>
    /// <response code="401">If the user does not have access to the specified payment method</response>
    /// <response code="404">If the payment method does not exist</response>
    /// <response code="500">If a server error occur</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(GetPaymentMethodDTO), 200)]
    [ProducesResponseType(typeof(BadRequestDTO), 400)]
    [ProducesResponseType(typeof(ErrorDTO), 401)]
    [ProducesResponseType(typeof(ErrorDTO), 404)]
    [ProducesResponseType(typeof(ErrorDTO), 500)]
    public async Task<ActionResult> DeletePaymentMethod(
        [Guid(ErrorMessage = "Id is not a valid GUID")] string id
    )
    {
        LogDeletePaymentMethod(id);
        var methodId = Guid.Parse(id);
        var userId = HttpContext.User.GetUserId();
        var result = await _service.RemovePaymentMethod(methodId, userId);
        return Ok(GetDTO(result, false));
    }

    /// <summary>
    /// This method returns a specific payment method.
    /// </summary>
    /// <param name="id">
    /// The id of the payment method.
    /// </param>
    /// <response code="200">If the payment method could be read successfully</response>
    /// <response code="400">If the received data does not fulfill the requirements</response>
    /// <response code="401">If the user does not have access to the specified payment method</response>
    /// <response code="404">If the payment method does not exist</response>
    /// <response code="500">If a server error occur</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(GetPaymentMethodDTO), 200)]
    [ProducesResponseType(typeof(BadRequestDTO), 400)]
    [ProducesResponseType(typeof(ErrorDTO), 401)]
    [ProducesResponseType(typeof(ErrorDTO), 404)]
    [ProducesResponseType(typeof(ErrorDTO), 500)]
    public async Task<ActionResult> GetPaymentMethod(
        [Guid(ErrorMessage = "Id is not a valid GUID")] string id
    )
    {
        LogGetPaymentMethod(id);
        var methodId = Guid.Parse(id);
        var userId = HttpContext.User.GetUserId();
        var result = await _service.GetPaymentMethod(methodId, userId);
        return Ok(GetDTO(result));
    }

    /// <summary>
    /// This method returns the corresponding payment method which
    /// includes the payment instance with the provided id.
    /// </summary>
    /// <param name="id">
    /// The id of the payment instances.
    /// </param>
    /// <response code="200">If the payment method could be read successfully</response>
    /// <response code="400">If the received data does not fulfill the requirements</response>
    /// <response code="401">If the user does not have access to the specified payment method</response>
    /// <response code="404">If the payment instance does not exist</response>
    /// <response code="500">If a server error occur</response>
    [HttpGet("instance/{id}")]
    [ProducesResponseType(typeof(GetPaymentMethodDTO), 200)]
    [ProducesResponseType(typeof(BadRequestDTO), 400)]
    [ProducesResponseType(typeof(ErrorDTO), 401)]
    [ProducesResponseType(typeof(ErrorDTO), 404)]
    [ProducesResponseType(typeof(ErrorDTO), 500)]
    public async Task<ActionResult> GetPaymentInstance(
        [Guid(ErrorMessage = "Id is not a valid GUID")] string id
    )
    {
        LogGetPaymentInstance(id);
        var instanceId = Guid.Parse(id);
        var userId = HttpContext.User.GetUserId();
        var result = await _service.GetPaymentMethodById(instanceId, userId);
        return Ok(GetDTO(result));
    }

    /// <summary>
    /// This method returns all payment method from a user.
    /// </summary>
    /// <response code="200">If the payment methods could be read successfully</response>
    /// <response code="500">If a server error occur</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<GetPaymentMethodDTO>), 200)]
    [ProducesResponseType(typeof(ErrorDTO), 500)]
    public async Task<ActionResult> GetPaymentMethods()
    {
        LogGetPaymentMethods();
        var userId = HttpContext.User.GetUserId();
        var result = await _service.GetPaymentMethods(userId);
        return Ok(result.Select(method => GetDTO(method)));
    }

    [LoggerMessage(
        EventId = LogEvents.PaymentPostRequest,
        Level = LogLevel.Information,
        Message = "Payment: Get post request - Method: {Method}"
    )]
    private partial void LogCreatePaymentMethod(PostPaymentMethodDTO method);

    [LoggerMessage(
        EventId = LogEvents.PaymentPutRequest,
        Level = LogLevel.Information,
        Message = "Payment: Get put request - Id: {Id}, Method: {Method}"
    )]
    private partial void LogUpdatePaymentMethod(string id, PostPaymentMethodDTO method);

    [LoggerMessage(
        EventId = LogEvents.PaymentDeleteRequest,
        Level = LogLevel.Information,
        Message = "Payment: Get delete request - Id: {Id}"
    )]
    private partial void LogDeletePaymentMethod(string id);

    [LoggerMessage(
        EventId = LogEvents.PaymentGetMethodRequest,
        Level = LogLevel.Information,
        Message = "Payment: Get get request - Id: {Id}"
    )]
    private partial void LogGetPaymentMethod(string id);

    [LoggerMessage(
        EventId = LogEvents.PaymentGetInstanceRequest,
        Level = LogLevel.Information,
        Message = "Payment: Get get request - Id: {Id}"
    )]
    private partial void LogGetPaymentInstance(string id);

    [LoggerMessage(
        EventId = LogEvents.PaymentGetAllRequest,
        Level = LogLevel.Information,
        Message = "Payment: Get get request"
    )]
    private partial void LogGetPaymentMethods();
}
