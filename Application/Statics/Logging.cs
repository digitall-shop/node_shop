using Microsoft.Extensions.Logging;
using Serilog.Context;

namespace Application.Statics
{
    /// <summary>
    /// Business logging helpers:
    /// - Adds scoped properties (Topic, EventType, …) to each log line.
    /// - Uses Microsoft ILogger, but enriches via Serilog LogContext.
    /// - Designed to work with your TelegramTopic sink (TopicPropertyName = "Topic").
    /// </summary>
    public static class Logging
    {
        private sealed class DisposeAll(IList<IDisposable> items) : IDisposable
        {
            public void Dispose()
            {
                for (var i = items.Count - 1; i >= 0; i--)
                    items[i].Dispose();
            }
        }

        private static IDisposable PushProps(params (string Key, object? Value)[] props)
        {
            var list = new List<IDisposable>(props.Length);
            foreach (var (k, v) in props)
                list.Add(LogContext.PushProperty(k, v ?? ""));
            return new DisposeAll(list);
        }

        // =====================================================================================
        // BANK ACCOUNTS  (Topic = Payment)
        // =====================================================================================
        public static void BankAccountCreated(this ILogger logger, long accountId, bool isActive)
        {
            using var _ = PushProps(("Topic", "Payment"), ("EventType", "BankAccount.Created"),
                ("AccountId", accountId), ("IsActive", isActive));
            logger.LogInformation("Bank account created. AccountId={AccountId} IsActive={IsActive}", accountId,
                isActive);
        }

        public static void BankAccountUpdated(this ILogger logger, long accountId)
        {
            using var _ = PushProps(("Topic", "Payment"), ("EventType", "BankAccount.Updated"),
                ("AccountId", accountId));
            logger.LogInformation("Bank account updated. AccountId={AccountId}", accountId);
        }

        public static void BankAccountSoftDeleted(this ILogger logger, long accountId)
        {
            using var _ = PushProps(("Topic", "Payment"), ("EventType", "BankAccount.Deleted"),
                ("AccountId", accountId));
            logger.LogWarning("Bank account soft-deleted. AccountId={AccountId}", accountId);
        }

        public static void BankAccountsVisibilityChanged(this ILogger logger, bool isActive)
        {
            using var _ = PushProps(("Topic", "Payment"), ("EventType", "BankAccount.BulkVisibility"),
                ("TargetIsActive", isActive));
            logger.LogInformation("All bank accounts visibility changed. NewIsActive={TargetIsActive}", isActive);
        }

        // =====================================================================================
        // BROADCAST (Topic = Broadcast)
        // =====================================================================================
        public static void BroadcastStarted(this ILogger logger, string? parseMode)
        {
            using var _ = PushProps(("Topic", "Broadcast"), ("EventType", "Broadcast.Started"),
                ("ParseMode", parseMode ?? "Markdown"));
            logger.LogInformation("Broadcast started. ParseMode={ParseMode}", parseMode ?? "Markdown");
        }

        public static void BroadcastFinished(this ILogger logger, int total, int sent, int failed)
        {
            using var _ = PushProps(("Topic", "Broadcast"), ("EventType", "Broadcast.Finished"),
                ("Total", total), ("Sent", sent), ("Failed", failed));
            logger.LogInformation("Broadcast finished. Total={Total} Sent={Sent} Failed={Failed}", total, sent, failed);
        }

        public static void BroadcastWarnUser(this ILogger logger, long userId, string? reason)
        {
            using var _ = PushProps(("Topic", "Broadcast"), ("EventType", "Broadcast.UserWarn"),
                ("UserId", userId));
            logger.LogWarning("Broadcast failed for user. UserId={UserId} Reason={Reason}", userId, reason ?? "-");
        }

        public static void BroadcastErrorUser(this ILogger logger, long userId, Exception ex)
        {
            using var _ = PushProps(("Topic", "Broadcast"), ("EventType", "Broadcast.UserError"),
                ("UserId", userId));
            logger.LogError(ex, "Broadcast error for user. UserId={UserId}", userId);
        }

        public static void DirectMessageResult(this ILogger logger, long userId, bool success, string? note = null)
        {
            using var _ = PushProps(("Topic", "Broadcast"), ("EventType", "DirectMessage.Result"),
                ("UserId", userId), ("Success", success), ("Note", note ?? ""));
            if (success)
                logger.LogInformation("Direct message sent. UserId={UserId}", userId);
            else
                logger.LogWarning("Direct message NOT sent. UserId={UserId} Note={Note}", userId, note ?? "");
        }

        public static void DirectMessageError(this ILogger logger, long userId, Exception ex)
        {
            using var _ = PushProps(("Topic", "Broadcast"), ("EventType", "DirectMessage.Error"),
                ("UserId", userId));
            logger.LogError(ex, "Direct message error. UserId={UserId}", userId);
        }

        // =====================================================================================
        // USER (Topic = User)
        // =====================================================================================

        public static void UserCreated(this ILogger logger, long userId, string? username)
        {
            using var _ = PushProps(("Topic", "User"), ("EventType", "User.Created"),
                ("UserId", userId), ("Username", username ?? ""));
            logger.LogInformation("User created. UserId={UserId} Username={Username}", userId, username ?? "");
        }

        public static void UserFound(this ILogger logger, long userId, string? username)
        {
            using var _ = PushProps(("Topic", "User"), ("EventType", "User.Found"),
                ("UserId", userId), ("Username", username ?? ""));
            logger.LogInformation("User found. UserId={UserId} Username={Username}", userId, username ?? "");
        }

        public static void UserBalanceAdjusted(this ILogger logger, long userId, long delta, long newBalance)
        {
            using var _ = PushProps(("Topic", "User"), ("EventType", "User.BalanceAdjusted"),
                ("UserId", userId), ("Delta", delta), ("NewBalance", newBalance));
            logger.LogInformation("User balance adjusted. UserId={UserId} Delta={Delta} NewBalance={NewBalance}",
                userId, delta, newBalance);
        }

        public static void UserCreditUpdated(this ILogger logger, long userId, decimal delta, decimal newCredit)
        {
            using var _ = PushProps(("Topic", "User"), ("EventType", "User.CreditUpdated"),
                ("UserId", userId), ("Delta", delta), ("NewCredit", newCredit));
            logger.LogInformation("User credit updated. UserId={UserId} Delta={Delta} NewCredit={NewCredit}",
                userId, delta, newCredit);
        }

        public static void UserPriceMultiplierUpdated(this ILogger logger, long userId, decimal multiplier)
        {
            using var _ = PushProps(("Topic", "User"), ("EventType", "User.PriceMultiplierUpdated"),
                ("UserId", userId), ("Multiplier", multiplier));
            logger.LogInformation("User price multiplier updated. UserId={UserId} Multiplier={Multiplier}",
                userId, multiplier);
        }

        public static void UserBlockedStatusSet(this ILogger logger, long userId, bool isBlocked, long? byAdminId)
        {
            using var _ = PushProps(("Topic", "User"), ("EventType", "User.BlockStatusSet"),
                ("UserId", userId), ("IsBlocked", isBlocked), ("ByAdminId", byAdminId ?? 0));
            if (isBlocked)
                logger.LogWarning("User blocked. UserId={UserId} ByAdminId={ByAdminId}", userId, byAdminId ?? 0);
            else
                logger.LogInformation("User unblocked. UserId={UserId} ByAdminId={ByAdminId}", userId, byAdminId ?? 0);
        }

        public static void UserPaymentAccessSet(this ILogger logger, long userId, string? access)
        {
            using var _ = PushProps(("Topic", "User"), ("EventType", "User.PaymentAccessSet"),
                ("UserId", userId), ("Access", access ?? ""));
            logger.LogInformation("User payment access set. UserId={UserId} Access={Access}", userId, access ?? "");
        }

        public static void UserPromotedToSuperAdmin(this ILogger logger, long userId, long? byAdminId)
        {
            using var _ = PushProps(("Topic", "User"), ("EventType", "User.Promoted"),
                ("UserId", userId), ("ByAdminId", byAdminId ?? 0));
            logger.LogInformation("User promoted to super-admin. UserId={UserId} ByAdminId={ByAdminId}",
                userId, byAdminId ?? 0);
        }


        // =====================================================================================
        // PAYMENT (Topic = Payment)
        // =====================================================================================
        public static void PaymentRequestCreated(this ILogger logger, long paymentId, long userId, decimal amount,
            string method)
        {
            using var _ = PushProps(("Topic", "Payment"), ("EventType", "Payment.RequestCreated"),
                ("PaymentId", paymentId), ("UserId", userId),
                ("Amount", amount), ("Method", method));
            logger.LogInformation(
                "Payment request created. PaymentId={PaymentId} UserId={UserId} Amount={Amount} Method={Method}",
                paymentId, userId, amount, method);
        }

        public static void PaymentReceiptSubmitted(this ILogger logger, long paymentId, long userId, decimal amount)
        {
            using var _ = PushProps(("Topic", "Payment"), ("EventType", "Payment.ReceiptSubmitted"),
                ("PaymentId", paymentId), ("UserId", userId), ("Amount", amount));
            logger.LogInformation("Payment receipt submitted. PaymentId={PaymentId} UserId={UserId} Amount={Amount}",
                paymentId, userId, amount);
        }

        public static void PaymentApproved(this ILogger logger, long paymentId, long userId, decimal amount)
        {
            using var _ = PushProps(("Topic", "Payment"), ("EventType", "Payment.Approved"),
                ("PaymentId", paymentId), ("UserId", userId), ("Amount", amount));
            logger.LogInformation("Payment approved. PaymentId={PaymentId} UserId={UserId} Amount={Amount}", paymentId,
                userId, amount);
        }

        public static void PaymentRejected(this ILogger logger, long paymentId, long userId, decimal amount,
            string reason)
        {
            using var _ = PushProps(("Topic", "Payment"), ("EventType", "Payment.Rejected"),
                ("PaymentId", paymentId), ("UserId", userId), ("Amount", amount),
                ("Reason", reason));
            logger.LogWarning("Payment rejected. PaymentId={PaymentId} UserId={UserId} Amount={Amount} Reason={Reason}",
                paymentId, userId, amount, reason);
        }

        public static void PaymentMethodDeniedAttempt(this ILogger logger, long userId, string method)
        {
            using var _ = PushProps(("Topic", "Payment"), ("EventType", "Payment.DeniedAttempt"),
                ("UserId", userId), ("Method", method));
            logger.LogWarning("User attempted a denied payment method. UserId={UserId} Method={Method}", userId,
                method);
        }

        public static void PaymentMethodGranted(this ILogger logger, long userId, string method, long grantedBy)
        {
            using var _ = PushProps(("Topic", "Payment"), ("EventType", "Payment.MethodGranted"),
                ("UserId", userId), ("Method", method), ("GrantedBy", grantedBy));
            logger.LogInformation("Payment method granted. UserId={UserId} Method={Method} GrantedBy={GrantedBy}",
                userId, method, grantedBy);
        }

        public static void PaymentMethodRevoked(this ILogger logger, long userId, string method, long revokedBy)
        {
            using var _ = PushProps(("Topic", "Payment"), ("EventType", "Payment.MethodRevoked"),
                ("UserId", userId), ("Method", method), ("RevokedBy", revokedBy));
            logger.LogInformation("Payment method revoked. UserId={UserId} Method={Method} RevokedBy={RevokedBy}",
                userId, method, revokedBy);
        }

        // =====================================================================================
        // TRANSACTION (Topic = Transaction)
        // =====================================================================================
        public static void TransactionCreated(this ILogger logger, long userId, decimal amount, string type,
            string reason,
            long balanceBefore, long balanceAfter)
        {
            using var _ = PushProps(("Topic", "Transaction"), ("EventType", "Transaction.Created"),
                ("UserId", userId), ("Amount", amount), ("Type", type),
                ("Reason", reason), ("BalanceBefore", balanceBefore), ("BalanceAfter", balanceAfter));
            logger.LogInformation(
                "Transaction created. UserId={UserId} Type={Type} Amount={Amount} Reason={Reason} Balance: {BalanceBefore} -> {BalanceAfter}",
                userId, type, amount, reason, balanceBefore, balanceAfter);
        }

        // =====================================================================================
        // PANEL (Topic = Panel)
        // =====================================================================================

        public static void PanelCreated(this ILogger logger, long panelId, long userId, string url)
        {
            using var _ = PushProps(("Topic", "Panel"), ("EventType", "Panel.Created"),
                ("PanelId", panelId), ("UserId", userId), ("Url", url));
            logger.LogInformation("Panel created. PanelId={PanelId} UserId={UserId} Url={Url}", panelId, userId, url);
        }

        public static void PanelUpdated(this ILogger logger, long panelId, long userId)
        {
            using var _ = PushProps(("Topic", "Panel"), ("EventType", "Panel.Updated"),
                ("PanelId", panelId), ("UserId", userId));
            logger.LogInformation("Panel updated. PanelId={PanelId} UserId={UserId}", panelId, userId);
        }

        public static void PanelDeleted(this ILogger logger, long panelId, long userId)
        {
            using var _ = PushProps(("Topic", "Panel"), ("EventType", "Panel.Deleted"),
                ("PanelId", panelId), ("UserId", userId));
            logger.LogWarning("Panel deleted. PanelId={PanelId} UserId={UserId}", panelId, userId);
        }

        public static void PanelCertificateUpdated(this ILogger logger, long panelId)
        {
            using var _ = PushProps(("Topic", "Panel"), ("EventType", "Panel.CertificateUpdated"),
                ("PanelId", panelId));
            logger.LogInformation("Panel certificate updated. PanelId={PanelId}", panelId);
        }

        public static void PanelInboundsUpdated(this ILogger logger, long panelId, int? port, bool retry = false)
        {
            using var _ = PushProps(("Topic", "Panel"), ("EventType", "Panel.InboundsUpdated"),
                ("PanelId", panelId), ("Port", port), ("IsRetry", retry));
            logger.LogInformation("Panel inbounds updated. PanelId={PanelId} Port={Port} Retry={IsRetry}", panelId,
                port, retry);
        }

        public static void PanelFetched(this ILogger logger, long panelId, long userId)
        {
            using var _ = PushProps(("Topic", "Panel"), ("EventType", "Panel.Fetched"),
                ("PanelId", panelId), ("UserId", userId));
            logger.LogInformation("Get panel by id. PanelId={PanelId} UserId={UserId}", panelId, userId);
        }

        public static void AdminPanelsListed(this ILogger logger)
        {
            using var _ = PushProps(("Topic", "Panel"), ("EventType", "Panel.ListAdmin"));
            logger.LogInformation("List all panels for admin.");
        }

        public static void UserPanelsListed(this ILogger logger, long userId)
        {
            using var _ = PushProps(("Topic", "Panel"), ("EventType", "Panel.ListUser"), ("UserId", userId));
            logger.LogInformation("List panels for user. UserId={UserId}", userId);
        }

        public static void PanelPortsAllocated(this ILogger logger, int? xray, int? api, int? inbound)
        {
            using var _ = PushProps(("Topic", "Panel"), ("EventType", "Panel.PortsAllocated"),
                ("Xray", xray ?? 0), ("Api", api ?? 0), ("Inbound", inbound ?? 0));
            logger.LogInformation("Allocated ports. Xray={Xray} Api={Api} Inbound={Inbound}", xray, api, inbound);
        }

        public static void PanelTokenRefreshed(this ILogger logger, long panelId)
        {
            using var _ = PushProps(("Topic", "Panel"), ("EventType", "Panel.TokenRefreshed"),
                ("PanelId", panelId));
            logger.LogInformation("Panel token refreshed (retry). PanelId={PanelId}", panelId);
        }

        // =====================================================================================
        // INSTANCE (Topic = Instance)
        // =====================================================================================
        public static void InstanceCreated(this ILogger logger, long instanceId, long userId, long nodeId, long panelId)
        {
            using var _ = PushProps(("Topic", "Instance"), ("EventType", "Instance.Created"),
                ("InstanceId", instanceId), ("UserId", userId),
                ("NodeId", nodeId), ("PanelId", panelId));
            logger.LogInformation(
                "Instance created. InstanceId={InstanceId} UserId={UserId} NodeId={NodeId} PanelId={PanelId}",
                instanceId, userId, nodeId, panelId);
        }

        public static void InstanceStatusChanged(this ILogger logger, long instanceId, string oldStatus,
            string newStatus)
        {
            using var _ = PushProps(("Topic", "Instance"), ("EventType", "Instance.StatusChanged"),
                ("InstanceId", instanceId), ("OldStatus", oldStatus), ("NewStatus", newStatus));
            logger.LogInformation("Instance status changed. InstanceId={InstanceId} {OldStatus} -> {NewStatus}",
                instanceId, oldStatus, newStatus);
        }

        public static void InstanceSuspended(this ILogger logger, long instanceId, long userId, string reason)
        {
            using var _ = PushProps(("Topic", "Instance"), ("EventType", "Instance.Suspended"),
                ("InstanceId", instanceId), ("UserId", userId), ("Reason", reason));
            logger.LogWarning("Instance suspended. InstanceId={InstanceId} UserId={UserId} Reason={Reason}", instanceId,
                userId, reason);
        }

        public static void InstanceResumed(this ILogger logger, long instanceId, long userId, string by)
        {
            using var _ = PushProps(("Topic", "Instance"), ("EventType", "Instance.Resumed"),
                ("InstanceId", instanceId), ("UserId", userId), ("By", by));
            logger.LogInformation("Instance resumed. InstanceId={InstanceId} UserId={UserId} By={By}", instanceId,
                userId, by);
        }

        public static void InstanceDeleted(this ILogger logger, long instanceId, long userId)
        {
            using var _ = PushProps(("Topic", "Instance"), ("EventType", "Instance.Deleted"),
                ("InstanceId", instanceId), ("UserId", userId));
            logger.LogWarning("Instance deleted. InstanceId={InstanceId} UserId={UserId}", instanceId, userId);
        }

        // =====================================================================================
        // SUPPORT (Topic = Support)
        // =====================================================================================
        public static void SupportTicketCreated(this ILogger logger, long ticketId, long userId, string? subject)
        {
            using var _ = PushProps(("Topic", "Support"), ("EventType", "Support.TicketCreated"),
                ("TicketId", ticketId), ("UserId", userId), ("Subject", subject ?? ""));
            logger.LogInformation("Support ticket created. TicketId={TicketId} UserId={UserId}", ticketId, userId);
        }

        public static void SupportMessageAdded(this ILogger logger, long ticketId, long messageId, bool isFromAdmin)
        {
            using var _ = PushProps(("Topic", "Support"), ("EventType", "Support.MessageAdded"),
                ("TicketId", ticketId), ("MessageId", messageId), ("IsFromAdmin", isFromAdmin));
            logger.LogInformation(
                "Support message added. TicketId={TicketId} MessageId={MessageId} IsFromAdmin={IsFromAdmin}",
                ticketId, messageId, isFromAdmin);
        }

        public static void SupportTicketAssigned(this ILogger logger, long ticketId, long adminId)
        {
            using var _ = PushProps(("Topic", "Support"), ("EventType", "Support.Assigned"),
                ("TicketId", ticketId), ("AdminId", adminId));
            logger.LogInformation("Support ticket assigned. TicketId={TicketId} AdminId={AdminId}", ticketId, adminId);
        }

        public static void SupportTicketClosed(this ILogger logger, long ticketId, long byUserId)
        {
            using var _ = PushProps(("Topic", "Support"), ("EventType", "Support.Closed"),
                ("TicketId", ticketId), ("ByUserId", byUserId));
            logger.LogInformation("Support ticket closed. TicketId={TicketId} ByUserId={ByUserId}", ticketId, byUserId);
        }

        // ==============================
// USER (Topic = User) - افزوده‌ها
// ==============================
        public static void UserRegistered(this ILogger logger, long userId, string? username, string? fullName)
        {
            using var _ = PushProps(
                ("Topic", "User"),
                ("EventType", "User.Registered"),
                ("UserId", userId),
                ("Username", username ?? ""),
                ("FullName", fullName ?? "")
            );
            logger.LogInformation("New user registered. UserId={UserId} Username={Username}", userId, username ?? "");
        }

        public static void NewUserAdminNotifyResult(this ILogger logger, long adminId, long userId, bool success,
            string? error = null)
        {
            using var _ = PushProps(
                ("Topic", "User"),
                ("EventType", "User.Registered.AdminNotify"),
                ("AdminId", adminId),
                ("UserId", userId),
                ("Success", success)
            );
            if (success)
                logger.LogInformation("Admin notified about new user. AdminId={AdminId} UserId={UserId}", adminId,
                    userId);
            else
                logger.LogWarning("Failed to notify admin. AdminId={AdminId} UserId={UserId} Error={Error}", adminId,
                    userId, error ?? "");
        }

        public static void NoSuperAdminsToNotify(this ILogger logger, long userId)
        {
            using var _ = PushProps(
                ("Topic", "User"),
                ("EventType", "User.Registered.NoAdmins"),
                ("UserId", userId)
            );
            logger.LogWarning("No super admins to notify. UserId={UserId}", userId);
        }

        public static void UserRegisteredHandlerError(this ILogger logger, long userId, Exception ex)
        {
            using var _ = PushProps(
                ("Topic", "User"),
                ("EventType", "User.Registered.Error"),
                ("UserId", userId)
            );
            logger.LogError(ex, "Failed to process new user registration event. UserId={UserId}", userId);
        }

        // ==============================
// PANEL (Topic = Panel) - افزوده‌ها برای Marzban
// ==============================
        public static void PanelInboundEnsured(this ILogger logger, long panelId, int port, string tag, bool createdNew)
        {
            using var _ = PushProps(
                ("Topic", "Panel"),
                ("EventType", "Panel.InboundEnsured"),
                ("PanelId", panelId),
                ("Port", port),
                ("Tag", tag),
                ("CreatedNew", createdNew)
            );
            logger.LogInformation(
                createdNew
                    ? "Inbound created/added on panel. PanelId={PanelId} Port={Port} Tag={Tag}"
                    : "Inbound already exists on panel. PanelId={PanelId} Port={Port} Tag={Tag}",
                panelId, port, tag);
        }

        public static void PanelNodeCreatedInMarzban(this ILogger logger, long panelId, long nodeId, string nodeName,
            long marzbanNodeId)
        {
            using var _ = PushProps(
                ("Topic", "Panel"),
                ("EventType", "Panel.NodeCreatedInMarzban"),
                ("PanelId", panelId),
                ("NodeId", nodeId),
                ("NodeName", nodeName),
                ("MarzbanNodeId", marzbanNodeId)
            );
            logger.LogInformation(
                "Node created in Marzban. PanelId={PanelId} NodeId={NodeId} NodeName={NodeName} MarzbanNodeId={MarzbanNodeId}",
                panelId, nodeId, nodeName, marzbanNodeId);
        }

        public static void PanelHostAssignedToInbound(this ILogger logger, long panelId, string tag, string host,
            string nodeName)
        {
            using var _ = PushProps(
                ("Topic", "Panel"),
                ("EventType", "Panel.HostAssignedToInbound"),
                ("PanelId", panelId),
                ("Tag", tag),
                ("Host", host),
                ("NodeName", nodeName)
            );
            logger.LogInformation(
                "Host assigned to inbound. PanelId={PanelId} Tag={Tag} Host={Host} NodeName={NodeName}",
                panelId, tag, host, nodeName);
        }

        public static void PanelTokenRefreshStarted(this ILogger logger, long panelId)
        {
            using var _ = PushProps(
                ("Topic", "Panel"),
                ("EventType", "Panel.TokenRefresh.Started"),
                ("PanelId", panelId)
            );
            logger.LogWarning("Refreshing Marzban token... PanelId={PanelId}", panelId);
        }

        public static void PanelTokenRefreshSucceeded(this ILogger logger, long panelId)
        {
            using var _ = PushProps(
                ("Topic", "Panel"),
                ("EventType", "Panel.TokenRefresh.Succeeded"),
                ("PanelId", panelId)
            );
            logger.LogInformation("Marzban token refreshed. PanelId={PanelId}", panelId);
        }

        public static void PanelOperationError(this ILogger logger, long panelId, string when, Exception ex)
        {
            using var _ = PushProps(
                ("Topic", "Panel"),
                ("EventType", "Panel.OperationError"),
                ("PanelId", panelId),
                ("When", when)
            );
            logger.LogError(ex, "Panel operation error at {When}. PanelId={PanelId}", when, panelId);
        }

        // ==============================
// SUPPORT (Notification helpers)
// ==============================
        public static void SupportNotifyUser(this ILogger logger, long ticketId, long userId, long messageId)
        {
            using var _ = PushProps(("Topic", "Support"), ("EventType", "Support.Notify.User"),
                ("TicketId", ticketId), ("UserId", userId), ("MessageId", messageId));
            logger.LogInformation(
                "Support notification sent to user. TicketId={TicketId} UserId={UserId} MessageId={MessageId}",
                ticketId, userId, messageId);
        }

        public static void SupportNotifyAdmin(this ILogger logger, long ticketId, long adminId, long messageId)
        {
            using var _ = PushProps(("Topic", "Support"), ("EventType", "Support.Notify.Admin"),
                ("TicketId", ticketId), ("AdminId", adminId), ("MessageId", messageId));
            logger.LogInformation(
                "Support notification sent to admin. TicketId={TicketId} AdminId={AdminId} MessageId={MessageId}",
                ticketId, adminId, messageId);
        }

        public static void SupportTicketNotifyAdmins(this ILogger logger, long ticketId, int adminCount)
        {
            using var _ = PushProps(("Topic", "Support"), ("EventType", "Support.TicketNotifyAdmins"),
                ("TicketId", ticketId), ("AdminCount", adminCount));
            logger.LogInformation("Support ticket notify admins. TicketId={TicketId} AdminCount={AdminCount}",
                ticketId, adminCount);
        }

        public static void SupportNotifyErrorUser(this ILogger logger, long ticketId, long userId, Exception ex)
        {
            using var _ = PushProps(("Topic", "Support"), ("EventType", "Support.Notify.Error"),
                ("TicketId", ticketId), ("UserId", userId));
            logger.LogError(ex, "Support notification error to user. TicketId={TicketId} UserId={UserId}",
                ticketId, userId);
        }

        public static void SupportNotifyErrorAdmin(this ILogger logger, long ticketId, long adminId, Exception ex)
        {
            using var _ = PushProps(("Topic", "Support"), ("EventType", "Support.Notify.Error"),
                ("TicketId", ticketId), ("AdminId", adminId));
            logger.LogError(ex, "Support notification error to admin. TicketId={TicketId} AdminId={AdminId}",
                ticketId, adminId);
        }

        // ==============================
// PAYMENT (notification helpers)
// ==============================
        public static void PaymentApprovedUserNotified(this ILogger logger, long paymentId, long userId)
        {
            using var _ = PushProps(("Topic", "Payment"), ("EventType", "Payment.Notify.Approved"),
                ("PaymentId", paymentId), ("UserId", userId));
            logger.LogInformation("Payment approved notification sent to user. PaymentId={PaymentId} UserId={UserId}",
                paymentId, userId);
        }

        public static void PaymentRejectedUserNotified(this ILogger logger, long paymentId, long userId)
        {
            using var _ = PushProps(("Topic", "Payment"), ("EventType", "Payment.Notify.Rejected"),
                ("PaymentId", paymentId), ("UserId", userId));
            logger.LogInformation("Payment rejected notification sent to user. PaymentId={PaymentId} UserId={UserId}",
                paymentId, userId);
        }

        public static void PaymentSubmittedAdminNotified(this ILogger logger, long paymentId, long adminId)
        {
            using var _ = PushProps(("Topic", "Payment"), ("EventType", "Payment.Notify.Submitted.Admin"),
                ("PaymentId", paymentId), ("AdminId", adminId));
            logger.LogInformation(
                "Payment submitted notification sent to admin. PaymentId={PaymentId} AdminId={AdminId}",
                paymentId, adminId);
        }

        public static void PaymentDeniedAttemptAdminsNotified(this ILogger logger, long userId, string method,
            int adminCount)
        {
            using var _ = PushProps(("Topic", "Payment"), ("EventType", "Payment.DeniedAttempt.AdminsNotified"),
                ("UserId", userId), ("Method", method), ("AdminCount", adminCount));
            logger.LogInformation(
                "Denied payment method attempt: admins notified. UserId={UserId} Method={Method} AdminCount={AdminCount}",
                userId, method, adminCount);
        }

        public static void PaymentNotifyErrorUser(this ILogger logger, long paymentId, long userId, Exception ex)
        {
            using var _ = PushProps(("Topic", "Payment"), ("EventType", "Payment.Notify.Error.User"),
                ("PaymentId", paymentId), ("UserId", userId));
            logger.LogError(ex, "Payment notification error to user. PaymentId={PaymentId} UserId={UserId}",
                paymentId, userId);
        }

        public static void PaymentNotifyErrorAdmin(this ILogger logger, long paymentId, long adminId, Exception ex)
        {
            using var _ = PushProps(("Topic", "Payment"), ("EventType", "Payment.Notify.Error.Admin"),
                ("PaymentId", paymentId), ("AdminId", adminId));
            logger.LogError(ex, "Payment notification error to admin. PaymentId={PaymentId} AdminId={AdminId}",
                paymentId, adminId);
        }
        
        public static void LowBalanceAlertSent(this ILogger logger, long userId, decimal available, decimal threshold, int activeCount)
        {
            using var _ = PushProps(("Topic","User"), ("EventType","User.LowBalanceAlert"),
                ("UserId", userId), ("Available", available),
                ("Threshold", threshold), ("ActiveInstances", activeCount));
            logger.LogWarning("Low balance alert sent. UserId={UserId} Available={Available} Threshold={Threshold} ActiveInstances={ActiveInstances}",
                userId, available, threshold, activeCount);
        }
        public static void LowBalanceAlertReset(this ILogger logger, long userId, decimal available, decimal threshold, decimal resetDelta)
        {
            using var _ = PushProps(("Topic","User"), ("EventType","User.LowBalanceAlertReset"),
                ("UserId", userId), ("Available", available),
                ("Threshold", threshold), ("ResetDelta", resetDelta));
            logger.LogInformation("Low balance alert reset. UserId={UserId} Available={Available} ResetAt={ResetAt}",
                userId, available, threshold + resetDelta);
        }

    }
}