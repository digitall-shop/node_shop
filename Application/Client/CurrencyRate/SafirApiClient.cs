using System.Net.Http.Headers;
using Application.Models.Safir;
using Domain.Exceptions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Application.Client.CurrencyRate;

public class SafirApiClient(ILogger<ISafirApiClient> logger) : ISafirApiClient
{
    public async Task<GetRateResponse> GetRateAsync()
    {
        logger.LogInformation("get rate");
        
        using var client = new HttpClient();
        client.BaseAddress = new Uri("https://sarfe.erfjab.com");
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        HttpRequestMessage requestMessage = new(HttpMethod.Get, SafirUrlPath.GetRate);
        

        var response = await client.SendAsync(requestMessage);
        var responseContent = await response.Content.ReadAsStringAsync();

        if (responseContent is null || responseContent.Length == 0)
        {
            throw new BadRequestException("API request failed");
        }
        return JsonConvert.DeserializeObject<GetRateResponse>(responseContent);
    }
}