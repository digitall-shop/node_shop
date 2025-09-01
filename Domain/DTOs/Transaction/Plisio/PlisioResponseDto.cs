namespace Domain.DTOs.Transaction.Plisio;

[Flags]
public enum PlisioResponseEnum : byte
{
    Success,
    Error
}

public class PlisioData
{
    public string? Txn_Id { get; set; }
    public string? Invoice_Url { get; set; }
    public string? Name { get; set; }
    public string? Message { get; set; }
    public int? Code { get; set; }
}

public class PlisioResponseDto
{
    public PlisioResponseEnum Status { get; set; }
    public PlisioData Data { get; set; }
}