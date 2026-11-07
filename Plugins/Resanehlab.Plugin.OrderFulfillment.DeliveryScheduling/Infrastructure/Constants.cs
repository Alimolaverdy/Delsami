using Microsoft.AspNetCore.Http;

namespace Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Infrastructure
{
    public class Constants
    {
        public const string SystemName = "Delsami.OrderFulfillment.DeliveryScheduling";

        public const string PluginOutputPath = "~/Plugins/" + SystemName;

        public const string PluginOutputPathWithoutTilde = "/Plugins/" + SystemName;

        public const string DeliverySchedulingCssPath = "/Content/css/";

        public const string SystemCustomerAttributeNames_ShippingMethodCapacityId = "D_DS_ShippingMethodCapacityId";

        public const string SystemCustomerAttributeNames_DeliveryDateOnUtc = "D_DS_DeliveryDateOnUtc";

        public const string SystemCustomerAttributeNames_DeliveryTime = "D_DS_DeliveryTime";

        public const string DefaultDeliveryDateTimeFormatString = "dddd, MMMM d, yyyy";

        public const string OrderDeliveryDateMessageToken = "DeliveryScheduling.DeliveryDate";

        public const string OrderDeliveryTimeMessageToken = "DeliveryScheduling.DeliveryTime";

        public const string LoadAvailableDeliveryDateTimesStoredProcedureName = "D_DS_LoadAvailableDeliveryDateTimes";

        public const string LoadAvailableDeliveryDateTimesSqlBody = @$"
CREATE PROCEDURE [{LoadAvailableDeliveryDateTimesStoredProcedureName}](
    @shippingMethodId INT,
    @storeId INT,
    @languageId INT
)
AS
BEGIN

    DECLARE @now DATE = GETDATE()

    SELECT
        SMC.[Id] AS ShippingMethodCapacityId,
        SMC.[Capacity],
        ADDTR.[DayOfWeek],
        ADDTR.[StartDateOnUtc],
        ADDTR.[EndDateOnUtc],
        CASE
            WHEN LP.[LocaleValue] IS NOT NULL THEN LP.[LocaleValue]
            ELSE ADTR.[Time]
        END AS [Time],
        ADTR.[StartHour],
        ADTR.[EndHour],
        ADTR.[DisplayOrder],
        EXD.[DateOnUtc] AS ExceptionDate,
        SMC.[ShippingMethodId],
        ADDTR.[AvailableDeliveryTimeRangeId]

    FROM [D_DS_ShippingMethodCapacity] SMC

    INNER JOIN [D_DS_AvailableDeliveryDateTimeRange] ADDTR
        ON SMC.[AvailableDeliveryDateTimeRangeId] = ADDTR.[Id]

    INNER JOIN [D_DS_AvailableDeliveryTimeRange] ADTR
        ON ADDTR.[AvailableDeliveryTimeRangeId] = ADTR.[Id]

    LEFT JOIN [LocalizedProperty] LP
        ON LP.[LocaleKeyGroup] = 'AvailableDeliveryTimeRange'
        AND LP.[EntityId] = ADTR.[Id]
        AND LP.[LanguageId] = @languageId
        AND LP.[LocaleKey] = 'Time'

    LEFT JOIN [D_DS_ExceptionDate] EXD
        ON EXD.[Id] = ADDTR.[ExceptionDateId]

    WHERE
        SMC.[ShippingMethodId] = @shippingMethodId
        AND SMC.[Deleted] = 0
        AND (ADDTR.[EndDateOnUtc] IS NULL OR ADDTR.[EndDateOnUtc] >= @now)
        AND ADTR.[StoreId] = @storeId
        AND (EXD.[DateOnUtc] >= @now OR EXD.[DateOnUtc] IS NULL)
        AND (EXD.[StoreId] = @storeId OR EXD.[StoreId] IS NULL)
        AND (EXD.[Deleted] = 0 OR EXD.[Deleted] IS NULL)

    GROUP BY
        SMC.[Id],
        SMC.[Capacity],
        ADDTR.[DayOfWeek],
        ADDTR.[StartDateOnUtc],
        ADDTR.[EndDateOnUtc],
        ADTR.[StartHour],
        ADTR.[EndHour],
        ADTR.[Time],
        LP.[LocaleValue],
        ADTR.[DisplayOrder],
        EXD.[DateOnUtc],
        ADDTR.[ExceptionDateId],
        SMC.[ShippingMethodId],
        ADDTR.[AvailableDeliveryTimeRangeId]

END";

        public const string GetOrderShippingMethodCapacitiesStoredProcedureName = "D_DS_GetOrderShippingMethodCapacities";

        public const string GetOrderShippingMethodCapacitiesMsSqlBody = @$"  
              CREATE PROCEDURE [{GetOrderShippingMethodCapacitiesStoredProcedureName}](
              @shippingMethodCapacityId INT = 0,
              @shippingMethodId INT = 0,
              @fromDate DATETIME = NULL,
        @toDate DATETIME = NULL,
        @storeId INT = 0,
        @pageIndex INT = 0,
        @pageSize INT = 2147483646,
        @totalRecords INT = NULL OUTPUT
              )

              AS BEGIN

              DECLARE @shippingMethodCapacityId1 INT = @shippingMethodCapacityId
              DECLARE @shippingMethodId1 INT = @shippingMethodId
              DECLARE @fromDate1 DATETIME = @fromDate
              DECLARE @toDate1 DATETIME = @toDate
              DECLARE @storeId1 INT = @storeId

              DECLARE @sql NVARCHAR(MAX)

              CREATE TABLE #D_DS_DisplayOrderShippingMethodCapacityMappingTmp 
              (
               [Id] INT IDENTITY (1, 1) NOT NULL,
               [OrderShippingMethodCapacityMappingId] INT NOT NULL
              )

              SET @sql = '
              SELECT OS.Id 
                  FROM [D_DS_OrderShippingMethodCapacityMapping] OS 
                  INNER JOIN [D_DS_ShippingMethodCapacity] SM ON OS.ShippingMethodCapacityId = SM.Id
                  INNER JOIN [Order] O ON OS.OrderId = O.Id
                  WHERE O.OrderStatusId != 40 AND O.Deleted = 0'

              IF @shippingMethodCapacityId1 > 0
           BEGIN
           	SET @sql = @sql + '
           	AND SM.Id = @shippingMethodCapacityId1'
           END

              IF @shippingMethodId1 > 0
                   BEGIN
           	        SET @sql = @sql + '
           	        AND SM.ShippingMethodId = @shippingMethodId1'
                   END

              IF @fromDate1 IS NOT NULL
                   BEGIN
           	        SET @sql = @sql + '
           	        AND OS.DeliveryDateOnUtc >= @fromDate1'
                   END

              IF @toDate1 IS NOT NULL
                   BEGIN
           	        SET @sql = @sql + '
           	        AND OS.DeliveryDateOnUtc <= @toDate1'
                   END

              IF @storeId1 > 0
                   BEGIN
           	        SET @sql = @sql + '
           	        AND O.StoreId = @storeId1'
                   END

              SET @sql = 'INSERT INTO #D_DS_DisplayOrderShippingMethodCapacityMappingTmp ([OrderShippingMethodCapacityMappingId])' + @sql        

              EXEC sp_executesql @sql, N' @shippingMethodCapacityId1 INT,
              @shippingMethodId1 INT,
              @fromDate1 DATETIME,
              @toDate1 DATETIME,
              @storeId1 INT', @shippingMethodCapacityId1, @shippingMethodId1, @fromDate1, @toDate1, @storeId1

              --paging
              DECLARE @PageLowerBound INT
              DECLARE @PageUpperBound INT
              DECLARE @RowsToReturn INT
              SET @RowsToReturn = @pageSize * (@pageIndex + 1)	
              SET @PageLowerBound = @pageSize * @pageIndex
              SET @PageUpperBound = @PageLowerBound + @pageSize + 1

              CREATE TABLE #D_DS_PageIndex 
               (
                [IndexId] INT IDENTITY (1, 1) NOT NULL,
                [OrderShippingMethodCapacityMappingId] INT NOT NULL
               )
               INSERT INTO #D_DS_PageIndex ([OrderShippingMethodCapacityMappingId])
               SELECT [OrderShippingMethodCapacityMappingId]
               FROM #D_DS_DisplayOrderShippingMethodCapacityMappingTmp
               GROUP BY [OrderShippingMethodCapacityMappingId]
               ORDER BY min([Id])

              --total records
              SET @totalRecords = @@rowcount

              DROP TABLE #D_DS_DisplayOrderShippingMethodCapacityMappingTmp

              --return entities
              SET @sql = 'SELECT TOP (' + CAST(@RowsToReturn AS NVARCHAR(MAX)) + ')
               OS.*
              FROM
               #D_DS_PageIndex [pi]
               INNER JOIN [D_DS_OrderShippingMethodCapacityMapping] OS WITH (NOLOCK) ON OS.[Id] = [pi].[OrderShippingMethodCapacityMappingId]
              WHERE
               [pi].[IndexId] > ' + CAST(@PageLowerBound AS NVARCHAR(MAX)) + ' AND 
               [pi].[IndexId] < ' + CAST(@PageUpperBound AS NVARCHAR(MAX)) + '
              ORDER BY
               [pi].[IndexId]'

              EXEC sp_executesql @sql

              DROP TABLE #D_DS_PageIndex

              END";

        public const string GetPlacedOrdersCountStoredProcedureName = "D_DS_GetPlacedOrdersCount";

        public const string GetPlacedOrdersCountMsSqlBody = @$"  
                                                            CREATE PROCEDURE [{GetPlacedOrdersCountStoredProcedureName}](
                                                            @storeId INT = 1
                                                            )

                                                            AS BEGIN

                                                            SELECT SMC.ShippingMethodId ShippingMethodId, OSMCM.DeliveryDateOnUtc DeliveryDateOnUtc, 
                                                            ADDTR.AvailableDeliveryTimeRangeId AvailableDeliveryTimeRangeId, COUNT(*) AS OrdersCount
	                                                            FROM [D_DS_OrderShippingMethodCapacityMapping] OSMCM
	                                                            INNER JOIN [D_DS_ShippingMethodCapacity] SMC ON OSMCM.ShippingMethodCapacityId = SMC.Id
	                                                            INNER JOIN [D_DS_AvailableDeliveryDateTimeRange] ADDTR ON SMC.AvailableDeliveryDateTimeRangeId = ADDTR.Id
	                                                            INNER JOIN [Order] O ON OSMCM.OrderId = O.Id
	                                                            WHERE O.StoreId = @storeId AND O.OrderStatusId != 40 AND O.Deleted = 0
	                                                            GROUP BY SMC.ShippingMethodId, OSMCM.DeliveryDateOnUtc, ADDTR.AvailableDeliveryTimeRangeId

                                                            END";
    }
}
