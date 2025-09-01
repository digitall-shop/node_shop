using Api.Filters;
using Api.Securities;
using Application.Services.Interfaces;
using Domain.DTOs.CurrencyReta;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Tags("Currency Rate Endpoint")]
[Route("api/currency rate")]
[SuperAdminAuthorize]
public class CurrencyRateController(ICurrencyRateService service) : ApiBaseController
{
    [HttpGet]
    [EndpointName("currency-rates")]
    [EndpointSummary("get currency rates")]
    [EndpointDescription("this for get all currency rates")]
    [ProducesResponseType(typeof(List<CurrencyRateDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<List<CurrencyRateDto>>> Get()
    {
        return Ok(await service.GetAllCurrencyRatesAsync());
    }

    [HttpGet("{currencyCode}")]
    [EndpointName("currency-rate")]
    [EndpointSummary("get currency rate")]
    [EndpointDescription("this for get currency rate by currency code")]
    [ProducesResponseType(typeof(CurrencyRateDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CurrencyRateDto>> Get([FromRoute] string currencyCode)
    {
        return Ok(await service.GetCurrencyRateByCodeAsync(currencyCode));
    }

    [HttpPost]
    [EndpointName("create-currency-rate")]
    [EndpointSummary("create new currency rate")]
    [EndpointDescription("this for create new currency rate")]
    [ProducesResponseType(typeof(CurrencyRateDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CurrencyRateDto>> Post([FromBody] CurrencyRateCreateDto currencyRate)
    {
        return Ok(await service.CreateCurrencyRateAsync(currencyRate));
    }

    [HttpPatch("{id}")]
    [EndpointName("update-currency-rates")]
    [EndpointSummary("update an existing currency rate")]
    [EndpointDescription("this for update an existing currency rate")]
    [ProducesResponseType(typeof(CurrencyRateDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status304NotModified)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CurrencyRateDto>> Patch([FromBody] CurrencyRateUpdateDto currencyRate,
        [FromRoute] long id)
    {
        return Ok(await service.UpdateCurrencyRateAsync(currencyRate, id));
    }
}