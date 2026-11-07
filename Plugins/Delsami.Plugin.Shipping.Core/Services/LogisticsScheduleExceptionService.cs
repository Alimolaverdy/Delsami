using Delsami.Plugin.Misc.Core.Infrastructure.Cache;
using Delsami.Plugin.Shipping.Core.Domain;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Data;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Delsami.Plugin.Shipping.Core.Services;

public class LogisticsScheduleExceptionService : ILogisticsScheduleExceptionService
{
    #region Fields

    private readonly IRepository<LogisticsScheduleException> _logisticsScheduleExceptionRepo;
    private readonly IStaticCacheManager _staticCacheManager;

    #endregion

    #region Ctor

    public LogisticsScheduleExceptionService(IRepository<LogisticsScheduleException> logisticsScheduleRepo, IStaticCacheManager staticCacheManager)
    {
        _logisticsScheduleExceptionRepo = logisticsScheduleRepo;
        _staticCacheManager = staticCacheManager;
    }

    #endregion

    #region Methods

    public virtual async Task<LogisticsScheduleException> IsExistException(DateTime exceptiondate)
    {
        return await _logisticsScheduleExceptionRepo.Table.FirstOrDefaultAsync(a => a.ExceptionDate.Date == exceptiondate.Date);
    }

    public virtual async Task InsertAsync(LogisticsScheduleException logisticsScheduleInfo)
    {
        await _staticCacheManager.RemoveByPrefixAsync(DelsamiModelCacheDefaults.CheckLogisticIsActivePrefixCacheKey);

        await _logisticsScheduleExceptionRepo.InsertAsync(logisticsScheduleInfo);
    }

    public virtual async Task UpdateAsync(LogisticsScheduleException logisticsScheduleInfo)
    {
        await _staticCacheManager.RemoveByPrefixAsync(DelsamiModelCacheDefaults.CheckLogisticIsActivePrefixCacheKey);

        await _logisticsScheduleExceptionRepo.UpdateAsync(logisticsScheduleInfo);
    }

    public virtual async Task DeleteAsync(LogisticsScheduleException logisticsScheduleInfo)
    {
        await _staticCacheManager.RemoveByPrefixAsync(DelsamiModelCacheDefaults.CheckLogisticIsActivePrefixCacheKey);

        await _logisticsScheduleExceptionRepo.DeleteAsync(logisticsScheduleInfo);
    }

    public async Task<LogisticsScheduleException> GetById(int exceptionId)
    {
        return await _logisticsScheduleExceptionRepo.Table.FirstOrDefaultAsync(a => a.Id == exceptionId);

    }

    public async Task<IPagedList<LogisticsScheduleException>> GetAllAsync(int pageIndex = 0, int pageSize = int.MaxValue)
    {
        var exceptions = await _logisticsScheduleExceptionRepo.GetAllPagedAsync(query =>
        {
            return query.OrderBy(c => c.Id);

        }, pageIndex, pageSize);

        return exceptions;
    }

    #endregion
}
