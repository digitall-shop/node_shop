using Api.Filters;
using Api.Securities;
using Application.Services.Interfaces;
using Domain.Common;
using Domain.DTOs.Transaction;
using Domain.DTOs.Transaction.Plisio;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Tags("Payment Endpoint")]
[Route("api/payment")]
public class PaymentController(IPaymentService service) : ApiBaseController
{
    [HttpPost]
    [EndpointName("payment-request")]
    [EndpointSummary("this for add payment request")]
    [EndpointDescription("user can add new payment request to update balance")]
    [ProducesResponseType(typeof(ApiResult<PaymentResultDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PaymentResultDto>> InitiatePayment([FromBody] PaymentInitiationDto request)
    {
        var result = await service.CreatePaymentAsync(request);
        if (!result.IsSuccess) return BadRequest(result.ErrorMessage);
        return Ok(result);
    }

    [HttpPost("{id:long}/approve")]
    [EndpointName("approve")]
    [EndpointSummary("approve payment request")]
    [EndpointDescription("admin can approve payment request from a user")]
    [SuperAdminAuthorize]
    [ProducesResponseType(typeof(ApiResult<PaymentResultDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Approve([FromRoute]long id)
    {
        var result = await service.ApprovePaymentAsync(id);
        return Ok(result);
    }

    [HttpPost("{id:long}/reject")]
    [EndpointName("reject")]
    [EndpointSummary("reject payment request")]
    [EndpointDescription("admin can reject payment request from a user")]
    [SuperAdminAuthorize]
    [ProducesResponseType(typeof(ApiResult<PaymentResultDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Reject([FromRoute]long id,[FromBody] PaymentRejectionDto request)
    {
        var result = await service.RejectPaymentAsync(request,id);
        return Ok(result);
    }
    
    [HttpPost("{id:long}/accept")]
    [EndpointName("accept")]
    [Consumes("multipart/form-data")]
    [EndpointSummary("accept payment request")]
    [EndpointDescription("user must accept payment request by this method")]
    [ProducesResponseType(typeof(ApiResult<PaymentResultDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SubmitReceipt([FromRoute] long id,IFormFile receipt)
    {
        var result = await service.SubmitReceiptAsync(id,receipt);
        return Ok(result);
    }

    [HttpGet("user/{id:long}")]
    [EndpointName("get-user-request")]
    [EndpointSummary("get a payment request by id for user")]
    [EndpointDescription("user can get a payment request by id")]
    [ProducesResponseType(typeof(ApiResult<PaymentRequestDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PaymentRequestDto>> GetPaymentForUser([FromRoute] long id)
    {
        var request = await service.GetPaymentRequestByIdForUserAsync(id);
        return Ok(request);
    }
    
    [HttpGet("admin/{id:long}")]
    [EndpointName("get-admin-request")]
    [EndpointSummary("get a payment request by id for admin")]
    [EndpointDescription("admin can get a payment request by id")]
    [ProducesResponseType(typeof(ApiResult<PaymentRequestDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PaymentRequestDto>> Get([FromRoute] long id)
    {
        var request = await service.GetPaymentRequestByIdForAdminAsync(id);
        return Ok(request);
    }
    
    [HttpGet("admin")]
    [EndpointName("get-all-request")]
    [EndpointSummary("get all payment requests for admin")]
    [EndpointDescription("admin can get all payment requests")]
    [SuperAdminAuthorize]
    [ProducesResponseType(typeof(ApiResult<BasePaging<PaymentRequestDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<BasePaging<PaymentRequestDto>>> GetAllRequests([FromQuery] PaymentRequestFilterDto filter)
    {
        var request = await service.GetPaymentsForAdminAsync(filter);
        return Ok(request);
    }
    
    [HttpGet("user")]
    [EndpointName("all-user-request")]
    [EndpointSummary("get all payment requests of user")]
    [EndpointDescription("user can get all payment requests")]
    [ProducesResponseType(typeof(ApiResult<BasePaging<PaymentRequestDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<BasePaging<PaymentRequestDto>>> GetAllUserRequests([FromQuery] PaymentRequestFilterDto filter)
    {
        var request = await service.GetPaymentsForUserAsync(filter);
        return Ok(request);
    }
    
    
    [HttpPost("ipn")]
    [AllowAnonymous]
    [EndpointName("plisio-payment-callback")]
    [EndpointSummary("receives payment status updates from Plisio.")]
    [EndpointDescription(
        "this is a webhook endpoint for plisio to send transaction status updates. do not call this directly.")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Ipn([FromForm] PlisioIpnDto ipn, [FromQuery] long prId)
    {
        await service.HandlePlisioIpnAsync(prId, ipn);
        return Ok("OK");
    }
    
}