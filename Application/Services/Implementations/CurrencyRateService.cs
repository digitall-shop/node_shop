using Application.Services.Interfaces;
using AutoMapper;
using Domain.Contract;
using Domain.DTOs.CurrencyReta;
using Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Application.Services.Implementations;

public class CurrencyRateService(
    IAsyncRepository<CurrencyRate, long> repository,
    ILogger<CurrencyRateService> logger,
    IMapper mapper)
    : ICurrencyRateService
{
    public async Task<IReadOnlyList<CurrencyRate>> GetAllCurrencyRatesAsync()
    {
        logger.LogInformation("this method for get all currency rates async");
        return await repository.GetAllAsync();
    }

    public async Task<CurrencyRateDto> GetCurrencyRateByIdAsync(long currencyId)
    {
        logger.LogInformation("this method for get currency rate by id async");
        CurrencyRate? currencyRate = await repository.GetByIdAsync(currencyId);
        return mapper.Map<CurrencyRateDto>(currencyRate);
    }

    public async Task<CurrencyRateDto> GetCurrencyRateByCodeAsync(string code)
    {
        logger.LogInformation("this method for get currency rate by code async");

        var currencyRate = await repository.GetSingleAsync((x) => x.CurrencyCode == code.Trim());

        return mapper.Map<CurrencyRateDto>(currencyRate);
    }

    public async Task<CurrencyRateDto> CreateCurrencyRateAsync(CurrencyRateCreateDto currency)
    {
        logger.LogInformation("this method for create new currency rate async");
        var newCurrencyRate = mapper.Map<CurrencyRate>(currency);
        
        await repository.AddEntity(newCurrencyRate);
        await repository.SaveChangesAsync();
        return mapper.Map<CurrencyRateDto>(newCurrencyRate);
    }

    public async Task<CurrencyRateDto> UpdateCurrencyRateAsync(CurrencyRateUpdateDto currency, long currencyId)
    {
        logger.LogInformation("this method for update currency rate async");

        var existingCurrency = await repository.GetByIdAsync(currencyId) ??
                               throw new KeyNotFoundException($"CurrencyRate with ID {currencyId} not found.");

        mapper.Map(currency, existingCurrency);
        existingCurrency.ModifiedDate = DateTime.UtcNow;
        existingCurrency.RateToBase = currency.ReteToBase;

        await repository.UpdateEntity(existingCurrency);
        await repository.SaveChangesAsync();

        return mapper.Map<CurrencyRateDto>(existingCurrency);
    }
}