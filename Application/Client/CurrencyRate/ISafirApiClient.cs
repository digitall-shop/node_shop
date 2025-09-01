using Application.Models.Safir;

namespace Application.Client.CurrencyRate;

public interface ISafirApiClient
{
    /// <summary>
    /// this for get and update rate of USD
    /// </summary>
    /// <returns></returns>
    Task<GetRateResponse> GetRateAsync();
}