using System;
using Nop.Core;
using Delsami.Plugin.Misc.SMS.Infrastructure.Enums;

namespace Delsami.Plugin.Misc.SMS.Model
{
    public class QueuedSMS : BaseEntity
    {
        public QueuedSMS(string receiverNumber,string body)
        {
            ReceiverNumber = receiverNumber;
            SenderNumber = "100020085";
            Body = body;
            CreatedDateTime = DateTime.Now;
            SentTries = 0;
            Status = SMSStatus.Pending;
        }
        public string ReceiverNumber { get; private set; }
        public string SenderNumber { get; private set; }
        public string Body { get; private set; }
        public DateTime CreatedDateTime { get; private set; }
        public int SentTries { get; private set; }
        public DateTime? SentDateTime { get; private set; }
        public string? Result { get; private set; }
        public SMSStatus Status { get; private set; }



        public void IncreaseSentTries()
        {
            SentTries++;
        }

        public void SetSentDate()
        {
            SentDateTime = DateTime.Now;
        }

        public void SetResult(SMSStatus status, string result)
        {
            Status = status;
            Result = result;
        }

    }
}
