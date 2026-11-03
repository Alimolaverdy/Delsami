using System.Collections.Generic;
using System.Threading.Tasks;
using Delsami.Plugin.Misc.SMS.Model;

namespace Delsami.Plugin.Misc.SMS.Servicecs
{
    public interface ISMSSender
    {
        Task<QueuedSMS> SendSMSAsync(QueuedSMS queuedSMS);
        Task<QueuedSMS> QueueSMSAsync(string receiverNumber, string body);
        Task<List<QueuedSMS>> GetAllPendingQueuedSMS();
    }
}
