using Delsami.Plugin.Shipping.Core.Domain;
using Nop.Core;
using System;
using System.Threading.Tasks;

namespace Delsami.Plugin.Shipping.Core.Services;

public interface ILogisticsScheduleExceptionService
{
    Task InsertAsync(LogisticsScheduleException logisticsScheduleInfo);

    Task UpdateAsync(LogisticsScheduleException logisticsScheduleInfo);

    Task DeleteAsync(LogisticsScheduleException logisticsScheduleInfo);

    Task<LogisticsScheduleException> IsExistException(DateTime exceptiondate);

    Task<LogisticsScheduleException> GetById(int exceptionId);

    Task<IPagedList<LogisticsScheduleException>> GetAllAsync(int pageIndex = 0, int pageSize = int.MaxValue);



}
