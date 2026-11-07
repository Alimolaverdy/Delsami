using Nop.Core;
using Nop.Data;
using Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Domain;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Services
{
    public class ExceptionDateService : IExceptionDateService
    {
        private readonly IRepository<ExceptionDate> _exceptionDateRepo;

        public ExceptionDateService(IRepository<ExceptionDate> exceptionDateRepo)
        {
            _exceptionDateRepo = exceptionDateRepo;
        }

        public virtual async Task DeleteExceptionDateAsync(ExceptionDate exceptionDate)
        {
            exceptionDate.Deleted = true;

            await _exceptionDateRepo.UpdateAsync(exceptionDate);
        }

        public virtual async Task<ExceptionDate> GetExceptionDateAsync(DateTime date, int storeId)
        {
            return await _exceptionDateRepo.Table.Where(w => !w.Deleted && w.DateOnUtc == date && w.StoreId == storeId).FirstOrDefaultAsync();
        }

        public virtual async Task<ExceptionDate> GetExceptionDateByIdAsync(int id)
        {
            return await _exceptionDateRepo.GetByIdAsync(id);
        }

        public virtual async Task<IPagedList<ExceptionDate>> GetExceptionDatesAsync(int? storeId = null, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = _exceptionDateRepo.Table;

            if (storeId.HasValue)
                query = query.Where(w => w.StoreId == storeId.Value);

            query = query.Where(w => !w.Deleted);

            return await query.ToPagedListAsync(pageIndex, pageSize);
        }

        public virtual async Task InsertExceptionDateAsync(ExceptionDate exceptionDate)
        {
            await _exceptionDateRepo.InsertAsync(exceptionDate);
        }

        public virtual async Task UpdateExceptionDateAsync(ExceptionDate exceptionDate)
        {
            await _exceptionDateRepo.UpdateAsync(exceptionDate);
        }
    }
}
