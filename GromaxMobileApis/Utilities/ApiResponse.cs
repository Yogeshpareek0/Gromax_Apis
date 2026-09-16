namespace GromaxMobileApis.Utilities
{
    public class ApiResponse<T>
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }


        //in case of check exist or not exists response will be
        //if (result == 1)
        //    return Ok(ApiResponse<string>.Success(default, "Exist"));
        //else
        //    return Ok(ApiResponse<string>.Success(default, "Not Exist"));
        //in case of check exist or not exists response will be
        //if (result == 1)
        //    return Ok(ApiResponse<string>.Success(default, "Update"));
        //else
        //    return Ok(ApiResponse<string>.Success(default, "Not Update"));
        public static ApiResponse<T> Success(T data, string message = "Success")
        {
            return new ApiResponse<T> { StatusCode = 200, Message = message, Data = data };
        }

        public static ApiResponse<T> Created(string message = "Success")
        {
            return new ApiResponse<T> { StatusCode = 200, Message = message };
        }

        public static ApiResponse<T> BadRequest(string message)
        {
            return new ApiResponse<T> { StatusCode = 400, Message = message, Data = default };
        }

        public static ApiResponse<T> Unauthorized(string message = "Unauthorized")
        {
            return new ApiResponse<T> { StatusCode = 401, Message = message, Data = default };
        }

        public static ApiResponse<T> Forbidden(string message = "Forbidden")
        {
            return new ApiResponse<T> { StatusCode = 403, Message = message, Data = default };
        }

        public static ApiResponse<T> NotFound(string message = "Not Found")
        {
            return new ApiResponse<T> { StatusCode = 404, Message = message, Data = default };
        }

        public static ApiResponse<T> Fail(string message, int statusCode = 500)
        {
            return new ApiResponse<T> { StatusCode = statusCode, Message = message, Data = default };
        }
    }

}
