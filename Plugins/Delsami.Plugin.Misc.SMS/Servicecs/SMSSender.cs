using Nop.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Delsami.Plugin.Misc.SMS.Exceptions;
using Delsami.Plugin.Misc.SMS.Infrastructure.Enums;
using Delsami.Plugin.Misc.SMS.Infrastructure.ExternalServices;
using Delsami.Plugin.Misc.SMS.Model;

namespace Delsami.Plugin.Misc.SMS.Servicecs
{
    public class SMSSender : ISMSSender
    {
        private readonly IKavehNegarSMSSenderService _ismsSender;
        private readonly IRepository<QueuedSMS> _queuedSMSRepo;

        public SMSSender(IKavehNegarSMSSenderService ismsSender, IRepository<QueuedSMS> queuedSMSRepo)
        {
            _ismsSender = ismsSender;
            _queuedSMSRepo = queuedSMSRepo;
        }

        public async Task<QueuedSMS> SendSMSAsync(QueuedSMS queuedSMS)
        {
            if (queuedSMS == null)
                return null;

            try
            {
                var kaveNegarResult = _ismsSender.Send(
                    queuedSMS.SenderNumber,
                    new List<string> { queuedSMS.ReceiverNumber },
                    queuedSMS.Body);

                var result = kaveNegarResult.FirstOrDefault();

                if (result == null)
                {
                    throw new KavehNegarException(
                        "پاسخی از سرویس پیام کوتاه دریافت نشد.");
                }

                if (result.Status is 1 or 4 or 5)
                {
                    queuedSMS.SetSentDate();
                    queuedSMS.SetResult(
                        SMSStatus.Sent,
                        "پیام کوتاه با موفقیت ارسال شد");
                }
                else
                {
                    queuedSMS.SetResult(
                        SMSStatus.Failed,
                        result.StatusText);
                }
            }
            catch (Exception ex)
            {
                queuedSMS.SetResult(
                    SMSStatus.Failed,
                    ex.Message);

                throw new Exception("خطا در ارسال پیام کوتاه", ex);
            }
            finally
            {
                queuedSMS.IncreaseSentTries();
                await _queuedSMSRepo.UpdateAsync(queuedSMS);

            }

            return queuedSMS;
        }

        public async Task<List<QueuedSMS>> GetAllPendingQueuedSMS()
        {
            return await _queuedSMSRepo.Table
                .Where(x => x.Status != SMSStatus.Sent && x.SentDateTime == null)
                .ToListAsync();
        }

        public async Task<QueuedSMS> QueueSMSAsync(string receiverNumber, string body)
        {
            var queuedSMS = new QueuedSMS(receiverNumber,body);
            await _queuedSMSRepo.InsertAsync(queuedSMS);
            return queuedSMS;
        }
    }
}