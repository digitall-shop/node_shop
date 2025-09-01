namespace Domain.DTOs.Panel;

public class PanelCreateDto
{ 
    public string? Name { get; set; }
    public string? Url { get; set; }
    public string? UserName { get; set; }
    public string? Password { get; set; }

    public bool SSL { get; set; }
}
