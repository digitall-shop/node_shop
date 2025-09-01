using System.Net.Http.Json;
using System.Text.Json;
using Application.Extensiones;
using Application.Helpers;
using Application.Services.Interfaces;
using AutoMapper;
using Domain.Common;
using Domain.Contract;
using Domain.DTOs.User;
using Domain.Enumes.User;
using Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Application.Statics; // ← فقط این

using User = Domain.Entities.User;

namespace Application.Services.Implementations;

public class UserService(
    ILogger<IUserService> logger,
    IMapper mapper,
    IAsyncRepository<User, long> repository,
    IUserContextService userContextService,
    IHttpClientFactory httpClientFactory)
    : IUserService
{
    public async Task<UserDto> GetOrCreateUserAsync(UserCreateDto create)
    {
        var user = await repository.GetSingleAsync(u => u.Id == create.ChatId);
        if (user == null)
        {
            var newUser = mapper.Map<User>(create);
            newUser.Id = create.ChatId;
            newUser.Credit = 100000;
            newUser.PriceMultiplier = 1;
            if (string.IsNullOrWhiteSpace(newUser.UserName))
                newUser.UserName = newUser.Id.ToString();

            newUser.Password = PasswordHasher.HashPassword(newUser.UserName);

            await repository.AddEntity(newUser);
            await repository.SaveChangesAsync();
            
            newUser.RaiseRegistered();
            await repository.UpdateEntity(newUser);
            await repository.SaveChangesAsync();

            logger.UserCreated(newUser.Id, newUser.UserName);
            return mapper.Map<UserDto>(newUser);
        }

        logger.UserFound(user.Id, user.UserName);
        return mapper.Map<UserDto>(user);
    }

    public async Task<UserDto> GetUserAsync()
    {
        var user = await repository.GetSingleAsync(u => u.Id == userContextService.UserId);
        return mapper.Map<UserDto>(user);
    }

    public async Task<ProfileLink> GetProfilePhotoLinkAsync()
        => await GetProfilePhotoLinkAsync(userContextService.UserId);

    public async Task<ProfileLink> GetProfilePhotoLinkAsync(long userId)
    {
        var client = httpClientFactory.CreateClient();

        var user = await repository.GetSingleAsync(x => x.Id == userId)
                   ?? throw new NotFoundException("User not found");

        var photoResp = await client.GetFromJsonAsync<JsonDocument>(
            $"https://api.telegram.org/getUserProfilePhotos?user_id={user.Id}&limit=1");

        if (photoResp is null ||
            photoResp.RootElement.GetProperty("result").GetProperty("total_count").GetInt32() == 0)
            return new ProfileLink("");

        var fileId = photoResp.RootElement
            .GetProperty("result").GetProperty("photos")[0][0]
            .GetProperty("file_id")
            .GetString();

        var fileResp = await client.GetFromJsonAsync<JsonDocument>(
            $"https://api.telegram.org/getFile?file_id={fileId}");

        var filePath = fileResp?.RootElement.GetProperty("result")
            .GetProperty("file_path").GetString();

        var link = $"https://api.telegram.org/file/{filePath}";
        return new ProfileLink(link);
    }

    public async Task<UserDto> GetCurrentUserInfoAsync() => await GetUserAsync();

    public async Task<UserDto> GetUserByIdAsync(long userId)
    {
        var user = await repository.GetSingleAsync(x => x.Id == userId)
                   ?? throw new NotFoundException("user not found");
        return mapper.Map<UserDto>(user);
    }

    public async Task UpdateUserBalanceAsync(long userId, long amount)
    {
        var user = await repository.GetByIdAsync(userId)
                   ?? throw new NotFoundException("User not found");

        user.Balance += amount;
        await repository.UpdateEntity(user);
        await repository.SaveChangesAsync();

        logger.UserBalanceAdjusted(userId, amount, user.Balance);
    }

    public async Task<bool> UpgradeToSuperAdminAsync(long id)
    {
        var user = await repository.GetByIdAsync(id) ?? throw new NotFoundException("User not found");
        if (user.IsSuperAdmin) throw new ExistsException("user is already super admin");

        user.IsSuperAdmin = true;
        await repository.UpdateEntity(user);
        await repository.SaveChangesAsync();

        logger.UserPromotedToSuperAdmin(id, userContextService.UserId);
        return true;
    }

    public async Task<long> GetUserBalanceAsync(long userId)
    {
        if (userId != userContextService.UserId)
            throw new UnauthorizedAccessException("User does not have access to this operation");

        var balance = await repository.GetQuery()
            .Where(u => u.Id == userId)
            .Select(u => u.Balance)
            .FirstOrDefaultAsync();

        return balance;
    }

    public async Task<List<UserDto>> GetAllUsersAsync()
    {
        var users = await repository.GetAllAsync();
        return mapper.Map<List<UserDto>>(users);
    }

    public async Task<BasePaging<UserDto>> GetAllUsersPagingAsync(UserFilterDto filter)
    {
        var (filters, _) = filter.ToRepositoryParams();
        var data = await repository.GetPagedDataAsync(
            predicate: null,
            filters: filters,
            page: (filter.Skip / filter.Take) + 1,
            take: filter.Take);

        var items = mapper.Map<List<UserDto>>(data.Items);

        return new BasePaging<UserDto>
        {
            Items = items,
            Page = data.Page,
            Take = data.Take,
            TotalCount = data.TotalCount,
        };
    }

    public async Task<List<UserDto>> GetAllSuperAdminAsync()
    {
        var admins = await repository.GetAsync(u => u.IsSuperAdmin);
        return mapper.Map<List<UserDto>>(admins);
    }

    public async Task<UserDto> UpdateUserCreditAsync(UserUpdateCreditDto credit)
    {
        var user = await repository.GetByIdAsync(credit.UserId)
                   ?? throw new NotFoundException($"User with ID {credit.UserId} not found.");

        user.Credit += credit.AmountToChange;

        await repository.UpdateEntity(user);
        await repository.SaveChangesAsync();

        logger.UserCreditUpdated(credit.UserId, credit.AmountToChange, user.Credit);
        return mapper.Map<UserDto>(user);
    }

    public async Task<UserDto> UpdateUserPriceMultiplier(long userId, decimal multiplier)
    {
        var user = await repository.GetByIdAsync(userId)
                   ?? throw new NotFoundException($"User with ID {userId} not found.");

        user.PriceMultiplier = multiplier;

        await repository.UpdateEntity(user);
        await repository.SaveChangesAsync();

        logger.UserPriceMultiplierUpdated(userId, multiplier);
        return mapper.Map<UserDto>(user);
    }

    public async Task SetBlockedAsync(long userId, bool isBlocked)
    {
        var user = await repository.GetByIdAsync(userId)
                   ?? throw new NotFoundException("User not found");

        user.IsBlocked = isBlocked;

        await repository.UpdateEntity(user);
        await repository.SaveChangesAsync();

        logger.UserBlockedStatusSet(userId, isBlocked, userContextService.UserId);
    }

    public async Task<bool> IsBlockedAsync(long userId)
    {
        var user = await repository.GetByIdAsync(userId)
                   ?? throw new NotFoundException("User not found");
        return user.IsBlocked;
    }

    public async Task<UserDto> SetPaymentAccessAsync(long userId, PaymentMethodAccess access)
    {
        var user = await repository.GetByIdAsync(userId) ?? throw new NotFoundException("User not found");
        user.PaymentAccess = access;

        await repository.UpdateEntity(user);
        await repository.SaveChangesAsync();

        logger.UserPaymentAccessSet(userId, access.ToString());
        return mapper.Map<UserDto>(user);
    }

    public async Task<UserDto> GrantPaymentMethodAsync(long userId, PaymentMethodAccess method)
    {
        var user = await repository.GetByIdAsync(userId) ?? throw new NotFoundException("User not found");
        user.PaymentAccess |= method;

        await repository.UpdateEntity(user);
        await repository.SaveChangesAsync();

        logger.PaymentMethodGranted(userId, method.ToString(), userContextService.UserId);
        return mapper.Map<UserDto>(user);
    }

    public async Task<UserDto> RevokePaymentMethodAsync(long userId, PaymentMethodAccess method)
    {
        var user = await repository.GetByIdAsync(userId) ?? throw new NotFoundException("User not found");
        user.PaymentAccess &= ~method;

        await repository.UpdateEntity(user);
        await repository.SaveChangesAsync();

        // لاگ Payment-Topic برای لغو روش پرداخت
        logger.PaymentMethodRevoked(userId, method.ToString(), userContextService.UserId);
        return mapper.Map<UserDto>(user);
    }
}
