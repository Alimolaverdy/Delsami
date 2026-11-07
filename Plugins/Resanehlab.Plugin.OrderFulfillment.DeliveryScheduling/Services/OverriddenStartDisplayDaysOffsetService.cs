using Nop.Core;
using Nop.Core.Events;
using Nop.Data;
using Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Domain;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Services
{
    public class OverriddenStartDisplayDaysOffsetService : IOverriddenStartDisplayDaysOffsetService
    {
        private readonly IRepository<OverriddenStartDisplayDaysOffset> _overriddenStartDisplayDaysOffsetRepository;
        private readonly IEventPublisher _eventPublisher;

        public OverriddenStartDisplayDaysOffsetService(
            IRepository<OverriddenStartDisplayDaysOffset> overriddenStartDisplayDaysOffsetRepository,
            IEventPublisher eventPublisher
            )
        {
            _overriddenStartDisplayDaysOffsetRepository = overriddenStartDisplayDaysOffsetRepository;
            _eventPublisher = eventPublisher;
        }

        public virtual async Task<OverriddenStartDisplayDaysOffset> GetOverriddenStartDisplayDaysOffsetByIdAsync(int overriddenStartDisplayDaysOffsetId)
        {
            if (overriddenStartDisplayDaysOffsetId == 0)
                return null;

            return await _overriddenStartDisplayDaysOffsetRepository.GetByIdAsync(overriddenStartDisplayDaysOffsetId);
        }

        public virtual async Task<IPagedList<OverriddenStartDisplayDaysOffset>> GetAllOverriddenStartDisplayDaysOffsetsAsync(int[] categoryIds = null, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = _overriddenStartDisplayDaysOffsetRepository.Table;

            if (categoryIds != null)
                query = query.Where(w => categoryIds.Contains(w.CategoryId));

            query = query.OrderByDescending(t => t.Id);

            return await query.ToPagedListAsync(pageIndex, pageSize);
        }

        public virtual async Task InsertOverriddenStartDisplayDaysOffsetAsync(OverriddenStartDisplayDaysOffset overriddenStartDisplayDaysOffset)
        {
            if (overriddenStartDisplayDaysOffset == null)
                throw new ArgumentNullException(nameof(overriddenStartDisplayDaysOffset));

            await _overriddenStartDisplayDaysOffsetRepository.InsertAsync(overriddenStartDisplayDaysOffset);

            //event notification
            await _eventPublisher.EntityInsertedAsync(overriddenStartDisplayDaysOffset);
        }

        public virtual async Task UpdateOverriddenStartDisplayDaysOffsetAsync(OverriddenStartDisplayDaysOffset overriddenStartDisplayDaysOffset)
        {
            if (overriddenStartDisplayDaysOffset == null)
                throw new ArgumentNullException(nameof(overriddenStartDisplayDaysOffset));

            await _overriddenStartDisplayDaysOffsetRepository.UpdateAsync(overriddenStartDisplayDaysOffset);

            //event notification
            await _eventPublisher.EntityUpdatedAsync(overriddenStartDisplayDaysOffset);
        }

        public virtual async Task DeleteOverriddenStartDisplayDaysOffsetAsync(OverriddenStartDisplayDaysOffset overriddenStartDisplayDaysOffset)
        {
            if (overriddenStartDisplayDaysOffset == null)
                throw new ArgumentNullException(nameof(overriddenStartDisplayDaysOffset));

            await _overriddenStartDisplayDaysOffsetRepository.DeleteAsync(overriddenStartDisplayDaysOffset);

            //event notification
            await _eventPublisher.EntityDeletedAsync(overriddenStartDisplayDaysOffset);
        }
    }
}
