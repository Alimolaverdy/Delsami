using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Delsami.Plugin.Misc.Core.Enums.Api;
using Delsami.Plugin.Misc.Core.Extensions;

namespace Delsami.Plugin.Misc.Core.Models.Api
{
    public record ApiResult
    {
        public ApiResult(bool isSucceed, ErrorType? errorType = null, string message = "", string stackTrace = "")
        {
            IsSucceed = isSucceed;
            ErrorType = errorType;
            Message = message;
            StackTrace = stackTrace;
            if (errorType.HasValue)
                Message = errorType.Value.GetDescription();
        }

        public bool IsSucceed { get; private set; }
        public ErrorType? ErrorType { get; private set; }
        public string Message { get; private set; }
        public string StackTrace { get; private set; }
    }

    public record ApiResult<T> : ApiResult
    {
        public ApiResult(bool isSucceed, T result, ErrorType? errorType = null, string message = "", string stackTrace = "") : base(isSucceed, errorType)
        {
            Result = result;
        }

        public T Result { get; private set; }
    }
}