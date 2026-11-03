using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Delsami.Plugin.Misc.SMS.Exceptions;
using Delsami.Plugin.Misc.SMS.Helpers;
using Delsami.Plugin.Misc.SMS.Infrastructure.Enums;
using Delsami.Plugin.Misc.SMS.Models.Infrastructure;

namespace Delsami.Plugin.Misc.SMS.Infrastructure.ExternalServices
{
    public class KavehNegarSMSSenderService : IKavehNegarSMSSenderService
    {

        #region Fields

        private readonly SMSSettings _smsSetting;

        #endregion

        #region Ctor

        public KavehNegarSMSSenderService(SMSSettings sMSSettings)
        {
            _smsSetting = sMSSettings;
        }

        #endregion

        #region Methods


        public List<SendResult> Send(
            string sender,
            List<string> receptor,
            string message,
            MessageType? type = null,
            DateTime? date = null,
            List<string> localids = null)
        {
            var path = GetApiPath("sms", "send", "json");

            var param = new Dictionary<string, object>()
    {
        { "sender", System.Web.HttpUtility.UrlEncode(sender) },
        { "receptor", System.Web.HttpUtility.UrlEncode(StringHelper.Join(",", receptor.ToArray())) },
        { "message", System.Web.HttpUtility.UrlEncode(message) },
    };

            if (type.HasValue)
                param.Add("type", (int)type.Value);

            if (date.HasValue)
                param.Add("date", NPDateTimeHelper.DateTimeToUnixTimestamp(date.Value));

            if (localids != null && localids.Count > 0)
                param.Add("localid", StringHelper.Join(",", localids.ToArray()));

            var responseBody = Execute(path, param);

            var response = JsonConvert.DeserializeObject<ReturnSend>(responseBody);

            if (response == null || response.entries == null)
                return new List<SendResult>();

            if (response.entries is Newtonsoft.Json.Linq.JArray entriesArray)
                return entriesArray.ToObject<List<SendResult>>() ?? new List<SendResult>();

            return JsonConvert.DeserializeObject<List<SendResult>>(
                JsonConvert.SerializeObject(response.entries))
                ?? new List<SendResult>();
        }


        #endregion

        #region Utilities

        private string GetApiPath(string _base, string method, string output)
        {
            return string.Format(_smsSetting.BaseUrl, _smsSetting.Apikey, _base, method, output);
        }

        private static string Execute(string path, Dictionary<string, object> _params)
        {

            string responseBody = "";
            string postdata = "";

            byte[] byteArray;
            if (_params != null)
            {
                postdata = _params.Keys.Aggregate(postdata,
                    (current, key) => current + string.Format("{0}={1}&", key, _params[key]));
                byteArray = Encoding.UTF8.GetBytes(postdata);
            }
            else
            {
                byteArray = new byte[0];
            }
            var webRequest = (HttpWebRequest)WebRequest.Create(path);
            webRequest.Method = "POST";
            webRequest.Timeout = -1;
            webRequest.ContentType = "application/x-www-form-urlencoded";
            webRequest.ContentLength = byteArray.Length;
            using (Stream webpageStream = webRequest.GetRequestStream())
            {
                webpageStream.Write(byteArray, 0, byteArray.Length);
            }
            HttpWebResponse webResponse;
            try
            {
                using (webResponse = (HttpWebResponse)webRequest.GetResponse())
                {
                    using (var reader = new StreamReader(webResponse.GetResponseStream()))
                    {
                        responseBody = reader.ReadToEnd();
                    }
                }
                JsonConvert.DeserializeObject<ReturnResult>(responseBody);
                return responseBody;
            }
            catch (WebException webException)
            {
                webResponse = (HttpWebResponse)webException.Response;
                using (var reader = new StreamReader(webResponse.GetResponseStream()))
                {
                    responseBody = reader.ReadToEnd();
                }
                try
                {
                    var result = JsonConvert.DeserializeObject<ReturnResult>(responseBody);
                    throw new ApiException(result.Return.Message, result.Return.Status);
                }
                catch (ApiException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    throw new HttpException(ex.Message, (int)((HttpWebResponse)webException.Response).StatusCode);
                }
            }
        }

        #endregion
    }
}