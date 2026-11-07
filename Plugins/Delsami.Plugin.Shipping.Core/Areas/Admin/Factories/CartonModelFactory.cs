using Nop.Services.Catalog;
using Nop.Services.Localization;
using System.Threading.Tasks;
using System;
using Nop.Web.Framework.Models.Extensions;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using System.Linq;
using Delsami.Plugin.Shipping.Core.Areas.Admin.Models.Cartons;
using Delsami.Plugin.Shipping.Core.Services;
using Delsami.Plugin.Shipping.Core.Domain;

namespace Delsami.Plugin.Shipping.Core.Areas.Admin.Factories
{
    public class CartonModelFactory: ICartonModelFactory
    {
        #region Fields

        private readonly ISpecificationAttributeService _specificationAttributeService;
        private readonly ILocalizationService _localizationService;
        private readonly ICartonService _cartonService;

        #endregion

        #region Ctor

        public CartonModelFactory(ISpecificationAttributeService specificationAttributeService,
                                  ILocalizationService localizationService,
                                  ICartonService cartonService)
        {
            _specificationAttributeService = specificationAttributeService;
            _localizationService = localizationService;
            _cartonService = cartonService;
        }

        #endregion

        #region Methods

        public virtual CartonSearchModel PrepareCartonSearchModel(CartonSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        public virtual CartonModel PrepareCartonModel(CartonModel model, Carton carton)
        {
            if (model == null)
            {
                model = new CartonModel();

                if (carton != null)
                    model = carton.ToModel<CartonModel>();
            }

            return model;
        }

        public virtual async Task<CartonListModel> PrepareCartonListModelAsync(CartonSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            var cartons = await _cartonService.GetCartonsAsync(pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare grid model
            var model = new CartonListModel().PrepareToGrid(searchModel, cartons, () =>
            {
                return cartons.Select( carton =>
                {
                    //fill in model values from the entity
                    var cartonModel = carton.ToModel<CartonModel>();
                    return cartonModel;
                });
            });

            return model;
        }

        #endregion
    }
}
