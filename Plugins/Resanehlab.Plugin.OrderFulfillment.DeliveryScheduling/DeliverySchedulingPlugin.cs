using LinqToDB.DataProvider;
using Nop.Core.Domain.Cms;
using Nop.Core.Domain.Payments;
using Nop.Data;
using Nop.Services.Cms;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Plugins;
using Nop.Web.Framework.Infrastructure;
using Nop.Web.Framework.Menu;
using Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Areas.Admin.Components;
using Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Components;
using Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Helpers;
using Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Infrastructure;
using Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Models;
using Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Delsami.Plugin.OrderFulfillment.DeliveryScheduling
{
    public class DeliverySchedulingPlugin : BasePlugin, IAdminMenuPlugin, IWidgetPlugin
    {
        #region Fields

        private readonly ISettingService _settingService;
        private readonly WidgetSettings _widgetSettings;
        private readonly ILocalizationService _localizationService;
        private readonly INopDataProvider _dataProvider;

        public bool HideInWidgetList => false;

        #endregion

        #region Ctor

        public DeliverySchedulingPlugin(ISettingService settingService,
            WidgetSettings widgetSettings,
            ILocalizationService localizationService,
            IRLSqlProvider sqlProvider,
            INopDataProvider dataProvider)
        {
            _settingService = settingService;
            _widgetSettings = widgetSettings;
            _localizationService = localizationService;
            _dataProvider = dataProvider;
        }

        #endregion

        #region Methods

        public override string GetConfigurationPageUrl() => "/Admin/Plugin/DeliveryScheduling/Configure";

        /// <summary>
        /// Install plugin
        /// </summary>
        public async override Task InstallAsync()
        {
            #region Stored procedures

            if (!await StoredProcedureExistsAsync(Constants.LoadAvailableDeliveryDateTimesStoredProcedureName))
                await _dataProvider.ExecuteNonQueryAsync(Constants.LoadAvailableDeliveryDateTimesSqlBody);

            if (!await StoredProcedureExistsAsync(Constants.GetPlacedOrdersCountStoredProcedureName))
                await _dataProvider.ExecuteNonQueryAsync(Constants.GetPlacedOrdersCountMsSqlBody);

            if (!await StoredProcedureExistsAsync(Constants.GetOrderShippingMethodCapacitiesStoredProcedureName))
                await _dataProvider.ExecuteNonQueryAsync(Constants.GetOrderShippingMethodCapacitiesMsSqlBody);

            #endregion

            #region Resources 

            await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
            {
                ["Delsami.Plugin.OrderFulfillment.DeliveryScheduling"] = "Delivery scheduling",
                ["Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Configuration"] = "Settings",
                ["Delsami.Plugin.OrderFulfillment.DeliveryScheduling.ManageAvailableDeliveryTimeRanges"] = "Manage delivery time ranges",
                ["Delsami.Plugin.OrderFulfillment.DeliveryScheduling.ManageAvailableDeliveryDateTimeCapacities"] = "Manage delivery date time capacities",
                ["Delsami.Plugin.OrderFulfillment.DeliveryScheduling.ManageExceptionDates"] = "Manage exception dates",
                ["Delsami.Plugin.OrderFulfillment.DeliveryScheduling.DefaultSelect"] = "Select",
                ["Delsami.Plugin.OrderFulfillment.DeliveryScheduling.ExceptionDate.SameDateError"] = "Exception with the same date already exists",
                ["Delsami.Plugin.OrderFulfillment.DeliveryScheduling.AvailableDeliveryTimeRange.Fields.Time"] = "Time",
                ["Delsami.Plugin.OrderFulfillment.DeliveryScheduling.AvailableDeliveryTimeRange.Fields.Time.Hint"] = "Time.",
                ["Delsami.Plugin.OrderFulfillment.DeliveryScheduling.AvailableDeliveryTimeRange.Fields.DisplayOrder"] = "Display order",
                ["Delsami.Plugin.OrderFulfillment.DeliveryScheduling.AvailableDeliveryTimeRange.Fields.DisplayOrder.Hint"] = "Display order.",
                ["Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Configuration.Fields.EnablePlugin"] = "Enabled",
                ["Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Configuration.Fields.EnablePlugin.Hint"] = "If checked, the delivery scheduling plugin will be enabled.",
                ["Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Configuration.Fields.NumberOfDisplayDays"] = "Number of display days",
                ["Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Configuration.Fields.NumberOfDisplayDays.Hint"] = "This filed specifies the number of available days that customers are able to choose from.",
                ["Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Configuration.Fields.StartDisplayDaysOffset"] = "Start display days offset",
                ["Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Configuration.Fields.StartDisplayDaysOffset.Hint"] = "This filed specifies the interval between the customer's order day and the first day which customers can select. For example, if the value of this field is “3”, customers that are submitting an order on 2021/01/01, the first available delivery day for them will be 2021/01/04.",
                ["Delsami.Plugin.OrderFulfillment.DeliveryScheduling.ExceptionDate.Fields.Title"] = "Title",
                ["Delsami.Plugin.OrderFulfillment.DeliveryScheduling.ExceptionDate.Fields.Title.Hint"] = "Exception date title.",
                ["Delsami.Plugin.OrderFulfillment.DeliveryScheduling.ExceptionDate.Fields.DateOnUtc"] = "Date",
                ["Delsami.Plugin.OrderFulfillment.DeliveryScheduling.ExceptionDate.Fields.DateOnUtc.Hint"] = "Exception date.",
                ["Delsami.Plugin.OrderFulfillment.DeliveryScheduling.ExceptionDate.Fields.SelectedShippingMethod"] = "Shipping method",
                ["Delsami.Plugin.OrderFulfillment.DeliveryScheduling.ExceptionDate.Fields.SelectedShippingMethod.Hint"] = "Select a shipping method for defining capacities.",
                ["Delsami.Plugin.OrderFulfillment.DeliveryScheduling.ExceptionDate.Fields.DateOnUtc.Required"] = "Date field is required",
                ["Delsami.Plugin.OrderFulfillment.DeliveryScheduling.AvailableDeliveryTimeRange.ConfirmButton"] = "Confirm existing delivery time ranges",
                ["Delsami.Plugin.OrderFulfillment.DeliveryScheduling.AvailableDeliveryTimeRange.AddButton"] = "Add delivery time range",
                ["Delsami.Plugin.OrderFulfillment.DeliveryScheduling.AvailableDeliveryTimeRange.DeleteButton"] = "Delete delivery time ranges",
                ["Delsami.Plugin.OrderFulfillment.DeliveryScheduling.AvailableDeliveryTimeRange.AddNew"] = "Add new delivery time range",
                ["Delsami.Plugin.OrderFulfillment.DeliveryScheduling.AvailableDeliveryTimeRange.BackToList"] = "Back to delivery time ranges list",
                ["Delsami.Plugin.OrderFulfillment.DeliveryScheduling.AvailableDeliveryTimeRange.EditDetails"] = "Edit delivery time range details",
                ["Delsami.Plugin.OrderFulfillment.DeliveryScheduling.AvailableDeliveryTimeRange.List"] = "Delivery time ranges list",
                ["Delsami.Plugin.OrderFulfillment.DeliveryScheduling.AvailableDeliveryTimeRange.Info"] = "Info",
                ["Delsami.Plugin.OrderFulfillment.DeliveryScheduling.AvailableDeliveryDateTime.MustBeSelectedAtLeastOne"] = "At least one delivery date time must be selected",
                ["Delsami.Plugin.OrderFulfillment.DeliveryScheduling.AvailableDeliveryDateTime.NoLongerAvailable"] = "Selected date time is no longer available",
                ["Delsami.Plugin.OrderFulfillment.DeliveryScheduling.OrderDetails.DeliveryDate"] = "Delivery date",
                ["Delsami.Plugin.OrderFulfillment.DeliveryScheduling.OrderDetails.DeliveryDate.Hint"] = "Order delivery date.",
                ["Delsami.Plugin.OrderFulfillment.DeliveryScheduling.OrderDetails.DeliveryTime"] = "Delivery time",
                ["Delsami.Plugin.OrderFulfillment.DeliveryScheduling.OrderDetails.DeliveryTime.Hint"] = "Order delivery time.",
                ["Delsami.Plugin.OrderFulfillment.DeliveryScheduling.OrderDetails.ShippingMethodName"] = "Shipping method",
                ["Delsami.Plugin.OrderFulfillment.DeliveryScheduling.OrderDetails.ShippingMethodName.Hint"] = "Order delivery shipping method.",
                ["Delsami.Plugin.OrderFulfillment.DeliveryScheduling.TimeTable.Fields.ShippingMethod"] = "Shipping method",
                ["Delsami.Plugin.OrderFulfillment.DeliveryScheduling.TimeTable.Fields.ShippingMethod.Hint"] = "Select a shipping method for defining capacities.",
                ["Delsami.Plugin.OrderFulfillment.DeliveryScheduling.TimeTable.DaysOfWeek"] = "Days of week",
                ["Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Saturday"] = "Saturday",
                ["Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Sunday"] = "Sunday",
                ["Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Monday"] = "Monday",
                ["Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Tuesday"] = "Tuesday",
                ["Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Wednesday"] = "Wednesday",
                ["Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Thursday"] = "Thursday",
                ["Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Friday"] = "Friday",
                ["Delsami.Plugin.OrderFulfillment.DeliveryScheduling.TimeTable.HasNotConfimedTimeRange"] = "There are no confirmed time ranges",
                ["Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Configuration"] = "Settings",
                ["Delsami.Plugin.OrderFulfillment.DeliveryScheduling.TimeTable"] = "Time table",
                ["Delsami.Plugin.OrderFulfillment.DeliveryScheduling.ExceptionDate.Info"] = "Info",
                ["Delsami.Plugin.OrderFulfillment.DeliveryScheduling.ExceptionDate.TimeTable"] = "Time table",
                ["Delsami.Plugin.OrderFulfillment.DeliveryScheduling.ExceptionDate.TimeTable.SaveBeforeEdit"] = "You need to save the exception date before you can add time table for this page.",
                ["Delsami.Plugin.OrderFulfillment.DeliveryScheduling.ExceptionDate.AddButton"] = "Add exception date",
                ["Delsami.Plugin.OrderFulfillment.DeliveryScheduling.ExceptionDate.AddNew"] = "Add new exception date",
                ["Delsami.Plugin.OrderFulfillment.DeliveryScheduling.ExceptionDate.BackToList"] = "Back to exception dates list",
                ["Delsami.Plugin.OrderFulfillment.DeliveryScheduling.ExceptionDate.EditDetails"] = "Edit exception date details",
                ["Delsami.Plugin.OrderFulfillment.DeliveryScheduling.ExceptionDate.List"] = "Exception dates list",
                ["Delsami.Plugin.OrderFulfillment.DeliveryScheduling.TimeRange.Fields.Capacity.Required"] = "Capacity field is required",
                ["Delsami.Plugin.OrderFulfillment.DeliveryScheduling.SuccessNotification"] = "Action completed successfully",
                ["Delsami.Plugin.OrderFulfillment.DeliveryScheduling.LoadTimeTable"] = "Current available date times",
                ["Delsami.Plugin.OrderFulfillment.DeliveryScheduling.LoadTimeTable.AvailableCapacities"] = "Available capacities",
                ["Delsami.Plugin.OrderFulfillment.DeliveryScheduling.LoadTimeTableText"] = "Click here to see current available date times states",
                ["Delsami.Plugin.Orderfulfillment.Deliveryscheduling.Availabledeliverytimerange.NotconfirmedText"] = "Not Confirmed",
                ["Delsami.Plugin.Orderfulfillment.Deliveryscheduling.Availabledeliverytimerange.ConfirmedText"] = "Confirmed",
                ["Delsami.Plugin.OrderFulfillment.DeliveryScheduling.AvailableDeliveryTimeRange.DeleteOnce"] = "Delete",
                ["Delsami.Plugin.OrderFulfillment.DeliveryScheduling.AvailableDeliveryTimeRange.DeleteConfirmText"] = "Are you sure you want to delete?",
                ["Delsami.Plugin.OrderFulfillment.DeliveryScheduling.AvailableDeliveryDateTime.BookedUp"] = "Booked up",
                ["Delsami.Plugin.OrderFulfillment.DeliveryScheduling.AvailableDeliveryDateTime.NoDefinedDeliveryDateTimes"] = "There is no defined delivery date times for this shipping method",
                ["Delsami.Plugin.OrderFulfillment.DeliveryScheduling.AvailableDeliveryTimeRange.DeleteModal.Header"] = "Select a expiration date for current time ranges",
                ["Delsami.Plugin.OrderFulfillment.DeliveryScheduling.AvailableDeliveryTimeRange.DeleteConfirmation.LastOrderDateInTimeRangesText"] = "Last order creation date",
                ["Delsami.Plugin.OrderFulfillment.DeliveryScheduling.AvailableDeliveryTimeRange.DeleteConfirmation.Date"] = "Expiration date",
                ["Delsami.Plugin.OrderFulfillment.DeliveryScheduling.AvailableDeliveryTimeRange.DeleteConfirmation.Submit"] = "Submit",
                ["Delsami.Plugin.OrderFulfillment.DeliveryScheduling.OrderDeliveryList"] = "Order delivery date times list",
                ["Delsami.Plugin.OrderFulfillment.DeliveryScheduling.NoData"] = "No data",
                ["Delsami.Plugin.OrderFulfillment.DeliveryScheduling.OrderDeliveryList.Fields.FromDate"] = "From date",
                ["Delsami.Plugin.OrderFulfillment.DeliveryScheduling.OrderDeliveryList.Fields.FromDate.Hint"] = "From date for the search.",
                ["Delsami.Plugin.OrderFulfillment.DeliveryScheduling.OrderDeliveryList.Fields.ToDate"] = "To date",
                ["Delsami.Plugin.OrderFulfillment.DeliveryScheduling.OrderDeliveryList.Fields.ToDate.Hint"] = "To date for the search.",
                ["Delsami.Plugin.OrderFulfillment.DeliveryScheduling.OrderDeliveryList.Fields.SelectedShippingMethod"] = "Shipping method",
                ["Delsami.Plugin.OrderFulfillment.DeliveryScheduling.OrderDeliveryList.Fields.SelectedShippingMethod.Hint"] = "Search by a shipping method.",
                ["Delsami.Plugin.OrderFulfillment.DeliveryScheduling.OrderDeliveryList.Fields.Store"] = "Store",
                ["Delsami.Plugin.OrderFulfillment.DeliveryScheduling.OrderDeliveryList.Fields.Store.Hint"] = "Search by store.",
                ["Delsami.Plugin.OrderFulfillment.DeliveryScheduling.OrderDeliveryList.OrderId"] = "Order details link",
                ["Delsami.Plugin.OrderFulfillment.DeliveryScheduling.OrderDeliveryList.OrderDetails"] = "Order details",
                ["Delsami.Plugin.OrderFulfillment.DeliveryScheduling.OrderDeliveryList.DeliveryDateTime"] = "Delivery date time",
                ["Delsami.Plugin.OrderFulfillment.DeliveryScheduling.OrderDeliveryList.ShippingMethodName"] = "Shipping method",
                ["Delsami.Plugin.OrderFulfillment.DeliveryScheduling.AvailableDeliveryDateTime.OverloadingOrder"] = "The number of placed orders in these date times have exceeded from the current capacity: {0}",
                ["Delsami.Plugin.OrderFulfillment.DeliveryScheduling.ErrorNotification"] = "Error happened!",
                ["Delsami.Plugin.OrderFulfillment.DeliveryScheduling.AvailableDeliveryTimeRange.DeleteConfirmation"] = "Are you sure you want to delete delivery time ranges?",
                ["Delsami.Plugin.OrderFulfillment.DeliveryScheduling.TimeTable.Capacities"] = "Delivery date time capacities",
                ["Delsami.Plugin.OrderFulfillment.DeliveryScheduling.ExceptionDate.Hint"] = "<li><strong>Define exceptions for special days.</strong></li><li><strong>Define appropriate capacity for every defined time range of the day in Time table tab.</strong></li>",
                ["Delsami.Plugin.OrderFulfillment.DeliveryScheduling.ManageAvailableDeliveryDateTimeCapacities.Hint"] = "<li><strong>Select a shipping method.</strong></li><li><strong>Define capacities for pre-defined time ranges of every week day (per shipping method).</strong></li>",
                ["Delsami.Plugin.OrderFulfillment.DeliveryScheduling.LoadTimeTable.Hint"] = "<strong>This table has been provided to generate an exports, according to real dates, time ranges and remaining capacities of upcoming days, based on primary capacities and considering the effect of exception dates and calculation of the number of submitted orders for defined capacities.</strong>",
                ["Delsami.Plugin.OrderFulfillment.DeliveryScheduling.OrderDeliveryList.StoreName"] = "Store",
                ["Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Unknown"] = "Unknown",
                ["Delsami.Plugin.OrderFulfillment.DeliveryScheduling.OrderDeliveryPdf.DeliveryInformation.DeliveryDate"] = "Delivery date",
                ["Delsami.Plugin.OrderFulfillment.DeliveryScheduling.OrderDeliveryPdf.DeliveryInformation.DeliveryTime"] = "Delivery time",
                ["Delsami.Plugin.OrderFulfillment.DeliveryScheduling.OrderDeliveryList.PdfOrdersDelivery"] = "Print PDF orders delivery",
                ["Delsami.Plugin.OrderFulfillment.DeliveryScheduling.OrderDeliveryList.PdfOrdersDelivery.All"] = "Print PDF orders delivery (all found)",
                ["Delsami.Plugin.OrderFulfillment.DeliveryScheduling.OrderDeliveryList.PdfOrdersDelivery.Selected"] = "Print PDF orders delivery (selected)",
                ["Delsami.Plugin.OrderFulfillment.DeliveryScheduling.AvailableDeliveryTimeRange.IsConfirmed"] = "Confirmed",
                ["Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Configuration.Fields.DisabledShippingMethodIds"] = "Excluded shipping methods",
                ["Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Configuration.Fields.DisabledShippingMethodIds.Hint"] = "Selected shipping methods will be excluded from delivery scheduling process.",
                ["Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Configuration.Fields.HideUnselectableDays"] = "Hide unselectable days",
                ["Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Configuration.Fields.HideUnselectableDays.Hint"] = "Hide days that have no available delivery time for selecting.",
                ["Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Orders.DeliveryScheduling"] = "Delivery scheduling",
                ["Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Configuration.Fields.ShippingMethods.NoShippingMethodsAvailable"] = "No shipping methods available.",
                ["Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Configuration.Fields.DeliveryDateTimeFormatString"] = "Delivery date time format string",
                ["Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Configuration.Fields.DeliveryDateTimeFormatString.Hint"] = "A standard date and time format string for defining the text representation of a date and time value. The delivery date time will be displayed in this format in the store. You can check this 'https://docs.microsoft.com/en-us/dotnet/standard/base-types/standard-date-and-time-format-strings' link for more details.",
                ["Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Configuration.Fields.FormatDeliveryDateTimesBasedOnCustomerLanguage"] = "Format delivery date times based on customer language",
                ["Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Configuration.Fields.FormatDeliveryDateTimesBasedOnCustomerLanguage.Hint"] = "If checked, the delivery date time values in the store will be formatted based on current customer language.",
                ["Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Configuration.OverrideStartDisplayDaysOffsets.Category"] = "Category",
                ["Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Configuration.OverrideStartDisplayDaysOffsets.StartDisplayDaysOffset"] = "Start display days offset",
                ["Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Configuration.OverrideStartDisplayDaysOffsets.OverriddenStartDisplayDaysOffsetsByCategories"] = "Overridden start display days offsets by categories",
                ["Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Configuration.OverrideStartDisplayDaysOffsets.OverriddenStartDisplayDaysOffsetsByCategories.Hint"] = "You can specify the start display days offset value for every category. These offsets will override the base offset setting in some cases. For example, if a customer has a product from category ‘a’ in his cart and this category has offset ‘x’, the offset will be ‘x’. If multiple categories exist, the largest offset will be considered. If products in the cart have no categories or any of the existing categories have no defined offset, the base offset setting will be considered."
            });

            #endregion

            #region Settings

            DeliverySchedulingSetting deliverySchedulingSetting = new DeliverySchedulingSetting()
            {
                EnablePlugin = true,
                NumberOfDisplayDays = 7,
                DeliveryDateTimeFormatString = Constants.DefaultDeliveryDateTimeFormatString
            };
            await _settingService.SaveSettingAsync(deliverySchedulingSetting);

            _widgetSettings.ActiveWidgetSystemNames.Add(Constants.SystemName);
            await _settingService.SaveSettingAsync(_widgetSettings);

            var paymentSettings = await _settingService.LoadSettingAsync<PaymentSettings>();

            paymentSettings.BypassPaymentMethodSelectionIfOnlyOne = false;
            await _settingService.SaveSettingAsync<PaymentSettings>(paymentSettings);

            #endregion

            await base.InstallAsync();
        }

        /// <summary>
        /// Uninstall plugin
        /// </summary>
        public async override Task UninstallAsync()
        {
            #region Resources 

            await _localizationService.DeleteLocaleResourcesAsync("Delsami.Plugin.OrderFulfillment.DeliveryScheduling");

            #endregion

            #region Settings

            await _settingService.DeleteSettingAsync<DeliverySchedulingSetting>();

            _widgetSettings.ActiveWidgetSystemNames.Remove(Constants.SystemName);
            await _settingService.SaveSettingAsync(_widgetSettings);

            #endregion

            await base.UninstallAsync();
        }

        public Task<IList<string>> GetWidgetZonesAsync()
        {
            var widgetList = new List<string>();

            widgetList.AddRange(new List<string>
                {
                    PublicWidgetZones.CheckoutShippingMethodBottom,
                    PublicWidgetZones.OpCheckoutShippingMethodBottom,
                    PublicWidgetZones.OrderDetailsPageOverview,
                    PublicWidgetZones.OrderSummaryContentBefore,
                    AdminWidgetZones.OrderDetailsBlock
                });

            return Task.FromResult<IList<string>>(widgetList);
        }

        public Type GetWidgetViewComponent(string widgetZone)
        {
            if (widgetZone == PublicWidgetZones.OrderDetailsPageOverview)
                return typeof(LoadOrderDetailsDeliveryDateTime);
            else if (widgetZone == PublicWidgetZones.OrderSummaryContentBefore)
                return typeof(LoadOrderSummaryDeliveryDateTime);
            else if (widgetZone == AdminWidgetZones.OrderDetailsBlock)
                return typeof(OrderDeliveryDateTime);
            else
                return typeof(LoadShippingMethodContent);
        }

        public async Task ManageSiteMapAsync(SiteMapNode rootNode)
        {

            var baseName = await _localizationService.GetResourceAsync("Delsami.Plugin.BaseName");
            var systemName = await _localizationService.GetResourceAsync("Delsami.Plugin.SystemName");

            var pluginName = await _localizationService.GetResourceAsync("Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Name");
            var pluginSystemName = await _localizationService.GetResourceAsync("Delsami.Plugin.OrderFulfillment.DeliveryScheduling.SystemName");

            var configurationName = await _localizationService.GetResourceAsync("Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Configuration");
            var configurationSystemName = await _localizationService.GetResourceAsync("Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Configuration.SystemName");

            //var receiverPhoneNumberName = await _localizationService.GetResourceAsync("Delsami.Plugin.Misc.OrderSMSManagement.ReceiverPhoneNumber.List");
            //var receiverPhoneNumberSystemName = await _localizationService.GetResourceAsync("Delsami.Plugin.Misc.OrderSMSManagement.ReceiverPhoneNumber.List.SystemName");

            var delsamiSiteMapNode = rootNode.ChildNodes.Where(w => w.SystemName.Equals(systemName)).FirstOrDefault();

            if (delsamiSiteMapNode == null)
                delsamiSiteMapNode = new SiteMapNode
                {
                    Title = baseName,
                    SystemName = systemName,
                    IconClass = "fa icon-plugins",
                    Visible = true
                };

            var childNodes = new List<SiteMapNode>
            {
                new SiteMapNode()
                {
                    Title = await _localizationService.GetResourceAsync("Resanehlab.Plugin.OrderFulfillment.DeliveryScheduling.Configuration"),
                    ControllerName="DeliverySchedulingAdmin",
                    ActionName="Configure",
                    Visible = true,
                    IconClass = "fa fa fa-genderless",
                    SystemName = "Resanehlab.DeliveryScheduling.Configuration"
                },
                new SiteMapNode()
                {
                    Title = await _localizationService.GetResourceAsync("Delsami.Plugin.OrderFulfillment.DeliveryScheduling.ManageAvailableDeliveryTimeRanges"),
                    ControllerName = "AvailableDeliveryTimeRangeAdmin",
                    ActionName = "List",
                    Visible = true,
                    IconClass = "fa fa fa-genderless",
                    SystemName = "Delsami.DeliveryScheduling.ManageAvailableDeliveryTimeRanges"
                },
                new SiteMapNode()
                {
                    Title = await _localizationService.GetResourceAsync("Delsami.Plugin.OrderFulfillment.DeliveryScheduling.ManageAvailableDeliveryDateTimeCapacities"),
                    ControllerName = "DeliverySchedulingAdmin",
                    ActionName = "TimeTable",
                    Visible = true,
                    IconClass = "fa fa fa-genderless",
                    SystemName = "Delsami.DeliveryScheduling.ManageAvailableDeliveryDateTimeCapacities"
                },
                new SiteMapNode()
                {
                    Title = await _localizationService.GetResourceAsync("Delsami.Plugin.OrderFulfillment.DeliveryScheduling.ManageExceptionDates"),
                    ControllerName = "ExceptionDateAdmin",
                    ActionName = "List",
                    Visible = true,
                    IconClass = "fa fa fa-genderless",
                    SystemName = "Delsami.DeliveryScheduling.ManageExceptionDates"
                },
                new SiteMapNode()
                {
                    Title = await _localizationService.GetResourceAsync("Delsami.Plugin.OrderFulfillment.DeliveryScheduling.OrderDeliveryList"),
                    ControllerName = "DeliverySchedulingAdmin",
                    ActionName = "OrderDeliveryList",
                    Visible = true,
                    IconClass = "fa fa fa-genderless",
                    SystemName = "Delsami.DeliveryScheduling.OrderDelivery"
                }
            };



            delsamiSiteMapNode.ChildNodes.Add(new SiteMapNode
            {
                Title = pluginName,
                SystemName = pluginSystemName,
                Visible = true,
                IconClass = "fas fa-car",
                ChildNodes = childNodes
            });

            //menu.ChildNodes.Add(new SiteMapNode()
            //{
            //    Title = await _localizationService.GetResourceAsync("Delsami.Plugin.OrderFulfillment.DeliveryScheduling.ManageAvailableDeliveryDateTimeCapacities"),
            //    Url = RLUrlHelper.GetAbsoluteUrlInStore("/Admin/Plugin/DeliveryScheduling/DeliverySchedulingAdmin/TimeTable"),
            //    Visible = true,
            //    IconClass = "fa fa fa-genderless",
            //    SystemName = "Delsami.DeliveryScheduling.ManageAvailableDeliveryDateTimeCapacities"
            //});
            //menu.ChildNodes.Add(new SiteMapNode()
            //{
            //    Title = await _localizationService.GetResourceAsync("Delsami.Plugin.OrderFulfillment.DeliveryScheduling.ManageExceptionDates"),
            //    Url = RLUrlHelper.GetAbsoluteUrlInStore("/Admin/Plugin/DeliveryScheduling/ExceptionDateAdmin/List"),
            //    Visible = true,
            //    IconClass = "fa fa fa-genderless",
            //    SystemName = "Delsami.DeliveryScheduling.ManageExceptionDates"
            //});
            //menu.ChildNodes.Add(new SiteMapNode()
            //{
            //    Title = await _localizationService.GetResourceAsync("Delsami.Plugin.OrderFulfillment.DeliveryScheduling.OrderDeliveryList"),
            //    Url = RLUrlHelper.GetAbsoluteUrlInStore("/Admin/Plugin/DeliveryScheduling/DeliverySchedulingAdmin/OrderDeliveryList"),
            //    Visible = true,
            //    IconClass = "fa fa fa-genderless",
            //    SystemName = "Delsami.DeliveryScheduling.OrderDelivery"
            //});

            if (!rootNode.ChildNodes.Where(w => w.SystemName.Equals(systemName)).Any())
                rootNode.ChildNodes.Add(delsamiSiteMapNode);

            await Task.CompletedTask;
        }

        #endregion

        public async Task<bool> StoredProcedureExistsAsync(string storedProcedureName)
        {
            return (await _dataProvider.QueryAsync<IntegerQueryResultModel>(
                string.Format("SELECT 1 AS [Result] FROM sys.objects WHERE object_id = OBJECT_ID(N'[{0}]') AND type in (N'P', N'PC')",
                storedProcedureName))).FirstOrDefault() != null;
        }
    }
}
