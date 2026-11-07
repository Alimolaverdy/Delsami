using Delsami.Plugin.Shipping.Core.Domain;
using Nop.Core;
using System.Threading.Tasks;

namespace Delsami.Plugin.Shipping.Core.Services
{
    public interface ICartonService
    {
        Task<Carton> GetCartonAsync(int id);

        Task InsertAsync(Carton cartonInfo);

        Task UpdateAsync(Carton cartonInfo);

        Task DeleteAsync(Carton cartonInfo);

        Task<IPagedList<Carton>> GetCartonsAsync(int pageIndex = 0, int pageSize = int.MaxValue);

        Task<IPagedList<Carton>> GetCartonsByDelsamiEventAsync(int pageIndex = 0, int pageSize = int.MaxValue);
    }
}
