using Delsami.Plugin.Shipping.Core.Areas.Admin.Models.LogisticsSchedules;
using Delsami.Plugin.Shipping.Core.Domain;
using System.Threading.Tasks;

namespace Delsami.Plugin.Shipping.Core.Areas.Admin.Factories
{
    public interface ILogisticsScheduleModelFactory
    {
        LogisticsScheduleSearchModel PrepareLogisticsScheduleSearchModel(LogisticsScheduleSearchModel searchModel);

        LogisticsScheduleModel PrepareLogisticsScheduleModel(LogisticsScheduleModel model, LogisticsSchedule carton);

        Task<LogisticsScheduleListModel> PrepareLogisticsScheduleListModelAsync(LogisticsScheduleSearchModel searchModel);
    }
}
