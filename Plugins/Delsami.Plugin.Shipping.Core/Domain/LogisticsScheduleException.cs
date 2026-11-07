using Nop.Core;
using System;

namespace Delsami.Plugin.Shipping.Core.Domain
{
    public class LogisticsScheduleException : BaseEntity
    {
        public DateTime ExceptionDate { get; set; }

        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }

        public bool IsHoliday { get; set; }
    }
}
