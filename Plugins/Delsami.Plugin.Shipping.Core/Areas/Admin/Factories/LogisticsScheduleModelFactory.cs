using Delsami.Plugin.Shipping.Core.Areas.Admin.Models.LogisticsSchedules;
using Delsami.Plugin.Shipping.Core.Domain;
using Delsami.Plugin.Shipping.Core.Services;
using Nop.Services.Catalog;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Framework.Models.Extensions;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Delsami.Plugin.Shipping.Core.Areas.Admin.Factories
{
    public class LogisticsScheduleModelFactory : ILogisticsScheduleModelFactory
    {
        #region Fields

        private readonly ISpecificationAttributeService _specificationAttributeService;
        private readonly ILocalizationService _localizationService;
        private readonly ILogisticsScheduleService _cartonService;

        #endregion

        #region Ctor

        public LogisticsScheduleModelFactory(ISpecificationAttributeService specificationAttributeService,
                                            ILocalizationService localizationService,
                                            ILogisticsScheduleService cartonService)
        {
            _specificationAttributeService = specificationAttributeService;
            _localizationService = localizationService;
            _cartonService = cartonService;
        }

        #endregion

        #region Methods

        public virtual LogisticsScheduleSearchModel PrepareLogisticsScheduleSearchModel(LogisticsScheduleSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        public virtual LogisticsScheduleModel PrepareLogisticsScheduleModel(LogisticsScheduleModel model, LogisticsSchedule logisticsSchedule)
        {
            if (model == null)
            {
                model = new LogisticsScheduleModel();

                if (logisticsSchedule != null)
                    model = logisticsSchedule.ToModel<LogisticsScheduleModel>();
            }

            foreach (var dayItem in Enum.GetValues(typeof(DayOfWeek)))
            {
                model.AvailableDays.Add(new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                    Text = dayItem.ToString(),
                    Value = ((int)dayItem).ToString(),
                    Selected = (int)model.DayOfWeek == (int)dayItem
                });
            }

            return model;
        }

        public virtual async Task<LogisticsScheduleListModel> PrepareLogisticsScheduleListModelAsync(LogisticsScheduleSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            var cartons = await _cartonService.GetLogisticsSchedulesAsync(pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare grid model
            var model = new LogisticsScheduleListModel().PrepareToGrid(searchModel, cartons, () =>
            {
                return cartons.Select(logisticsSchedule =>
                {
                    //fill in model values from the entity
                    var cartonModel = logisticsSchedule.ToModel<LogisticsScheduleModel>();
                    return cartonModel;
                });
            });

            return model;
        }

        #endregion
    }
}
