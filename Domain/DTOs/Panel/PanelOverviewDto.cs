namespace Domain.DTOs.Panel;

public class PanelOverviewDto
{
    public long Id { get; set; }
    public string? Name { get; set; }
    public string? Url { get; set; }
    public int? XrayPort { get; set; }
    public int? ApiPort { get; set; }
    public int? ServerPort { get; set; }
    public string? UserName { get; set; }
    public long UserId { get; set; }
}