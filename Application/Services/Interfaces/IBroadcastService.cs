using Domain.DTOs.Broadcast;

namespace Application.Services.Interfaces;

public interface IBroadcastService
{
    /// <summary>
    /// this for sand messages to all users
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    Task<BroadcastResultDto> SendToAllAsync(BroadcastCreateDto message);

    /// <summary>
    /// this for a sand message to a user
    /// </summary>
    /// <param name="message"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<bool> SendToUserAsync(DirectMessageDto message ,long userId);

}