using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.Collections.Generic;

namespace Delsami.Plugin.Shipping.Core.Areas.Admin.Models.LogisticsSchedules
{
    public record LogisticsScheduleModel : BaseNopEntityModel
    {
        public LogisticsScheduleModel()
        {
            AvailableDays = new List<SelectListItem>();
        }

        [NopResourceDisplayName("Delsami.Plugin.Misc.ShippingCore.Admin.LogisticsSchedule.StartTime")]
        public string StartTimeStr { get; set; }

        [NopResourceDisplayName("Delsami.Plugin.Misc.ShippingCore.Admin.LogisticsSchedule.EndTime")]
        public string EndTimeStr { get; set; }


        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }

        [NopResourceDisplayName("Delsami.Plugin.Misc.ShippingCore.Admin.LogisticsSchedule.ValidFrom")]
        public DateTime? ValidFrom { get; set; }

        [NopResourceDisplayName("Delsami.Plugin.Misc.ShippingCore.Admin.LogisticsSchedule.ValidTo")]
        public DateTime? ValidTo { get; set; }

        [NopResourceDisplayName("Delsami.Plugin.Misc.ShippingCore.Admin.LogisticsSchedule.DayOfWeek")]
        public DayOfWeek DayOfWeek { get; set; }

        [NopResourceDisplayName("Delsami.Plugin.Misc.ShippingCore.Admin.LogisticsSchedule.DayOfWeekStr")]
        public string DayOfWeekStr
        {
            get
            {
                return DayOfWeek.ToString();
            }
        }

        public IList<SelectListItem> AvailableDays { get; set; }

    }
}
