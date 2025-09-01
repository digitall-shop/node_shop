namespace Domain.DTOs.Panel;

public class PanelDto
{
    public long Id { get; set; }
    
    public string? Name { get; set; }
    public string Url { get; set; }
    public string UserName { get; set; }
    
    public long UserId { get; set; }
    public string Password { get; set; }
    public string? CertificateKey { get; set; }
    public int XrayPort { get; set; }
    public int ApiPort { get; set; }
    public int InboundPort { get; set; }
    public string Token { get; set; }
    
}