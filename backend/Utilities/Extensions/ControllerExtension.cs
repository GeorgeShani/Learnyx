using learnyx.Models.Responses;
using learnyx.Utilities.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace learnyx.Utilities.Extensions;

public static class ControllerExtension
{
    public static ActionResult<ApiResponse<T>> ApiOk<T>(this ControllerBase controller, T data, string message = "Success")
    {
        return ApiResponseHelper.Ok(data, message);
    }

    public static ActionResult<ApiResponse> ApiOk(this ControllerBase controller, string message = "Success")
    {
        return ApiResponseHelper.Ok(message);
    }

    public static ActionResult<ApiResponse<T>> ApiCreated<T>(this ControllerBase controller, T data, string message = "Resource created successfully")
    {
        return ApiResponseHelper.Created(data, message);
    }

    public static ActionResult<ApiResponse> ApiCreated(this ControllerBase controller, string message = "Resource created successfully")
    {
        return ApiResponseHelper.Created(message);
    }

    public static ActionResult<ApiResponse> ApiBadRequest(this ControllerBase controller, string message = "Bad request", object errors = null)
    {
        return ApiResponseHelper.BadRequest(message, errors);
    }

    public static ActionResult<ApiResponse> ApiNotFound(this ControllerBase controller, string message = "Resource not found")
    {
        return ApiResponseHelper.NotFound(message);
    }

    public static ActionResult<ApiResponse> ApiUnauthorized(this ControllerBase controller, string message = "Unauthorized access")
    {
        return ApiResponseHelper.Unauthorized(message);
    }

    public static ActionResult<ApiResponse> ApiForbidden(this ControllerBase controller, string message = "Access forbidden")
    {
        return ApiResponseHelper.Forbidden(message);
    }

    public static ActionResult<ApiResponse> ApiInternalServerError(this ControllerBase controller, string message = "Internal server error", object errors = null)
    {
        return ApiResponseHelper.InternalServerError(message, errors);
    }
}