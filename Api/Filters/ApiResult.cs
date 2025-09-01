﻿using Domain.Common;
using Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Api.Filters;

/// <summary>
/// Represents the result of an API operation without strongly-typed data.
/// </summary>
public record ApiResult(
    bool IsSuccess,
    ApiResultStatusCode StatusCode,
    string? JsonValidationMessage = null,
    string Message = "")
{
    /// <summary>
    /// Implicit conversion from OkResult to a successful ApiResult.
    /// </summary>
    public static implicit operator ApiResult(OkResult result)
        => new(true, ApiResultStatusCode.Success);

    /// <summary>
    /// Implicit conversion from JsonResult to a successful ApiResult.
    /// </summary>
    public static implicit operator ApiResult(JsonResult result)
        => new(true, ApiResultStatusCode.Success);

    /// <summary>
    /// Implicit conversion from BadRequestResult to a failed ApiResult.
    /// </summary>
    public static implicit operator ApiResult(BadRequestResult result)
        => new(false, ApiResultStatusCode.BadRequest);

    /// <summary>
    /// Implicit conversion from ContentResult to a successful ApiResult, using the result's content as JsonValidationMessage.
    /// </summary>
    public static implicit operator ApiResult(ContentResult result)
        => new(true, ApiResultStatusCode.Success, result.Content);

    /// <summary>
    /// Implicit conversion from NotFoundResult to a failed ApiResult.
    /// </summary>
    public static implicit operator ApiResult(NotFoundResult result)
        => new(false, ApiResultStatusCode.NotFound);

    /// <summary>
    /// Implicit conversion from AppException to a failed ApiResult, using the exception's ApiStatusCode and Message.
    /// </summary>
    public static implicit operator ApiResult(AppException result)
        => new(false, result.ApiStatusCode, Message: result.Message);

    /// <summary>
    /// Implicit conversion from BadRequestObjectResult to a failed ApiResult, parsing error messages if present.
    /// </summary>
    public static implicit operator ApiResult(BadRequestObjectResult result)
    {
        var message = result.Value?.ToString();
        if (result.Value is SerializableError errors)
        {
            var errorMessages = errors.SelectMany(p => (string[])p.Value).Distinct();
            message = string.Join(" | ", errorMessages);
        }

        return new(false, ApiResultStatusCode.BadRequest, message);
    }

    /// <summary>
    /// Implicit conversion from Exception to an ApiResult, mapping various exception types.
    /// </summary>
    /// <param name="ex">The exception to convert.</param>
    public static implicit operator ApiResult(Exception ex)
        => ex switch
        {
            DbUpdateException { InnerException: SqlException sqlEx } when (sqlEx.Number == 2627 || sqlEx.Number == 2601)
                => new(false, ApiResultStatusCode.BadRequest,
                    Message: $"Duplicate key error: {sqlEx.Message}"),

            ExistsException existsEx
                => new(false, existsEx.ApiStatusCode, existsEx.Message),

            BadRequestException badReq
                => new(false, badReq.ApiStatusCode, badReq.Message),

            NotFoundException notFound
                => new(false, notFound.ApiStatusCode, notFound.Message),

            AppException appEx
                => new(false, appEx.ApiStatusCode, appEx.Message),

            _ => new(false, ApiResultStatusCode.BadRequest,
                       "An unexpected error occurred.")
        };
}

public record ApiResult<TData>(
    bool IsSuccess,
    ApiResultStatusCode StatusCode,
    TData? Data,
    string? JsonValidationMessage = null,
    string? Message = "") : ApiResult(IsSuccess, StatusCode, JsonValidationMessage, Message) where TData : class
{
    /// <summary>
    /// Implicit conversion from data to a successful ApiResult.
    /// </summary>
    public static implicit operator ApiResult<TData>(TData data)
        => new(true, ApiResultStatusCode.Success, data);

    /// <summary>
    /// Implicit conversion from OkResult to a successful ApiResult.
    /// </summary>
    public static implicit operator ApiResult<TData>(OkResult result)
        => new(true, ApiResultStatusCode.Success, null);

    /// <summary>
    /// Implicit conversion from JsonResult to a successful ApiResult.
    /// </summary>
    public static implicit operator ApiResult<TData>(JsonResult result)
        => new(true, ApiResultStatusCode.Success, null);

    /// <summary>
    /// Implicit conversion from OkObjectResult to a successful ApiResult, using the result's value as Data.
    /// </summary>
    public static implicit operator ApiResult<TData>(OkObjectResult result)
        => new(true, ApiResultStatusCode.Success, (TData)result.Value!);

    /// <summary>
    /// Implicit conversion from BadRequestResult to a failed ApiResult.
    /// </summary>
    public static implicit operator ApiResult<TData>(BadRequestResult result)
        => new(false, ApiResultStatusCode.BadRequest, null);

    /// <summary>
    /// Implicit conversion from AppException to a failed ApiResult, using the exception's AdditionalData and Message.
    /// </summary>
    public static implicit operator ApiResult<TData>(AppException result)
        => new(false,
            result.ApiStatusCode,
            (TData)result.AdditionalData,
            result.AdditionalData.ToString(),
            result.Message);

    /// <summary>
    /// Implicit conversion from UnauthorizedResult to a failed ApiResult.
    /// </summary>
    public static implicit operator ApiResult<TData>(UnauthorizedResult result)
        => new(false, ApiResultStatusCode.BadRequest, null);

    /// <summary>
    /// Implicit conversion from ContentResult to a successful ApiResult, using the result's content as JsonValidationMessage.
    /// </summary>
    public static implicit operator ApiResult<TData>(ContentResult result)
        => new(true, ApiResultStatusCode.Success, null, result.Content);

    /// <summary>
    /// Implicit conversion from NotFoundResult to a failed ApiResult.
    /// </summary>
    public static implicit operator ApiResult<TData>(NotFoundResult result)
        => new(false, ApiResultStatusCode.NotFound, null);

    /// <summary>
    /// Implicit conversion from NotFoundObjectResult to a failed ApiResult, using the result's value as Data.
    /// </summary>
    public static implicit operator ApiResult<TData>(NotFoundObjectResult result)
        => new(false, ApiResultStatusCode.NotFound, (TData)result.Value!);

    /// <summary>
    /// Implicit conversion from BadRequestObjectResult to a failed ApiResult, parsing error messages if present.
    /// </summary>
    public static implicit operator ApiResult<TData>(BadRequestObjectResult result)
    {
        var message = result.Value?.ToString();
        if (result.Value is SerializableError errors)
        {
            var errorMessages = errors.SelectMany(p => (string[])p.Value).Distinct();
            message = string.Join(" | ", errorMessages);
        }

        return new(false, ApiResultStatusCode.BadRequest, null, message);
    }

    /// <summary>
    /// Implicit conversion from Exception to an ApiResult, mapping various exception types.
    /// </summary>
    /// <param name="ex">The exception to convert.</param>
    public static implicit operator ApiResult<TData>(Exception ex)
        => ex switch
        {
            DbUpdateException { InnerException: SqlException sqlEx } when (sqlEx.Number == 2627 || sqlEx.Number == 2601)
                => new(false, ApiResultStatusCode.BadRequest, null,
                    Message: $"Duplicate key error: {sqlEx.Message}"),

            ExistsException existsEx
                => new(false, existsEx.ApiStatusCode, null, existsEx.Message),

            BadRequestException badReq
                => new(false, badReq.ApiStatusCode, null, badReq.Message),

            NotFoundException notFound
                => new(false, notFound.ApiStatusCode, null, notFound.Message),

            AppException appEx
                => new(false, appEx.ApiStatusCode,
                    (TData?)appEx.AdditionalData,
                    appEx.Message),

            _ => new(false, ApiResultStatusCode.BadRequest, null,
                "An unexpected error occurred.")
        };
}