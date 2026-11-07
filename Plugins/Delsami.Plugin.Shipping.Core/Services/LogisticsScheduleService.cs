using Delsami.Plugin.Misc.Core.Infrastructure.Cache;
using Delsami.Plugin.Shipping.Core.Domain;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Data;
using Nop.Services.Catalog;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Delsami.Plugin.Shipping.Core.Services
{
    public class LogisticsScheduleService : ILogisticsScheduleService
    {
        #region Fields

        private readonly IRepository<LogisticsSchedule> _logisticsScheduleRepo;
        private readonly ILogisticsScheduleExceptionService _logisticsScheduleExceptionService;
        private readonly IStaticCacheManager _staticCacheManager;

        #endregion

        #region Ctor

        public LogisticsScheduleService(IRepository<LogisticsSchedule> logisticsScheduleRepo, ILogisticsScheduleExceptionService logisticsScheduleExceptionService, IStaticCacheManager staticCacheManager)
        {
            _logisticsScheduleRepo = logisticsScheduleRepo;
            _logisticsScheduleExceptionService = logisticsScheduleExceptionService;
            _staticCacheManager = staticCacheManager;
        }

        #endregion

        #region Methods

        public virtual async Task<bool> IsExistDayOfWeek(DayOfWeek dayOfWeek, int? currentLogiscticsScheduleId = null)
        {
            var query = _logisticsScheduleRepo.Table.AsQueryable();

            if (currentLogiscticsScheduleId.HasValue)
                query = query.Where(w => w.Id != currentLogiscticsScheduleId.Value);

            return await query.AnyAsync(a => a.DayOfWeek == dayOfWeek);
        }

        public async Task<LogisticsSchedule> GetLogisticsScheduleAsync(int id)
        {
            return await _logisticsScheduleRepo.GetByIdAsync(id);
        }

        public virtual async Task InsertAsync(LogisticsSchedule logisticsScheduleInfo)
        {
            await _staticCacheManager.RemoveByPrefixAsync(DelsamiModelCacheDefaults.CheckLogisticIsActivePrefixCacheKey);

            await _logisticsScheduleRepo.InsertAsync(logisticsScheduleInfo);
        }

        public virtual async Task UpdateAsync(LogisticsSchedule logisticsScheduleInfo)
        {
            await _staticCacheManager.RemoveByPrefixAsync(DelsamiModelCacheDefaults.CheckLogisticIsActivePrefixCacheKey);

            await _logisticsScheduleRepo.UpdateAsync(logisticsScheduleInfo);
        }

        public virtual async Task DeleteAsync(LogisticsSchedule logisticsScheduleInfo)
        {
            await _staticCacheManager.RemoveByPrefixAsync(DelsamiModelCacheDefaults.CheckLogisticIsActivePrefixCacheKey);

            await _logisticsScheduleRepo.DeleteAsync(logisticsScheduleInfo);
        }

        public virtual async Task<IPagedList<LogisticsSchedule>> GetLogisticsSchedulesAsync(int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var logisticsSchedules = await _logisticsScheduleRepo.GetAllPagedAsync(query =>
            {
                //query = query.Where(w => !w.ValidTo.HasValue);

                return query.OrderBy(c => c.Id);

            }, pageIndex, pageSize);

            return logisticsSchedules;
        }

        public virtual async Task<IPagedList<LogisticsSchedule>> GetLogisticsSchedulesByNobPartsEventAsync(int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var logisticsSchedules = await _logisticsScheduleRepo.GetAllPagedAsync(query =>
            {
                return query.OrderBy(c => c.Id);

            }, pageIndex, pageSize);

            return logisticsSchedules;
        }

        public virtual async Task<bool> IsLogisticsActiveAsync(DateTime dateTime)
        {
            var exception = await _logisticsScheduleExceptionService.IsExistException(dateTime);

            if (exception != null)
            {
                var check = !exception.IsHoliday && exception.StartTime <= dateTime.TimeOfDay && exception.EndTime >= dateTime.TimeOfDay;
                return check;
            }

            return await _logisticsScheduleRepo.Table.AnyAsync(a => a.DayOfWeek == dateTime.DayOfWeek && a.StartTime <= dateTime.TimeOfDay && a.EndTime >= dateTime.TimeOfDay);
        }

        public virtual async Task<bool> IsLogisticsActiveForScheduleAsync(DateTime dateTime, TimeSpan startHour, TimeSpan endHour)
        {
            var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(DelsamiModelCacheDefaults.CheckLogisticIsActiveCacheKey, dateTime, startHour, endHour);

            var result = await _staticCacheManager.GetAsync(cacheKey, async () =>
            {
                TimeSpan workStart;
                TimeSpan workEnd;

                var exception = await _logisticsScheduleExceptionService.IsExistException(dateTime);

                if (exception != null)
                {
                    if (exception.IsHoliday)
                        return false;

                    workStart = exception.StartTime;
                    workEnd = exception.EndTime;
                }
                else
                {
                    var schedule = await _logisticsScheduleRepo.Table.FirstOrDefaultAsync(x => x.DayOfWeek == dateTime.DayOfWeek);

                    if (schedule == null)
                        return false;

                    workStart = schedule.StartTime;
                    workEnd = schedule.EndTime;
                }

                if (startHour > workStart && startHour < workEnd || endHour > workStart && endHour < workEnd)
                    return true;
                else
                    return false;
            });

            return result;
        }
        #endregion
    }
}
