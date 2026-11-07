using Delsami.Plugin.Shipping.Core.Domain;
using Nop.Core;
using Nop.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Delsami.Plugin.Shipping.Core.Services
{
    public class CartonService : ICartonService
    {
        #region Fields

        private readonly IRepository<Carton> _cartonRepo;

        #endregion

        #region Ctor

        public CartonService(IRepository<Carton> cartonRepo)
        {
            _cartonRepo = cartonRepo;
        }

        #endregion

        #region Methods

        public async Task<Carton> GetCartonAsync(int id)
        {
            return await _cartonRepo.GetByIdAsync(id);
        }

        public virtual async Task InsertAsync(Carton cartonInfo)
        {
            await _cartonRepo.InsertAsync(cartonInfo);
        }

        public virtual async Task UpdateAsync(Carton cartonInfo)
        {
            await _cartonRepo.UpdateAsync(cartonInfo);
        }

        public virtual async Task DeleteAsync(Carton cartonInfo)
        {
            await _cartonRepo.DeleteAsync(cartonInfo);
        }

        public virtual async Task<IPagedList<Carton>> GetCartonsAsync(int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var cartons = await _cartonRepo.GetAllPagedAsync(query =>
            {
                return query.OrderBy(c => c.Id);

            }, pageIndex, pageSize);

            return cartons;
        }

        public virtual async Task<IPagedList<Carton>> GetCartonsByDelsamiEventAsync(int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var cartons = await _cartonRepo.GetAllPagedAsync(query =>
            {
                return query.OrderBy(c => c.Id);

            }, pageIndex, pageSize);

            return cartons;
        }

        #endregion
    }
}
