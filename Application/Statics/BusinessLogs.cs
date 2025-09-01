using Microsoft.Extensions.Logging;
using Serilog.Context;

namespace Application.Statics;

public static partial class BusinessLogs
{
    private static IDisposable Topic(string name) => LogContext.PushProperty("Topic", name);
    public static void InstanceProvisionRequested(this ILogger logger, long userId, long nodeId, long panelId)
    {
        using var _t = Topic("Instance");
        using var _u = LogContext.PushProperty("UserId", userId);
        using var _n = LogContext.PushProperty("NodeId", nodeId);
        using var _p = LogContext.PushProperty("PanelId", panelId);
        logger.LogInformation("Instance provision requested by user.");
    }

    public static void InstanceProvisionCompleted(this ILogger logger, long instanceId)
    {
        using var _t = Topic("Instance");
        using var _i = LogContext.PushProperty("InstanceId", instanceId);
        logger.LogInformation("Instance provision completed.");
    }

    public static void InstanceProvisionFailed(this ILogger logger, long instanceId, Exception ex)
    {
        using var _t = Topic("Instance");
        using var _i = LogContext.PushProperty("InstanceId", instanceId);
        logger.LogError(ex, "Instance provision failed.");
    }

    // NodeManager-side events
    public static void NodeProvisionRequestSent(this ILogger logger, long instanceId, long nodeId, long panelId)
    {
        using var _t = Topic("Node");
        using var _i = LogContext.PushProperty("InstanceId", instanceId);
        using var _n = LogContext.PushProperty("NodeId", nodeId);
        using var _p = LogContext.PushProperty("PanelId", panelId);
        logger.LogInformation("Provision request sent to NodeManager.");
    }

    public static void NodeProvisionResponse(this ILogger logger, long instanceId, bool isSuccess)
    {
        using var _t = Topic("Node");
        using var _i = LogContext.PushProperty("InstanceId", instanceId);
        logger.LogInformation("NodeManager provision response received. Success={IsSuccess}", isSuccess);
    }
}
