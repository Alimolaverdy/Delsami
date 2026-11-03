using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NopShop.Plugin.Misc.LoginWithMobile.Helpers
{
    public interface IFaraPayamakSmsHelper
    {
        Task<bool> SendSmsAsync(string templateMessage, string[] tokens, string customerMobile);
    }
}
