using System.Text.Json.Serialization;

namespace Application.Models.NodeManager;


    public class ProvisionRequestDto
    {
        public string XrayContainerImage { get; set; }
        public long CustomerId { get; set; }
        public long NodeId { get; set; }
        public int InboundPort  { get; set; }
        public int XrayPort { get; set; }
        public int ApiPort { get; set; }
        public string? CertificateKey { get; set; } = null!;
        public long InstanceId { get; set; }
        public bool ShouldSerializeNodeId() => false;
        
    }
    
    public class ProvisionResponseDto
    {
        public bool IsSuccess { get; set; }
        
        public string? ErrorMessage { get; set; }
        public string? ContainerDockerId { get; set; }
        
    }
    public class TrafficUsageDto
    {
        [JsonPropertyName("total_bytes_in")]
        public long TotalBytesIn { get; set; }

        [JsonPropertyName("total_bytes_out")]
        public long TotalBytesOut { get; set; }
    }

    public class ContainerActionRequestDto 
    {
        public string? ContainerId { get; set; }
        public long NodeId { get; set; }
        public bool ShouldSerializeNodeId() => false;
    }