using Newtonsoft.Json;

namespace Application.Extensions;

public static class JsonExtensions
{
    public static string SerializeModelToJsonObject(this Dictionary<string, string> model)
    {
        return JsonConvert.SerializeObject(model);
    }
    
}