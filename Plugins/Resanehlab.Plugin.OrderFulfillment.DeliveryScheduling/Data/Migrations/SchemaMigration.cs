using FluentMigrator;
using Nop.Data.Extensions;
using Nop.Data.Migrations;
using Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Domain;

namespace Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Data.Migrations
{
    [NopMigration("2020/11/07 11:40:55:1687541", "Delsami.OrderFulfillment.DeliveryScheduling base schema")]
    public class SchemaMigration : AutoReversingMigration
    {
        public override void Up()
        {
            Create.TableFor<ExceptionDate>();
            Create.TableFor<AvailableDeliveryTimeRange>();
            Create.TableFor<OverriddenStartDisplayDaysOffset>();
            Create.TableFor<AvailableDeliveryDateTimeRange>();
            Create.TableFor<ShippingMethodCapacity>();
            Create.TableFor<OrderShippingMethodCapacityMapping>();
        }
    }
}