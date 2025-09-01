using Newtonsoft.Json;

namespace Application.Models.Marzban;

public class MarzbanLoginRequest
{
    [JsonProperty("username")] public string? Username { get; set; } = null!;
    [JsonProperty("password")] public string? Password { get; set; } = null!;
    [JsonProperty("grant_type")] public string GrantType { get; set; } = "password";
    [JsonProperty("path")] public string? Path { get; set; } = null!;
    public bool ShouldSerializePath() => false;
}

public class MarzbanLoginResponse
{
    [JsonProperty("access_token")] public string AccessToken { get; set; }

    [JsonProperty("token_type")] public string TokenType { get; set; }

    [JsonProperty("expires_in")] public int ExpiresIn { get; set; }

    public bool ShouldSerializeExpiresIn() => false;
}

public class MarzbanNodeCreateRequest
{
    [JsonProperty("add_as_new_host")] public bool AddAsNewHost { get; set; }

    [JsonProperty("address")] public string? Address { get; set; }

    [JsonProperty("api_port")] public int? ApiPort { get; set; }
    [JsonProperty("name")] public string? Name { get; set; }
    [JsonProperty("port")] public int Port { get; set; }
    [JsonProperty("usage_coefficient")] public decimal UsageCoefficient { get; set; } = 1.0m;
    [JsonProperty("path")] public string Path { get; set; } = null!;
    [JsonProperty("access_token")] public string? Token { get; set; }
    public bool ShouldSerializePath() => false;
    public bool ShouldSerializeToken() => false;
}

public class MarzbanNodeCreateResponse
{
    [JsonProperty("name")] public string Name { get; set; } = null!;

    [JsonProperty("address")] public string Address { get; set; } = null!;

    [JsonProperty("port")] public int Port { get; set; }

    [JsonProperty("usage_coefficient")] public decimal UsageCoefficient { get; set; }
    [JsonProperty("id")] public long Id { get; set; }

    [JsonProperty("status")] public string Status { get; set; }

    [JsonProperty("message")] public string? Message { get; set; }
}

public class MarzbanNodeGetSettingRequest
{
    [JsonProperty("token")] public string? Token { get; set; }
    [JsonProperty("path")] public string Path { get; set; } = null!;
    public bool ShouldSerializeToken() => false;
    public bool ShouldSerializePath() => false;
}

public class MarzbanUpdateCoreConfigRequest
{
    [JsonProperty("token")] public string? Token { get; set; }
    [JsonProperty("path")] public string Path { get; set; } = null!;
    public bool ShouldSerializeToken() => false;
    public bool ShouldSerializePath() => false;
}

public class MarzbanNodeGetSettingResponse
{
    [JsonProperty("certificate")] public string Certificate { get; set; } = null!;
}

public class Host
{
    [JsonProperty("remark")] public string? Remark { get; set; }
    [JsonProperty("address")] public string Address { get; set; } = null!;
    [JsonProperty("port")] public int? Port { get; set; }
    [JsonProperty("sni")] public string? Sni { get; set; }
    [JsonProperty("host")] public string? Hostname { get; set; }
    [JsonProperty("path")] public string? Path { get; set; }
    [JsonProperty("security")] public string? Security { get; set; }
    [JsonProperty("alpn")] public string? Alpn { get; set; }
    [JsonProperty("fingerprint")] public string? Fingerprint { get; set; }
    [JsonProperty("allow_insecure")] public bool? AllowInsecure { get; set; }
    [JsonProperty("is_disabled")] public bool? IsDisabled { get; set; }
    [JsonProperty("mux_enable")] public bool? MuxEnable { get; set; }
    [JsonProperty("fragment_setting")] public string? FragmentSetting { get; set; }
    [JsonProperty("noise_setting")] public string? NoiseSetting { get; set; }
    [JsonProperty("random_user_agent")] public bool? RandomUserAgent { get; set; }
    [JsonProperty("use_sni_as_host")] public bool? UseSniAsHost { get; set; }
}

public class MarzbanModifyHostsRequest
{
    [JsonIgnore] public string Path { get; set; } = null!;
    [JsonIgnore] public string Token { get; set; } = null!;
    public string Tag { get; set; } = null!;
    public List<Host> Hosts { get; set; } = [];
}