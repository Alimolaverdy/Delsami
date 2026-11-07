using System.Threading.Tasks;
using Delsami.Plugin.Shipping.Core.Areas.Admin.Models.Cartons;
using Delsami.Plugin.Shipping.Core.Domain;

namespace Delsami.Plugin.Shipping.Core.Areas.Admin.Factories
{
    public interface ICartonModelFactory
    {
        CartonSearchModel PrepareCartonSearchModel(CartonSearchModel searchModel);

        CartonModel PrepareCartonModel(CartonModel model, Carton carton);

        Task<CartonListModel> PrepareCartonListModelAsync(CartonSearchModel searchModel);
    }
}
