using Nop.Services.ScheduleTasks;
using System.Threading.Tasks;
using Delsami.Plugin.Misc.SMS.Servicecs;

namespace Delsami.Plugin.Misc.SMS.ScheduleTasks
{
    public class SendPendingQueuedSMS : IScheduleTask
    {
        private readonly ISMSSender _sMSSender;

        public SendPendingQueuedSMS(ISMSSender sMSSender)
        {
            _sMSSender = sMSSender;
        }

        public async Task ExecuteAsync()
        {
            var allPendingQueuedSMS = await _sMSSender.GetAllPendingQueuedSMS();

            foreach (var queuedSMS in allPendingQueuedSMS)
            {
               await _sMSSender.SendSMSAsync(queuedSMS);
            }
        }
    }
}
