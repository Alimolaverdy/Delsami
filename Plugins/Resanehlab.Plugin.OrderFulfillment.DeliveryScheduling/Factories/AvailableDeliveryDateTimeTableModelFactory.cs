using Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Areas.Admin.Models;
using Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Extensions;
using Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Services;
using Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Settings;
using Delsami.Plugin.Shipping.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Factories
{
    public partial class AvailableDeliveryDateTimeTableModelFactory : IAvailableDeliveryDateTimeTableModelFactory
    {
        #region Fields

        private readonly IAvailableDeliveryDateTimeRangeService _availableDeliveryDateTimeRangeService;
        private readonly IOrderShippingMethodCapacityMappingService _orderShippingMethodCapacityMappingService;
        private readonly ILogisticsScheduleService  _logisticsScheduleService;

        private readonly DeliverySchedulingSetting _deliverySchedulingSetting;

        #endregion

        #region Ctor

        public AvailableDeliveryDateTimeTableModelFactory(
            IAvailableDeliveryDateTimeRangeService availableDeliveryDateTimeRangeService,
            IOrderShippingMethodCapacityMappingService orderShippingMethodCapacityMappingService,
            DeliverySchedulingSetting deliverySchedulingSetting,
            ILogisticsScheduleService logisticsScheduleService)
        {
            _availableDeliveryDateTimeRangeService = availableDeliveryDateTimeRangeService;
            _orderShippingMethodCapacityMappingService = orderShippingMethodCapacityMappingService;
            _deliverySchedulingSetting = deliverySchedulingSetting;
            _logisticsScheduleService = logisticsScheduleService;
        }

        #endregion

        #region Methods

        public virtual async Task<List<AvailableDeliveryDateTimesTableModel>> PrepareAvailableDeliveryDateTimesTableAsync(int shippingMethodId,
                                                                                                                          int storeId,
                                                                                                                          int languageId,
                                                                                                                          DateTime firstSelectableDay)
        {
            var model = new List<AvailableDeliveryDateTimesTableModel>();

            var availableDeliveryDateTimes = await _availableDeliveryDateTimeRangeService.LoadAvailableDeliveryDateTimesAsync(shippingMethodId, storeId, languageId);

            var placedOrdersCounts = await _orderShippingMethodCapacityMappingService.GetPlacedOrdersCountAsync(storeId: storeId);

            for (int index = 0; index < _deliverySchedulingSetting.NumberOfDisplayDays; index++)
            {
                var date = firstSelectableDay.AddDays(index).Date;

                var mappedDate = new AvailableDeliveryDateTimesTableModel
                {
                    DeliveryDateOnUtc = date,
                    DeliveryDateString = await date.FormatDateTimeAsync()
                };

                var availableDateTimes = availableDeliveryDateTimes.Any(t => t.ExceptionDate == date)
                ? availableDeliveryDateTimes.Where(t => t.ExceptionDate == date)
                : availableDeliveryDateTimes.Where(t => t.DayOfWeek == (int)date.DayOfWeek && t.StartDateOnUtc <= date && (!t.EndDateOnUtc.HasValue || t.EndDateOnUtc.Value >= date));

                foreach (var availableDateTime in availableDateTimes.OrderBy(t => t.DisplayOrder))
                {
                    var isActiveWorkTime = await _logisticsScheduleService.IsLogisticsActiveForScheduleAsync(date, availableDateTime.StartHour, availableDateTime.EndHour);

                    if (!isActiveWorkTime)
                        continue;

                    var placedOrdersCountForThisDateTime = 0;
                    if (placedOrdersCounts.Any(t => t.ShippingMethodId == availableDateTime.ShippingMethodId && t.DeliveryDateOnUtc == date && t.AvailableDeliveryTimeRangeId == availableDateTime.AvailableDeliveryTimeRangeId))
                        placedOrdersCountForThisDateTime = placedOrdersCounts.Single(t => t.ShippingMethodId == availableDateTime.ShippingMethodId && t.DeliveryDateOnUtc == date && t.AvailableDeliveryTimeRangeId == availableDateTime.AvailableDeliveryTimeRangeId).OrdersCount;

                    var isNotSelectable = DateTime.Now.Date == date && (availableDateTime.StartHour - DateTime.Now.TimeOfDay).TotalHours <= _deliverySchedulingSetting.StartHourOffsetIfDaysOffsetIsZero;

                    if (!isNotSelectable)
                        mappedDate.Times.Add(new AvailableDeliveryTimesTableModel()
                        {
                            ShippingMethodCapacityId = availableDateTime.ShippingMethodCapacityId,
                            Time = availableDateTime.Time,
                            AvailableCapacities = availableDateTime.Capacity - placedOrdersCountForThisDateTime,
                            AvailableDeliveryTimeRangeId = availableDateTime.AvailableDeliveryTimeRangeId
                        });
                }

                if (mappedDate.Times.Count > 0 )
                    model.Add(mappedDate);
            }

            return model;
        }

        #endregion
    }
}