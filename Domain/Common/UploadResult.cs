namespace Domain.Common;

public class UploadResult
{
    public bool Succeeded { get; set; }
    public string? FilePath { get; set; }
    public string? ErrorMessage { get; set; }
}