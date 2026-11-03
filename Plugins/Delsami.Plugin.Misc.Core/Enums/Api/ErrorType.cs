using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delsami.Plugin.Misc.Core.Enums.Api
{
    public enum ErrorType
    {
        #region SMS

        [Description("خطای ناشناخته")]
        UnknownException = 1,

        [Description("متن پیام کوتاه خالی است")]
        SMSBodyIsEmpty,

        [Description("شماره موبایل خالی است")]
        ReceiverMobileNumberIsEmpty,

        [Description("شماره موبایل وارد شده معتبر نیست")]
        ReceiverMobileNumberInvalid,

        [Description("پسورد وارد شده اشتباه است")]
        PasswordInvalid,

        [Description("خطا در ارسال SMS")]
        ErrorInSendingSMS,

        [Description("حداکثر طول SMS 400 کاراکتر می تواند باشد")]
        MaximumSMSBodyLengthIs400
        #endregion
    }
}