//using Delsami.Plugin.Core.Bases.Infrastructure;
//using System;

//namespace Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Data.StoredProcedures
//{
//    public class GetOrderShippingMethodCapacitiesSPPresenter : RLBaseStoredProcedurePresenter
//    {
//        public override string StoredProcedureName => "RL_DS_GetOrderShippingMethodCapacities";

//  //      public override string MsSqlBody => @$"  
//  //      CREATE PROCEDURE [{StoredProcedureName}](
//  //      @shippingMethodCapacityId INT = 0,
//  //      @shippingMethodId INT = 0,
//  //      @fromDate DATETIME = NULL,
//		//@toDate DATETIME = NULL,
//		//@storeId INT = 0,
//		//@pageIndex INT = 0,
//		//@pageSize INT = 2147483646,
//		//@totalRecords INT = NULL OUTPUT
//  //      )

//  //      AS BEGIN

//  //      DECLARE @shippingMethodCapacityId1 INT = @shippingMethodCapacityId
//  //      DECLARE @shippingMethodId1 INT = @shippingMethodId
//  //      DECLARE @fromDate1 DATETIME = @fromDate
//  //      DECLARE @toDate1 DATETIME = @toDate
//  //      DECLARE @storeId1 INT = @storeId

//  //      DECLARE @sql NVARCHAR(MAX)

//  //      CREATE TABLE #RL_DS_DisplayOrderShippingMethodCapacityMappingTmp 
//  //      (
//	 //       [Id] INT IDENTITY (1, 1) NOT NULL,
//	 //       [OrderShippingMethodCapacityMappingId] INT NOT NULL
//  //      )

//  //      SET @sql = '
//  //      SELECT OS.Id 
//  //          FROM [RL_DS_OrderShippingMethodCapacityMapping] OS 
//  //          INNER JOIN [RL_DS_ShippingMethodCapacity] SM ON OS.ShippingMethodCapacityId = SM.Id
//  //          INNER JOIN [Order] O ON OS.OrderId = O.Id
//  //          WHERE O.OrderStatusId != 40 AND O.Deleted = 0'

//  //      IF @shippingMethodCapacityId1 > 0
//	 //   BEGIN
//	 //   	SET @sql = @sql + '
//	 //   	AND SM.Id = @shippingMethodCapacityId1'
//	 //   END

//  //      IF @shippingMethodId1 > 0
//	 //           BEGIN
//	 //   	        SET @sql = @sql + '
//	 //   	        AND SM.ShippingMethodId = @shippingMethodId1'
//	 //           END

//  //      IF @fromDate1 IS NOT NULL
//	 //           BEGIN
//	 //   	        SET @sql = @sql + '
//	 //   	        AND OS.DeliveryDateOnUtc >= @fromDate1'
//	 //           END

//  //      IF @toDate1 IS NOT NULL
//	 //           BEGIN
//	 //   	        SET @sql = @sql + '
//	 //   	        AND OS.DeliveryDateOnUtc <= @toDate1'
//	 //           END

//  //      IF @storeId1 > 0
//	 //           BEGIN
//	 //   	        SET @sql = @sql + '
//	 //   	        AND O.StoreId = @storeId1'
//	 //           END

//  //      SET @sql = 'INSERT INTO #RL_DS_DisplayOrderShippingMethodCapacityMappingTmp ([OrderShippingMethodCapacityMappingId])' + @sql        

//  //      EXEC sp_executesql @sql, N' @shippingMethodCapacityId1 INT,
//  //      @shippingMethodId1 INT,
//  //      @fromDate1 DATETIME,
//  //      @toDate1 DATETIME,
//  //      @storeId1 INT', @shippingMethodCapacityId1, @shippingMethodId1, @fromDate1, @toDate1, @storeId1

//  //      --paging
//  //      DECLARE @PageLowerBound INT
//  //      DECLARE @PageUpperBound INT
//  //      DECLARE @RowsToReturn INT
//  //      SET @RowsToReturn = @pageSize * (@pageIndex + 1)	
//  //      SET @PageLowerBound = @pageSize * @pageIndex
//  //      SET @PageUpperBound = @PageLowerBound + @pageSize + 1

//  //      CREATE TABLE #RL_DS_PageIndex 
//	 //       (
//		//        [IndexId] INT IDENTITY (1, 1) NOT NULL,
//		//        [OrderShippingMethodCapacityMappingId] INT NOT NULL
//	 //       )
//	 //       INSERT INTO #RL_DS_PageIndex ([OrderShippingMethodCapacityMappingId])
//	 //       SELECT [OrderShippingMethodCapacityMappingId]
//	 //       FROM #RL_DS_DisplayOrderShippingMethodCapacityMappingTmp
//	 //       GROUP BY [OrderShippingMethodCapacityMappingId]
//	 //       ORDER BY min([Id])

//  //      --total records
//  //      SET @totalRecords = @@rowcount

//  //      DROP TABLE #RL_DS_DisplayOrderShippingMethodCapacityMappingTmp

//  //      --return entities
//  //      SET @sql = 'SELECT TOP (' + CAST(@RowsToReturn AS NVARCHAR(MAX)) + ')
//	 //       OS.*
//  //      FROM
//	 //       #RL_DS_PageIndex [pi]
//	 //       INNER JOIN RL_DS_OrderShippingMethodCapacityMapping OS WITH (NOLOCK) ON OS.[Id] = [pi].[OrderShippingMethodCapacityMappingId]
//  //      WHERE
//	 //       [pi].[IndexId] > ' + CAST(@PageLowerBound AS NVARCHAR(MAX)) + ' AND 
//	 //       [pi].[IndexId] < ' + CAST(@PageUpperBound AS NVARCHAR(MAX)) + '
//  //      ORDER BY
//	 //       [pi].[IndexId]'
	
//  //      EXEC sp_executesql @sql

//  //      DROP TABLE #RL_DS_PageIndex

//  //      END";

//  //      public override string MySqlBody => throw new NotImplementedException();
//    }
//}
