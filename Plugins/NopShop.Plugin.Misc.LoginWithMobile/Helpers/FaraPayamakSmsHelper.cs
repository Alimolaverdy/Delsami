//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Nop.Core.Domain.Logging;
//using Nop.Services.Logging;
//using RestSharp;

//namespace NopShop.Plugin.Misc.LoginWithMobile.Helpers
//{
//    public class FaraPayamakSmsHelper : IFaraPayamakSmsHelper
//    {
//        private readonly LoginWithMobileSetting _loginWithMobileSetting;
//        private readonly ILogger _logger;

//        public FaraPayamakSmsHelper(LoginWithMobileSetting loginWithMobileSetting,
//            ILogger logger)
//        {
//            _loginWithMobileSetting = loginWithMobileSetting;
//            _logger = logger;
//        }

//        public async Task<bool> SendSmsAsync(string templateMessage, string[] tokens, string customerMobile)
//        {
//            IRestResponse response = new RestResponse();
//            response.ResponseStatus = ResponseStatus.Error;

//            try
//            {
//                var username = _loginWithMobileSetting.FaraPayamakUsername;
//                var password = _loginWithMobileSetting.FaraPayamakPassword;
//                var from = _loginWithMobileSetting.FaraPayamakDedicatedLineNumber;
//                var text = string.Format(templateMessage, tokens);

//                var client = new RestClient("http://rest.payamak-panel.com/api/SendSMS/SendSMS");
//                var request = new RestRequest(Method.POST);
//                request.AddHeader("content-type", "application/x-www-form-urlencoded");
//                //request.AddHeader("postman-token", "fcddb5f4-dc58-c7d5-4bf9-9748710f8789");
//                request.AddHeader("cache-control", "no-cache");
//                request.AddParameter("application/x-www-form-urlencoded",
//                    "username=" + username + "&password=" + password + "&to=" + customerMobile +
//                    "&from=" + from + "&text=" + text + "&isflash=false", ParameterType.RequestBody);
//                response = client.ExecuteAsync(request).Result;

//                return response.ResponseStatus == ResponseStatus.Completed;
//            }
//            catch (Exception exception)
//            {
//                await _logger.InsertLogAsync(LogLevel.Error, exception.Message);
//                return response.ResponseStatus == ResponseStatus.Completed;
//            }
//        }
//    }
//}
