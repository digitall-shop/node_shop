using Domain.DTOs.CurrencyReta;
using Domain.Entities;

namespace Application.Services.Interfaces;

public interface ICurrencyRateService
{
    /// <summary>
    /// this method for get all currency rates
    /// </summary>
    /// <returns></returns>
    Task<IReadOnlyList<CurrencyRate>> GetAllCurrencyRatesAsync();
    
    /// <summary>
    /// this method for get a currency rate by id
    /// </summary>
    /// <param name="currencyId"></param>
    /// <returns></returns>
    Task<CurrencyRateDto> GetCurrencyRateByIdAsync(long currencyId);
    
    /// <summary>
    /// this method for get a currency rate by id
    /// </summary>
    /// <param name="code"></param>
    /// <returns></returns>
    Task<CurrencyRateDto> GetCurrencyRateByCodeAsync(string code);
    
    /// <summary>
    /// this method for create new currency rete
    /// </summary>
    /// <param name="currency"></param>
    /// <returns></returns>
    Task<CurrencyRateDto> CreateCurrencyRateAsync(CurrencyRateCreateDto currency);
    
    /// <summary>
    /// this method for update a currency rate
    /// </summary>
    /// <param name="currency"></param>
    /// <param name="currencyId"></param>
    /// <returns></returns>
    Task<CurrencyRateDto> UpdateCurrencyRateAsync(CurrencyRateUpdateDto currency,long currencyId);
    
    
}