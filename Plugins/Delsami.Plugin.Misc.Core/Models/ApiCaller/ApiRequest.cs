using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Delsami.Plugin.Misc.Core.Models.Resilience;

namespace Delsami.Plugin.Misc.Core.Models.ApiCaller
{
    public class ApiRequest
    {
        public string Url { get; set; } = string.Empty;
        public HttpMethod Method { get; set; } = HttpMethod.Get;
        public object Body { get; set; }
        public Dictionary<string, string> Headers { get; set; }
        public int? TimeoutSeconds { get; set; }
        public string BearerToken { get; set; }
        public string ContentType { get; set; } = "application/json";
        public ResilienceOptions ResilienceOptions { get; set; }
    }
}