using Api.Filters;
using Api.Securities;
using Application.Services.Interfaces;
using Domain.Common;
using Domain.DTOs.User;
using Domain.Enumes.User;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Tags("User Endpoint")]
[Route("api/user")]
public class UserController(IUserService service) : ApiBaseController
{
    [HttpGet]
    [EndpointName("get-all-users")]
    [EndpointSummary("get all users for admin")]
    [EndpointDescription("admin can get all users to management")]
    [SuperAdminAuthorize]
    [ProducesResponseType(typeof(ApiResult<BasePaging<UserDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResult<BasePaging<UserDto>>>> GetAllUsers([FromQuery] UserFilterDto filter)
    {
        var users = await service.GetAllUsersPagingAsync(filter);
        return Ok(users);
    }

    [HttpGet("user/{userId:long}")]
    [EndpointName("get-user-by-id")]
    [EndpointSummary("get user by id")]
    [EndpointDescription("when get user detail information")]
    [SuperAdminAuthorize]
    [ProducesResponseType(typeof(ApiResult<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserDto>> GetUserInformation([FromRoute] long userId)
    {
        var user = await service.GetUserByIdAsync(userId);
        return Ok(user);
    }

    [HttpGet("user")]
    [EndpointName("get-user")]
    [EndpointSummary("get user")]
    [EndpointDescription("when get user detail information")]
    [ProducesResponseType<ApiResult<UserDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiResult>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ApiResult>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserDto>> GetUserInformation()
    {
        var user = await service.GetCurrentUserInfoAsync();
        return Ok(user);
    }


    [HttpGet("profile-photo")]
    [EndpointDescription("give sun user profile telegram profile")]
    [EndpointSummary("telegram profile")]
    [EndpointName("profile")]
    [ProducesResponseType<ApiResult<ProfileLink>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiResult>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ApiResult>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProfileLink>> GetProfileUserLink()
    {
        var profileLink = await service.GetProfilePhotoLinkAsync();
        return Ok(profileLink);
    }

    [HttpGet("profile")]
    [EndpointDescription("give telegram profile")]
    [EndpointSummary("telegram profile")]
    [EndpointName("profile-link")]
    [ProducesResponseType<ApiResult<ProfileLink>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiResult>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ApiResult>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProfileLink>> GetProfileLink()
    {
        var profileLink = await service.GetProfilePhotoLinkAsync();
        return Ok(profileLink);
    }

    [HttpPatch("{userId:long}")]
    [EndpointName("upgrade")]
    [EndpointSummary("upgrade user")]
    [EndpointDescription("this for upgrade user to super admin")]
    [SuperAdminAuthorize]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResult>> Upgrade([FromRoute] long userId)
    {
        return Ok(await service.UpgradeToSuperAdminAsync(userId));
    }

    [HttpGet("balance/{userId:long}")]
    [EndpointName("get-balance")]
    [EndpointSummary("gat balance for user")]
    [EndpointDescription("gat latest balance for user untimely")]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResult>> GetBalance([FromRoute] long userId)
    {
        var balance = await service.GetUserBalanceAsync(userId);
        return Ok(balance);
    }

    [HttpPut("credit")]
    [EndpointName("update-user-credit")]
    [EndpointSummary("updates the credit limit for a specific user.")]
    [SuperAdminAuthorize]
    [ProducesResponseType(typeof(ApiResult<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<UserDto>> UpdateCredit([FromBody] UserUpdateCreditDto credit)
    {
        var newCredit = await service.UpdateUserCreditAsync(credit);
        return Ok(newCredit);
    }
    
    [HttpPut("multiplier/{userId:long}")]
    [EndpointName("update-user-multiplier")]
    [EndpointSummary("updates the multiplier for a specific user.")]
    [SuperAdminAuthorize]
    [ProducesResponseType(typeof(ApiResult<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<UserDto>> UpdateMultiplier([FromBody] decimal price,[FromRoute] long userId)
    {
        var multiplier = await service.UpdateUserPriceMultiplier(userId, price);
        return Ok(multiplier);
    }
    
    [HttpPost("{userId:long}/block")]
    [EndpointName("block-user")]
    [EndpointSummary("Block a user (prevent token issuance)")]
    [EndpointDescription("Sets User.IsBlocked = true. The user cannot get JWT afterward.")]
    [SuperAdminAuthorize]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> Block([FromRoute]long userId)
    {
        await service.SetBlockedAsync(userId, true);
        return Ok();
    }

    [HttpPost("{userId:long}/unblock")]
    [EndpointName("unblock-user")]
    [EndpointSummary("Unblock a user (allow token issuance)")]
    [EndpointDescription("Sets User.IsBlocked = false. The user can get JWT again.")]
    [SuperAdminAuthorize]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> Unblock([FromRoute]long userId)
    {
        await service.SetBlockedAsync(userId, false);
        return Ok();
    }
    
    [HttpPost("{userId:long}/payment-access")]
    [EndpointName("set-user-payment-access")]
    [EndpointSummary("Set user payment access mask")]
    [ProducesResponseType(typeof(ApiResult<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResult<UserDto>>> SetPaymentAccess(long userId, [FromQuery] PaymentMethodAccess access)
        => Ok(await service.SetPaymentAccessAsync(userId, access));

    [HttpPost("{userId:long}/payment-access/grant")]
    [EndpointName("grant-user-payment-method")]
    [EndpointSummary("Grant a payment method to user")]
    [ProducesResponseType(typeof(ApiResult<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResult<UserDto>>> Grant(long userId, [FromQuery] PaymentMethodAccess method)
        => Ok(await service.GrantPaymentMethodAsync(userId, method));

    [HttpPost("{userId:long}/payment-access/revoke")]
    [EndpointName("revoke-user-payment-method")]
    [EndpointSummary("Revoke a payment method from user")]
    [ProducesResponseType(typeof(ApiResult<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResult<UserDto>>> Revoke(long userId, [FromQuery] PaymentMethodAccess method)
        => Ok(await service.RevokePaymentMethodAsync(userId, method));

}