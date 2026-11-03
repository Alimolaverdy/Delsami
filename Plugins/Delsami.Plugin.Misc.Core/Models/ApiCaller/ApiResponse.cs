using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Delsami.Plugin.Misc.Core.Models.ApiCaller
{
    public class ApiResponse<T>
    {
        public bool IsSuccess { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public T Data { get; set; }
        public string RawResponse { get; set; }
        public string ErrorMessage { get; set; }
    }
}