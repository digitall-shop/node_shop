using Domain.Enumes.Transaction;
using Telegram.Bot.Types.ReplyMarkups;

namespace Application.Builder;

public static class TelegramKeyboardBuilder
{
    /// <summary>
    /// generate access token for EasyUi
    /// </summary>
    /// <returns></returns>
    public static InlineKeyboardMarkup BuildOpenShopKeyboard(string token)
    {
        var hostAddress = Environment.GetEnvironmentVariable("HOST_ADDRESS");
        
        return new InlineKeyboardMarkup([
            [
                InlineKeyboardButton.WithWebApp("Open Node Shop", $"{hostAddress}/EasyUi?token={token}"),
            ]
        ]);
    }

    public static InlineKeyboardMarkup BuildPaymentResult(long paymentId)
    {
        
        return new InlineKeyboardMarkup([
            [
                InlineKeyboardButton.WithCallbackData("❌ رد کردن", $"reject_payment:{paymentId}"),
                InlineKeyboardButton.WithCallbackData("✅ تایید", $"approve_payment:{paymentId}")
            ]
        ]);
    }
    
    public static InlineKeyboardMarkup BuildActivatePaymentButton(long targetUserId, PaymentMethod method)
    {
        var data = $"grant_payment:{method}:{targetUserId}";
        var text = method == PaymentMethod.CardToCard ? "✅ فعال‌سازی کارت به کارت" : "✅ فعال‌سازی پرداخت";
        return new InlineKeyboardMarkup([
            [InlineKeyboardButton.WithCallbackData(text, data)]
        ]);
    }
    
}