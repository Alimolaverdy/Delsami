//using Delsami.Plugin.Core.Bases.Infrastructure;
//using System;

//namespace Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Data.StoredProcedures
//{
//    public class GetPlacedOrdersCountSPPresenter : RLBaseStoredProcedurePresenter
//    {
//        public override string StoredProcedureName => "RL_DS_GetPlacedOrdersCount";

//        public override string MsSqlBody => @$"  
//        CREATE PROCEDURE [{StoredProcedureName}](
//        @storeId INT = 1
//        )

//        AS BEGIN

//        SELECT SMC.ShippingMethodId ShippingMethodId, OSMCM.DeliveryDateOnUtc DeliveryDateOnUtc, 
//        ADDTR.AvailableDeliveryTimeRangeId AvailableDeliveryTimeRangeId, COUNT(*) AS OrdersCount
//	        FROM [RL_DS_OrderShippingMethodCapacityMapping] OSMCM
//	        INNER JOIN [RL_DS_ShippingMethodCapacity] SMC ON OSMCM.ShippingMethodCapacityId = SMC.Id
//	        INNER JOIN [RL_DS_AvailableDeliveryDateTimeRange] ADDTR ON SMC.AvailableDeliveryDateTimeRangeId = ADDTR.Id
//	        INNER JOIN [Order] O ON OSMCM.OrderId = O.Id
//	        WHERE O.StoreId = @storeId AND O.OrderStatusId != 40 AND O.Deleted = 0
//	        GROUP BY SMC.ShippingMethodId, OSMCM.DeliveryDateOnUtc, ADDTR.AvailableDeliveryTimeRangeId

//        END";

//        public override string MySqlBody => throw new NotImplementedException();
//    }
//}
