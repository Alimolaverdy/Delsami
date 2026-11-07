using Delsami.Plugin.Shipping.Core.Areas.Admin.Models.LogisticsSchedulesException;
using Delsami.Plugin.Shipping.Core.Services;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Framework.Models.Extensions;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Delsami.Plugin.Shipping.Core.Areas.Admin.Factories;

public class LogisticsScheduleExceptionModelFactory : ILogisticsScheduleExceptionModelFactory
{
    #region Fields

    private readonly ILogisticsScheduleExceptionService _logisticsScheduleExceptionService;

    #endregion

    #region Ctor

    public LogisticsScheduleExceptionModelFactory(ILogisticsScheduleExceptionService logisticsScheduleExceptionService)
    {
        _logisticsScheduleExceptionService = logisticsScheduleExceptionService;
    }

    #endregion

    #region Methods

    public virtual LogisticsScheduleExceptionSearchModel PrepareLogisticsScheduleSearchModel(LogisticsScheduleExceptionSearchModel searchModel)
    {
        if (searchModel == null)
            throw new ArgumentNullException(nameof(searchModel));

        //prepare page parameters
        searchModel.SetGridPageSize();

        return searchModel;
    }

    public virtual async Task<LogisticsScheduleExceptionListModel> PrepareLogisticsScheduleExceptionListModelAsync(LogisticsScheduleExceptionSearchModel searchModel)
    {
        if (searchModel == null)
            throw new ArgumentNullException(nameof(searchModel));

        var exceptions = await _logisticsScheduleExceptionService.GetAllAsync(pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

        //prepare grid model
        var model = new LogisticsScheduleExceptionListModel().PrepareToGrid(searchModel, exceptions, () =>
        {
            return exceptions.Select(logisticsSchedule =>
            {
                //fill in model values from the entity
                var cartonModel = logisticsSchedule.ToModel<LogisticsScheduleExceptionModel>();
                return cartonModel;
            });
        });

        return model;
    }

    #endregion
}
