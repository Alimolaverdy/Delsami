using Delsami.Plugin.Shipping.Core.Areas.Admin.Models.LogisticsSchedulesException;
using System.Threading.Tasks;

namespace Delsami.Plugin.Shipping.Core.Areas.Admin.Factories;

public interface ILogisticsScheduleExceptionModelFactory
{
    Task<LogisticsScheduleExceptionListModel> PrepareLogisticsScheduleExceptionListModelAsync(LogisticsScheduleExceptionSearchModel searchModel);
}
