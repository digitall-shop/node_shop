namespace Domain.DTOs.Broadcast;

public class BroadcastResultDto
{
    public int TotalTargets { get; set; }
    public int Sent { get; set; }
    public int Failed { get; set; }
    public List<long> FailedUserIds { get; set; } = [];
}