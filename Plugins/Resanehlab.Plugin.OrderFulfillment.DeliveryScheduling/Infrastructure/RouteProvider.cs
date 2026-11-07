using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Mvc.Routing;

namespace Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Infrastructure
{
    public partial class RouteProvider : IRouteProvider
    {
        public int Priority => 9999;

        public void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder)
        {
            endpointRouteBuilder.MapControllerRoute(name: "Delsami.Plugin.OrderFulfillment.DeliveryScheduling.LoadAvailableDeliveryDateTimes",
                                                    pattern: "DeliveryScheduling/LoadAvailableDeliveryDateTimes",
                                                    defaults: new { controller = "DeliveryScheduling", action = "LoadAvailableDeliveryDateTimes" });

            endpointRouteBuilder.MapControllerRoute(name: "Delsami.Plugin.OrderFulfillment.DeliveryScheduling.LoadOpcAvailableDeliveryDateTimes",
                                                    pattern: "DeliveryScheduling/LoadOpcAvailableDeliveryDateTimes",
                                                    defaults: new { controller = "DeliveryScheduling", action = "LoadOpcAvailableDeliveryDateTimes" });

            endpointRouteBuilder.MapControllerRoute(name: "Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Admin.AvailableDeliveryTimeRangeAdmin.List",
                                                    pattern: "Admin/Plugin/DeliveryScheduling/AvailableDeliveryTimeRangeAdmin/List",
                                                    defaults: new { controller = "AvailableDeliveryTimeRangeAdmin", action = "List", area = "Admin" });

            endpointRouteBuilder.MapControllerRoute(name: "Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Admin.AvailableDeliveryTimeRangeAdmin.Create",
                                                    pattern: "Admin/Plugin/DeliveryScheduling/AvailableDeliveryTimeRangeAdmin/Create",
                                                    defaults: new { controller = "AvailableDeliveryTimeRangeAdmin", action = "Create", area = "Admin" });

            endpointRouteBuilder.MapControllerRoute(name: "Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Admin.AvailableDeliveryTimeRangeAdmin.Edit",
                                                    pattern: "Admin/Plugin/DeliveryScheduling/AvailableDeliveryTimeRangeAdmin/Edit",
                                                    defaults: new { controller = "AvailableDeliveryTimeRangeAdmin", action = "Edit", area = "Admin" });

            endpointRouteBuilder.MapControllerRoute(name: "Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Admin.AvailableDeliveryTimeRangeAdmin.Delete",
                                                    pattern: "Admin/Plugin/DeliveryScheduling/AvailableDeliveryTimeRangeAdmin/Delete",
                                                    defaults: new { controller = "AvailableDeliveryTimeRangeAdmin", action = "Delete", area = "Admin" });

            endpointRouteBuilder.MapControllerRoute(name: "Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Admin.AvailableDeliveryTimeRangeAdmin.DeleteByException",
                                                    pattern: "Admin/Plugin/DeliveryScheduling/AvailableDeliveryTimeRangeAdmin/DeleteByException/",
                                                    defaults: new { controller = "AvailableDeliveryTimeRangeAdmin", action = "DeleteByException", area = "Admin" });

            endpointRouteBuilder.MapControllerRoute(name: "Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Admin.AvailableDeliveryTimeRangeAdmin.Confirm",
                                                    pattern: "Admin/Plugin/DeliveryScheduling/AvailableDeliveryTimeRangeAdmin/Confirm",
                                                    defaults: new { controller = "AvailableDeliveryTimeRangeAdmin", action = "Confirm", area = "Admin" });

            endpointRouteBuilder.MapControllerRoute(name: "Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Admin.ExceptionDateAdmin.List",
                                                    pattern: "Admin/Plugin/DeliveryScheduling/ExceptionDateAdmin/List",
                                                    defaults: new { controller = "ExceptionDateAdmin", action = "List", area = "Admin" });

            endpointRouteBuilder.MapControllerRoute(name: "Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Admin.ExceptionDateAdmin.Create",
                                                    pattern: "Admin/Plugin/DeliveryScheduling/ExceptionDateAdmin/Create",
                                                    defaults: new { controller = "ExceptionDateAdmin", action = "Create", area = "Admin" });

            endpointRouteBuilder.MapControllerRoute(name: "Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Admin.ExceptionDateAdmin.ConfirmingTimeRanges",
                                                    pattern: "Admin/Plugin/DeliveryScheduling/ExceptionDateAdmin/ConfirmingTimeRanges",
                                                    defaults: new { controller = "ExceptionDateAdmin", action = "ConfirmingTimeRanges", area = "Admin" });

            endpointRouteBuilder.MapControllerRoute(name: "Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Admin.ExceptionDateAdmin.Edit",
                                                    pattern: "Admin/Plugin/DeliveryScheduling/ExceptionDateAdmin/Edit/{id}",
                                                    defaults: new { controller = "ExceptionDateAdmin", action = "Edit", area = "Admin" });

            endpointRouteBuilder.MapControllerRoute(name: "Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Admin.ExceptionDateAdmin.Delete",
                                                    pattern: "Admin/Plugin/DeliveryScheduling/ExceptionDateAdmin/Delete",
                                                    defaults: new { controller = "ExceptionDateAdmin", action = "Delete", area = "Admin" });

            endpointRouteBuilder.MapControllerRoute(name: "Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Admin.DeliverySchedulingAdmin.TimeTable",
                                                    pattern: "Admin/Plugin/DeliveryScheduling/DeliverySchedulingAdmin/TimeTable",
                                                    defaults: new { controller = "DeliverySchedulingAdmin", action = "TimeTable", area = "Admin" });

            endpointRouteBuilder.MapControllerRoute(name: "Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Admin.DeliverySchedulingAdmin.TimeTableList",
                                                    pattern: "Admin/Plugin/DeliveryScheduling/DeliverySchedulingAdmin/TimeTableList",
                                                    defaults: new { controller = "DeliverySchedulingAdmin", action = "TimeTableList", area = "Admin" });

            endpointRouteBuilder.MapControllerRoute(name: "Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Admin.ExceptionDateAdmin.TimeTableList",
                                                    pattern: "Admin/Plugin/DeliveryScheduling/ExceptionDateAdmin/TimeTableList",
                                                    defaults: new { controller = "ExceptionDateAdmin", action = "TimeTableList", area = "Admin" });

            endpointRouteBuilder.MapControllerRoute(name: "Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Admin.Configuration",
                                                    pattern: "Admin/Plugin/DeliveryScheduling/Configure",
                                                    defaults: new { controller = "DeliverySchedulingAdmin", action = "Configure", area = "Admin" });


            endpointRouteBuilder.MapControllerRoute(name: "Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Admin.DeliverySchedulingAdmin.TimeTablePopup",
                                                    pattern: "Admin/Plugin/DeliveryScheduling/DeliverySchedulingAdmin/TimeTablePopup",
                                                    defaults: new { controller = "DeliverySchedulingAdmin", action = "TimeTablePopup", area = "Admin" });


            endpointRouteBuilder.MapControllerRoute(name: "Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Admin.DeliverySchedulingAdmin.OrderDeliveryList",
                                                    pattern: "Admin/Plugin/DeliveryScheduling/DeliverySchedulingAdmin/OrderDeliveryList",
                                                    defaults: new { controller = "DeliverySchedulingAdmin", action = "OrderDeliveryList", area = "Admin" });


            endpointRouteBuilder.MapControllerRoute(name: "Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Admin.DeliverySchedulingAdmin.GetOrderDeliveryList",
                                                    pattern: "Admin/Plugin/DeliveryScheduling/DeliverySchedulingAdmin/GetOrderDeliveryList",
                                                    defaults: new { controller = "DeliverySchedulingAdmin", action = "GetOrderDeliveryList", area = "Admin" });


            endpointRouteBuilder.MapControllerRoute(name: "Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Admin.DeliverySchedulingAdmin.DeliveryScheduling",
                                                    pattern: "Admin/Plugin/DeliveryScheduling/DeliverySchedulingAdmin/DeliveryScheduling/{id}",
                                                    defaults: new { controller = "DeliverySchedulingAdmin", action = "DeliveryScheduling", area = "Admin" });


            endpointRouteBuilder.MapControllerRoute(name: "Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Admin.AvailableDeliveryTimeRangeAdmin.DeleteAll",
                                                    pattern: "Admin/Plugin/DeliveryScheduling/AvailableDeliveryTimeRangeAdmin/DeleteAll",
                                                    defaults: new { controller = "AvailableDeliveryTimeRangeAdmin", action = "DeleteAll", area = "Admin" });

        }
    }
}
