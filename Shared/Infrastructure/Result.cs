using System.Runtime.CompilerServices;

namespace BusinessSolution.Shared.Infrastructure
{
    public sealed class Result<T>
    {
        public string Message { get; set; } = string.Empty;
        public bool Succeeded { get; set; }
        public int StatusCode { get; set; }
        public T? Data { get; set; }

        public static Task<Result<T>> SuccessAsync(string message, T? data = default)
        {
            return Task.FromResult(Success(message, data));
        }

        public static Result<T> Success(string message, T? data = default)
        {
            return new()
            {
                StatusCode = StatusCodes.Status200OK,
                Succeeded = true,
                Message = message,
                Data = data
            };
        }

        public static Task<Result<T>> ErrorAsync(string message,
            int errorCode = StatusCodes.Status500InternalServerError,
            Exception? ex = null,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            if (ex != null)
            {
                //ResultLogger.Logger?.Error(ex,
                //    """
                //UserId:{UserId}
                //Message:{Message}
                //Code:{Code}
                //Method:{MemberName}
                //File:{FilePath}
                //Line:{LineNumber}
                //""",
                //    ResultLogger.CurrentUserService?.UserId,
                //    message,
                //    errorCode,
                //    memberName,
                //    Path.GetFileName(filePath),
                //    lineNumber);
            }
            else
            {
                //ResultLogger.Logger?.Information(ex,
                //    """
                //UserId:{UserId}
                //Message:{Message}
                //Code:{Code}
                //Method:{MemberName}
                //File:{FilePath}
                //Line:{LineNumber}
                //""",
                //    ResultLogger.CurrentUserService?.UserId,
                //    message,
                //    errorCode,
                //    memberName,
                //    Path.GetFileName(filePath),
                //    lineNumber);
            }

            return Task.FromResult(Error(message, errorCode));
        }

        public static Task<Result<T>> RecordNotFoundAsync(string message, int errorCode = StatusCodes.Status404NotFound)
        {
            return Task.FromResult(Error(message, errorCode));
        }
        public static Task<Result<T>> RecordAlreadyExistAsync(string message, int errorCode = StatusCodes.Status409Conflict, T? data = default)
        {
            return Task.FromResult(Error(message, errorCode, data));
        }
        public static Task<Result<T>> BadRequestAsync(
        string message,
        int errorCode = StatusCodes.Status400BadRequest,
        Exception? ex = null,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0)
        {
            LogError(ex, message, errorCode, memberName, filePath, lineNumber);
            return Task.FromResult(Error(message, errorCode));
        }

        public static Result<T> BadRequest(
         string message,
         int errorCode = StatusCodes.Status400BadRequest,
         Exception? ex = null,
         [CallerMemberName] string memberName = "",
         [CallerFilePath] string filePath = "",
         [CallerLineNumber] int lineNumber = 0)
        {
            LogError(ex, message, errorCode, memberName, filePath, lineNumber);
            return Error(message, errorCode);
        }

        private static void LogError(
        Exception? ex,
        string message,
        int errorCode,
        string memberName,
        string filePath,
        int lineNumber)
        {

            if (ex != null)
            {
                //ResultLogger.Logger?.Error(ex,
                //    """
                //UserId:{UserId}
                //Message:{Message}
                //Code:{Code}
                //Method:{MemberName}
                //File:{FilePath}
                //Line:{LineNumber}
                //""",
                //        ResultLogger.CurrentUserService?.UserId,
                //        message,
                //        errorCode,
                //        memberName,
                //        Path.GetFileName(filePath),
                //        lineNumber);
            }
            else
            {
                //ResultLogger.Logger?.Information(ex,
                //    """
                //UserId:{UserId}
                //Message:{Message}
                //Code:{Code}
                //Method:{MemberName}
                //File:{FilePath}
                //Line:{LineNumber}
                //""",
                //        ResultLogger.CurrentUserService?.UserId,
                //        message,
                //        errorCode,
                //        memberName,
                //        Path.GetFileName(filePath),
                //        lineNumber);
            }
        }

        public static Result<T> Error(string message, int errorCode = StatusCodes.Status500InternalServerError, T? data = default)
        {
            return new()
            {
                Succeeded = false,
                Message = message,
                StatusCode = errorCode,
                Data = data
            };
        }
    }
}
