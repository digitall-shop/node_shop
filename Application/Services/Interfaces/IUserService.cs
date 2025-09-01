using Domain.Common;
using Domain.DTOs.User;
using Domain.Enumes.User;


namespace Application.Services.Interfaces;

public interface IUserService
{
    /// <summary>
    /// this for get or create user that start our bot
    /// </summary>
    /// <param name="create"></param>
    /// <returns></returns>
    Task<UserDto> GetOrCreateUserAsync(UserCreateDto create);

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    Task<UserDto> GetUserAsync();

    /// <summary>
    /// get user profile link async
    /// </summary>
    /// <returns></returns>
    Task<ProfileLink> GetProfilePhotoLinkAsync();

    /// <summary>
    /// get user profile link async
    /// </summary>
    /// <param name="userId">for child user id</param>
    /// <returns></returns>
    Task<ProfileLink> GetProfilePhotoLinkAsync(long userId);

    /// <summary>
    /// get user information by http context accessor
    /// </summary>
    /// <returns>INSTANCE UserDto</returns>
    Task<UserDto> GetCurrentUserInfoAsync();

    /// <summary>
    /// this for get user by ID
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<UserDto> GetUserByIdAsync(long userId);

    /// <summary>
    /// this for update user balance after any transaction  
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="amount"></param>
    /// <returns></returns>
    Task UpdateUserBalanceAsync(long userId, long amount);

    /// <summary>
    /// this for upgrade user to super admin
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<bool> UpgradeToSuperAdminAsync(long id);

    /// <summary>
    /// this for get user balance 
    /// </summary>
    /// <returns></returns>
    Task<long> GetUserBalanceAsync(long userId);

    /// <summary>
    /// this for get all users for show to admin 
    /// </summary>
    /// <returns></returns>
    Task<List<UserDto>> GetAllUsersAsync();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="filter"></param>
    /// <returns></returns>
    Task<BasePaging<UserDto>> GetAllUsersPagingAsync(UserFilterDto filter);

    /// <summary>
    /// this for get all super admins
    /// </summary>
    /// <returns></returns>
    Task<List<UserDto>> GetAllSuperAdminAsync();

    /// <summary>
    /// this for update user credit 
    /// </summary>
    /// <param name="credit"></param>
    /// <returns></returns>
    Task<UserDto> UpdateUserCreditAsync(UserUpdateCreditDto credit);

    /// <summary>
    /// this for update PriceMultiplier of user
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="multiplier"></param>
    /// <returns></returns>
    Task<UserDto> UpdateUserPriceMultiplier(long userId, decimal multiplier);

    /// <summary>
    /// this for block and unblock user
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="isBlocked"></param>
    /// <returns></returns>
    Task SetBlockedAsync(long userId, bool isBlocked);

    /// <summary>
    /// this for check state user block/unblock
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<bool> IsBlockedAsync(long userId);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="access"></param>
    /// <returns></returns>
    Task<UserDto> SetPaymentAccessAsync(long userId, PaymentMethodAccess access);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="method"></param>
    /// <returns></returns>
    Task<UserDto> GrantPaymentMethodAsync(long userId, PaymentMethodAccess method);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="method"></param>
    /// <returns></returns>
    Task<UserDto> RevokePaymentMethodAsync(long userId, PaymentMethodAccess method);
}