using Nop.Core;
using Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Domain;
using System;
using System.Threading.Tasks;

namespace Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Services
{
    public interface IExceptionDateService
    {
        Task<IPagedList<ExceptionDate>> GetExceptionDatesAsync(int? storeId = null, int pageIndex = 0, int pageSize = int.MaxValue);

        Task UpdateExceptionDateAsync(ExceptionDate exceptionDate);

        Task DeleteExceptionDateAsync(ExceptionDate exceptionDate);

        Task<ExceptionDate> GetExceptionDateByIdAsync(int id);

        Task InsertExceptionDateAsync(ExceptionDate exceptionDate);

        Task<ExceptionDate> GetExceptionDateAsync(DateTime date, int storeId);
    }
}
