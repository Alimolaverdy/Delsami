using System;
using System.Collections.Generic;
using Delsami.Plugin.Misc.SMS.Infrastructure.Enums;
using Delsami.Plugin.Misc.SMS.Models.Infrastructure;

namespace Delsami.Plugin.Misc.SMS.Infrastructure.ExternalServices
{
    public interface IKavehNegarSMSSenderService
    {
        List<SendResult> Send(string sender, List<string> receptor, string message, MessageType? type = null, DateTime? date = null, List<string> localids = null);
    }
}
