using Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Areas.Admin.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Factories
{
    public partial interface IAvailableDeliveryDateTimeTableModelFactory
    {
        Task<List<AvailableDeliveryDateTimesTableModel>> PrepareAvailableDeliveryDateTimesTableAsync(int shippingMethodId, int storeId, int languageId, DateTime firstSelectableDay);
    }
}