using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.ComponentModel.DataAnnotations;

namespace Delsami.Plugin.Shipping.Core.Areas.Admin.Models.LogisticsSchedulesException;

public record LogisticsScheduleExceptionModel : BaseNopEntityModel
{
    [NopResourceDisplayName("Delsami.Plugin.Misc.ShippingCore.Admin.LogisticsScheduleExceptionModel.StartTime")]
    public string StartTimeStr { get; set; }

    [NopResourceDisplayName("Delsami.Plugin.Misc.ShippingCore.Admin.LogisticsScheduleExceptionModel.EndTime")]
    public string EndTimeStr { get; set; }

    [NopResourceDisplayName("Delsami.Plugin.Misc.ShippingCore.Admin.LogisticsScheduleExceptionModel.IsHoliday")]
    public bool IsHoliday { get; set; }


    public TimeSpan StartTime { get; set; }

    public TimeSpan EndTime { get; set; }

    [UIHint("Date")]
    public DateTime ExceptionDate { get; set; }=DateTime.Now;


    [NopResourceDisplayName("Delsami.Plugin.Misc.ShippingCore.Admin.LogisticsScheduleExceptionModel.ExceptionDateStr")]
    public string ExceptionDateStr
    {
        get
        {
            return ExceptionDate.ToString("yyyy/MM/dd");
        }
    }

}
