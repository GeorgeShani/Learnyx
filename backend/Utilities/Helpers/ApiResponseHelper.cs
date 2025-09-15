using learnyx.Models.Common;
using learnyx.Models.Responses;
using Microsoft.AspNetCore.Mvc;

namespace learnyx.Utilities.Helpers;

public static class ApiResponseHelper
{
    public static ActionResult<ApiResponse<T>> Ok<T>(T data, string message = "Success")
    {
        var response = new ApiResponse<T>(true, message, data, null, 200);
        return new OkObjectResult(response);
    }

    public static ActionResult<ApiResponse> Ok(string message = "Success")
    {
        var response = new ApiResponse(true, message, null, null, 200);
        return new OkObjectResult(response);
    }

    public static ActionResult<ApiResponse<T>> Created<T>(T data, string message = "Resource created successfully")
    {
        var response = new ApiResponse<T>(true, message, data, null, 201);
        return new ObjectResult(response) { StatusCode = 201 };
    }

    public static ActionResult<ApiResponse> Created(string message = "Resource created successfully")
    {
        var response = new ApiResponse(true, message, null, null, 201);
        return new ObjectResult(response) { StatusCode = 201 };
    }

    public static ActionResult<ApiResponse> NoContent(string message = "No content")
    {
        var response = new ApiResponse(true, message, null, null, 204);
        return new ObjectResult(response) { StatusCode = 204 };
    }

    // Client error responses
    public static ActionResult<ApiResponse> BadRequest(string message = "Bad request", object errors = null)
    {
        var response = new ApiResponse(false, message, null, errors, 400);
        return new BadRequestObjectResult(response);
    }

    public static ActionResult<ApiResponse> BadRequest(object errors, string message = "Validation failed")
    {
        var response = new ApiResponse(false, message, null, errors, 400);
        return new BadRequestObjectResult(response);
    }

    public static ActionResult<ApiResponse> Unauthorized(string message = "Unauthorized access")
    {
        var response = new ApiResponse(false, message, null, null, 401);
        return new UnauthorizedObjectResult(response);
    }

    public static ActionResult<ApiResponse> Forbidden(string message = "Access forbidden")
    {
        var response = new ApiResponse(false, message, null, null, 403);
        return new ObjectResult(response) { StatusCode = 403 };
    }

    public static ActionResult<ApiResponse> NotFound(string message = "Resource not found")
    {
        var response = new ApiResponse(false, message, null, null, 404);
        return new NotFoundObjectResult(response);
    }

    public static ActionResult<ApiResponse> Conflict(string message = "Conflict occurred", object errors = null)
    {
        var response = new ApiResponse(false, message, null, errors, 409);
        return new ConflictObjectResult(response);
    }

    public static ActionResult<ApiResponse> UnprocessableEntity(string message = "Unprocessable entity", object errors = null)
    {
        var response = new ApiResponse(false, message, null, errors, 422);
        return new UnprocessableEntityObjectResult(response);
    }

    // Server error responses
    public static ActionResult<ApiResponse> InternalServerError(string message = "Internal server error", object errors = null)
    {
        var response = new ApiResponse(false, message, null, errors, 500);
        return new ObjectResult(response) { StatusCode = 500 };
    }

    public static ActionResult<ApiResponse> NotImplemented(string message = "Not implemented")
    {
        var response = new ApiResponse(false, message, null, null, 501);
        return new ObjectResult(response) { StatusCode = 501 };
    }

    public static ActionResult<ApiResponse> ServiceUnavailable(string message = "Service unavailable")
    {
        var response = new ApiResponse(false, message, null, null, 503);
        return new ObjectResult(response) { StatusCode = 503 };
    }

    // Custom status code response
    public static ActionResult<ApiResponse<T>> Custom<T>(int statusCode, bool success, string message, T data = default, object errors = null)
    {
        var response = new ApiResponse<T>(success, message, data, errors, statusCode);
        return new ObjectResult(response) { StatusCode = statusCode };
    }

    public static ActionResult<ApiResponse> Custom(int statusCode, bool success, string message, object errors = null)
    {
        var response = new ApiResponse(success, message, null, errors, statusCode);
        return new ObjectResult(response) { StatusCode = statusCode };
    }

    // Paginated response
    public static ActionResult<ApiResponse<PagedResult<T>>> PagedOk<T>(IEnumerable<T> data, int totalCount, int pageNumber, int pageSize, string message = "Success")
    {
        var pagedResult = new PagedResult<T>
        {
            Data = data,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalPages = (int)Math.Ceiling((double)totalCount / pageSize),
            HasNextPage = pageNumber < Math.Ceiling((double)totalCount / pageSize),
            HasPreviousPage = pageNumber > 1
        };

        var response = new ApiResponse<PagedResult<T>>(true, message, pagedResult, null, 200);
        return new OkObjectResult(response);
    }
}