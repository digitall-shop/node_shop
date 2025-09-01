namespace Application.Options;

public sealed class LowBalanceAlertOptions
{
    public bool Enabled { get; set; } = true;
    public decimal PercentThreshold { get; set; } = 0.05m;
    public decimal ResetPercent { get; set; } = 0.07m;
    public long MinAbsThreshold { get; set; } 
}