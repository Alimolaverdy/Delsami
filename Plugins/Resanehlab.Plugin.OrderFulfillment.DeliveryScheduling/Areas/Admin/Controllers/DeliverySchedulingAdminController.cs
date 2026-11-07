using DocumentFormat.OpenXml.Office2010.ExcelAc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Areas.Admin.Models;
using Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Domain;
using Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Factories;
using Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Services;
using Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Settings;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Shipping;
using Nop.Services.Stores;
using Nop.Web.Areas.Admin.Controllers;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Models.Extensions;
using Nop.Web.Framework.Mvc;
using NUglify.Helpers;
using StackExchange.Profiling.Internal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Areas.Admin.Controllers
{
    public partial class DeliverySchedulingAdminController : BaseAdminController
    {
        private readonly ISettingService _settingService;
        private readonly IPermissionService _permissionService;
        private readonly IWorkContext _workContext;
        private readonly IStoreService _storeService;
        private readonly IStoreContext _storeContext;
        private readonly IAvailableDeliveryTimeRangeService _availableDeliveryTimeRangeService;
        private readonly IAvailableDeliveryDateTimeRangeService _availableDeliveryDateTimeRangeService;
        private readonly IShippingService _shippingService;
        private readonly IShippingMethodCapacityService _shippingMethodCapacityService;
        private readonly IOrderShippingMethodCapacityMappingService _orderShippingMethodCapacityMappingService;
        private readonly ILocalizationService _localizationService;
        private readonly DeliverySchedulingSetting _deliverySchedulingSetting;
        private readonly IOrderService _orderService;
        private readonly OrderSettings _orderSettings;
        private readonly IOverriddenStartDisplayDaysOffsetService _overriddenStartDisplayDaysOffsetService;
        private readonly IBaseAdminModelFactory _baseAdminModelFactory;
        private readonly ICategoryService _categoryService;
        private readonly IAvailableDeliveryDateTimeTableModelFactory _availableDeliveryDateTimeTableModelFactory;
        private readonly INotificationService _notificationService;

        public DeliverySchedulingAdminController(
            ISettingService settingService,
            IPermissionService permissionService,
            IWorkContext workContext,
            IStoreService storeService,
            IStoreContext storeContext,
            IAvailableDeliveryTimeRangeService availableDeliveryTimeRangeService,
            IShippingService shippingService,
            IAvailableDeliveryDateTimeRangeService availableDeliveryDateTimeRangeService,
            IShippingMethodCapacityService shippingMethodCapacityService,
            IOrderShippingMethodCapacityMappingService orderShippingMethodCapacityMappingService,
            ILocalizationService localizationService,
            DeliverySchedulingSetting deliverySchedulingSetting,
            IOrderService orderService,
            OrderSettings orderSettings,
            IOverriddenStartDisplayDaysOffsetService overriddenStartDisplayDaysOffsetService,
            IBaseAdminModelFactory baseAdminModelFactory,
            ICategoryService categoryService,
            IAvailableDeliveryDateTimeTableModelFactory availableDeliveryDateTimeTableModelFactory
,
            INotificationService notificationService)
        {
            _settingService = settingService;
            _permissionService = permissionService;
            _storeService = storeService;
            _workContext = workContext;
            _storeContext = storeContext;
            _availableDeliveryTimeRangeService = availableDeliveryTimeRangeService;
            _shippingService = shippingService;
            _availableDeliveryDateTimeRangeService = availableDeliveryDateTimeRangeService;
            _shippingMethodCapacityService = shippingMethodCapacityService;
            _orderShippingMethodCapacityMappingService = orderShippingMethodCapacityMappingService;
            _localizationService = localizationService;
            _deliverySchedulingSetting = deliverySchedulingSetting;
            _orderService = orderService;
            _orderSettings = orderSettings;
            _overriddenStartDisplayDaysOffsetService = overriddenStartDisplayDaysOffsetService;
            _baseAdminModelFactory = baseAdminModelFactory;
            _categoryService = categoryService;
            _availableDeliveryDateTimeTableModelFactory = availableDeliveryDateTimeTableModelFactory;
            _notificationService = notificationService;
        }

        protected virtual async ValueTask<bool> HasAccessToOrderAsync(Order order)
        {
            return order != null && await HasAccessToOrderAsync(order.Id);
        }

        protected virtual async Task<bool> HasAccessToOrderAsync(int orderId)
        {
            if (orderId == 0)
                return false;

            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor == null)
                //not a vendor; has access
                return true;

            var vendorId = currentVendor.Id;
            var hasVendorProducts = (await _orderService.GetOrderItemsAsync(orderId, vendorId: vendorId)).Any();

            return hasVendorProducts;
        }

        protected virtual OrderDeliverySearchModel PrepareOrderDeliverySearchModel(OrderDeliverySearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        [NonAction]
        protected virtual async Task<OverriddenStartDisplayDaysOffsetListModel> PrepareOverriddenStartDisplayDaysOffsetListModelAsync(OverriddenStartDisplayDaysOffsetSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get overridden start display days offsets
            var overriddenStartDisplayDaysOffsets = await _overriddenStartDisplayDaysOffsetService.GetAllOverriddenStartDisplayDaysOffsetsAsync();

            //prepare list model
            var model = await new OverriddenStartDisplayDaysOffsetListModel().PrepareToGridAsync(searchModel, overriddenStartDisplayDaysOffsets, () =>
            {
                //fill in model values from the entity
                return overriddenStartDisplayDaysOffsets.SelectAwait(async c =>
                {
                    var overriddenStartDisplayDaysOffsetModel = new OverriddenStartDisplayDaysOffsetModel
                    {
                        CategoryId = c.CategoryId
                    };

                    var category = await _categoryService.GetCategoryByIdAsync(c.CategoryId);
                    overriddenStartDisplayDaysOffsetModel.CategoryName = category != null ? await _categoryService.GetFormattedBreadCrumbAsync(category) : "Deleted";

                    overriddenStartDisplayDaysOffsetModel.Id = c.Id;
                    overriddenStartDisplayDaysOffsetModel.StartDisplayDaysOffset = c.StartDisplayDaysOffset;

                    return overriddenStartDisplayDaysOffsetModel;
                });
            });

            return model;
        }

        [HttpGet]
        public virtual async Task<IActionResult> Configure()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var settings = await _settingService.LoadSettingAsync<DeliverySchedulingSetting>(storeId);

            var model = new ConfigurationModel
            {
                DeliveryDateTimeFormatString = settings.DeliveryDateTimeFormatString,
                EnablePlugin = settings.EnablePlugin,
                FormatDeliveryDateTimesBasedOnCustomerLanguage = settings.FormatDeliveryDateTimesBasedOnCustomerLanguage,
                HideUnselectableDays = settings.HideUnselectableDays,
                NumberOfDisplayDays = settings.NumberOfDisplayDays,
                StartDisplayDaysOffset = settings.StartDisplayDaysOffset,
                StartHourOffsetIfDaysOffsetIsZero = settings.StartHourOffsetIfDaysOffsetIsZero,
            };

            if (storeId > 0)
            {
                model.DeliveryDateTimeFormatString_OverrideForStore = await _settingService.SettingExistsAsync(settings, setting => setting.DeliveryDateTimeFormatString, storeId);
                model.EnablePlugin_OverrideForStore = await _settingService.SettingExistsAsync(settings, setting => setting.EnablePlugin, storeId);
                model.FormatDeliveryDateTimesBasedOnCustomerLanguage_OverrideForStore = await _settingService.SettingExistsAsync(settings, setting => setting.FormatDeliveryDateTimesBasedOnCustomerLanguage, storeId);
                model.HideUnselectableDays_OverrideForStore = await _settingService.SettingExistsAsync(settings, setting => setting.HideUnselectableDays, storeId);
                model.NumberOfDisplayDays_OverrideForStore = await _settingService.SettingExistsAsync(settings, setting => setting.NumberOfDisplayDays, storeId);
                model.StartDisplayDaysOffset_OverrideForStore = await _settingService.SettingExistsAsync(settings, setting => setting.StartDisplayDaysOffset, storeId);
                model.StartHourOffsetIfDaysOffsetIsZero_OverrideForStore = await _settingService.SettingExistsAsync(settings, setting => setting.StartHourOffsetIfDaysOffsetIsZero, storeId);
            }

            var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var deliverySchedulingSettings = await _settingService.LoadSettingAsync<DeliverySchedulingSetting>(storeScope);
            model.EnablePlugin = deliverySchedulingSettings.EnablePlugin;
            model.NumberOfDisplayDays = deliverySchedulingSettings.NumberOfDisplayDays;
            model.StartDisplayDaysOffset = deliverySchedulingSettings.StartDisplayDaysOffset;
            model.StartHourOffsetIfDaysOffsetIsZero = deliverySchedulingSettings.StartHourOffsetIfDaysOffsetIsZero;
            model.HideUnselectableDays = deliverySchedulingSettings.HideUnselectableDays;
            model.DeliveryDateTimeFormatString = deliverySchedulingSettings.DeliveryDateTimeFormatString;
            model.FormatDeliveryDateTimesBasedOnCustomerLanguage = deliverySchedulingSettings.FormatDeliveryDateTimesBasedOnCustomerLanguage;
            model.ActiveStoreScopeConfiguration = storeScope;

            if (storeScope > 0)
            {
                model.EnablePlugin_OverrideForStore = await _settingService.SettingExistsAsync(deliverySchedulingSettings, x => x.EnablePlugin, storeScope);
                model.StartDisplayDaysOffset_OverrideForStore = await _settingService.SettingExistsAsync(deliverySchedulingSettings, x => x.StartDisplayDaysOffset, storeScope);
                model.StartHourOffsetIfDaysOffsetIsZero_OverrideForStore = await _settingService.SettingExistsAsync(deliverySchedulingSettings, x => x.StartHourOffsetIfDaysOffsetIsZero, storeScope);
                model.NumberOfDisplayDays_OverrideForStore = await _settingService.SettingExistsAsync(deliverySchedulingSettings, x => x.NumberOfDisplayDays, storeScope);
                model.HideUnselectableDays_OverrideForStore = await _settingService.SettingExistsAsync(deliverySchedulingSettings, x => x.HideUnselectableDays, storeScope);
                model.DeliveryDateTimeFormatString_OverrideForStore = await _settingService.SettingExistsAsync(deliverySchedulingSettings, x => x.DeliveryDateTimeFormatString, storeScope);
                model.FormatDeliveryDateTimesBasedOnCustomerLanguage_OverrideForStore = await _settingService.SettingExistsAsync(deliverySchedulingSettings, x => x.FormatDeliveryDateTimesBasedOnCustomerLanguage, storeScope);
            }

            var disabledShippingMethodIds = deliverySchedulingSettings.DisabledShippingMethodIdsAsInt;
            model.AvailableShippingMethods = (await _shippingService.GetAllShippingMethodsAsync()).Select(s => new SelectListItem { Text = s.Name, Value = s.Id.ToString(), Selected = disabledShippingMethodIds.Contains(s.Id) }).ToList();
            model.SelectedDisabledShippingMethodIds = disabledShippingMethodIds;

            return View("~/Plugins/Delsami.OrderFulfillment.DeliveryScheduling/Areas/Admin/Views/DeliverySchedulingAdmin/Configure.cshtml", model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Configure(ConfigurationModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var deliverySchedulingSettings = await _settingService.LoadSettingAsync<DeliverySchedulingSetting>(storeScope);

            deliverySchedulingSettings.EnablePlugin = model.EnablePlugin;
            deliverySchedulingSettings.NumberOfDisplayDays = model.NumberOfDisplayDays;
            deliverySchedulingSettings.StartDisplayDaysOffset = model.StartDisplayDaysOffset;
            deliverySchedulingSettings.StartHourOffsetIfDaysOffsetIsZero = model.StartHourOffsetIfDaysOffsetIsZero;
            deliverySchedulingSettings.DisabledShippingMethodIds = string.Join(",", model.SelectedDisabledShippingMethodIds ?? new List<int>());
            deliverySchedulingSettings.HideUnselectableDays = model.HideUnselectableDays;
            deliverySchedulingSettings.DeliveryDateTimeFormatString = model.DeliveryDateTimeFormatString;
            deliverySchedulingSettings.FormatDeliveryDateTimesBasedOnCustomerLanguage = model.FormatDeliveryDateTimesBasedOnCustomerLanguage;

            await _settingService.SaveSettingOverridablePerStoreAsync(deliverySchedulingSettings, x => x.EnablePlugin, model.EnablePlugin_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(deliverySchedulingSettings, x => x.NumberOfDisplayDays, model.NumberOfDisplayDays_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(deliverySchedulingSettings, x => x.StartDisplayDaysOffset, model.StartDisplayDaysOffset_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(deliverySchedulingSettings, x => x.StartHourOffsetIfDaysOffsetIsZero, model.StartHourOffsetIfDaysOffsetIsZero_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(deliverySchedulingSettings, x => x.HideUnselectableDays, model.HideUnselectableDays_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(deliverySchedulingSettings, x => x.DeliveryDateTimeFormatString, model.DeliveryDateTimeFormatString_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(deliverySchedulingSettings, x => x.FormatDeliveryDateTimesBasedOnCustomerLanguage, model.FormatDeliveryDateTimesBasedOnCustomerLanguage_OverrideForStore, storeScope, false);

            await _settingService.SaveSettingAsync(deliverySchedulingSettings, x => x.DisabledShippingMethodIds);

            await _settingService.ClearCacheAsync();

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Delsami.Plugin.OrderFulfillment.DeliveryScheduling.SuccessNotification"));

            return await Configure();
        }

        public virtual async Task<IActionResult> TimeTable(int? storeId, int? shippingMethodId)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            var model = new TimeTableModel();

            var shippingMethods = await _shippingService.GetAllShippingMethodsAsync();

            shippingMethods = shippingMethods.Where(w => !_deliverySchedulingSetting.DisabledShippingMethodIdsAsInt.Contains(w.Id)).ToList();

            model.AvailableShippingMethods.Add(new SelectListItem { Text = await _localizationService.GetResourceAsync("Delsami.Plugin.OrderFulfillment.DeliveryScheduling.DefaultSelect"), Value = "0" });

            model.AvailableShippingMethods.AddRange(shippingMethods.Select(s => new SelectListItem
            {
                Text = s.Name,
                Value = s.Id.ToString()
            }).ToList());

            var stores = await _storeService.GetAllStoresAsync();
            if (stores.Count > 1)
            {
                foreach (var store in stores)
                {
                    model.AvailableStores.Add(new SelectListItem
                    {
                        Text = store.Name,
                        Value = store.Id.ToString(),
                        Selected = store.Id == (await _storeContext.GetCurrentStoreAsync()).Id
                    });
                }
            }

            if (!storeId.HasValue)
                storeId = (await _storeContext.GetCurrentStoreAsync()).Id;

            model.StoreId = storeId;

            model.ShippingMethodId = shippingMethodId ?? 0;

            var availableDeliveryTimeRanges = await _availableDeliveryTimeRangeService.GetAvailableDeliveryTimeRangesAsync(storeId);

            model.HasConfirmedTimeRange = await availableDeliveryTimeRanges.AnyAwaitAsync(async a => (await _availableDeliveryDateTimeRangeService.GetAvailableDeliveryDateTimeRangesAsync(availableDeliveryTimeRangeIds: new List<int> { a.Id })).Any());

            return View("~/Plugins/Delsami.OrderFulfillment.DeliveryScheduling/Areas/Admin/Views/DeliverySchedulingAdmin/TimeTable.cshtml", model);
        }

        public virtual async Task<IActionResult> TimeTableList(int shippingMethodId, int? storeId = null)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            if (!storeId.HasValue)
                storeId = (await _storeContext.GetCurrentStoreAsync()).Id;


            var availableDeliveryTimeRanges = await _availableDeliveryTimeRangeService.GetAvailableDeliveryTimeRangesAsync(storeId);

            var model = new TimeTableModel
            {
                HasConfirmedTimeRange = await availableDeliveryTimeRanges.AnyAwaitAsync(async a => (await _availableDeliveryDateTimeRangeService.GetAvailableDeliveryDateTimeRangesAsync(availableDeliveryTimeRangeIds: new List<int> { a.Id })).Any())
            };

            var timeDic = new Dictionary<string, int>();

            for (int i = 0; i < 7; i++)
            {
                model.TimeRangeLists.Add(new List<TimeRangeModel>());

                foreach (var timeRange in availableDeliveryTimeRanges)
                {
                    if (!timeDic.ContainsKey(timeRange.Time))
                    {
                        model.AvailableDeliveryTimeRanges.Add(new AvailableDeliveryTimeRangeModel
                        {
                            Time = await _localizationService.GetLocalizedAsync(timeRange, d => d.Time),
                            Id = timeRange.Id,
                            DisplayOrder = timeRange.DisplayOrder,
                            StartHour = timeRange.StartHour,
                            EndHour = timeRange.EndHour,
                        });
                        timeDic.Add(timeRange.Time, (timeRange.Id));
                    }

                    var availableDeliveryDateTimeRanges = await _availableDeliveryDateTimeRangeService.GetAvailableDeliveryDateTimeRangesAsync(availableDeliveryTimeRangeIds: new List<int> { timeRange.Id });

                    foreach (var availableDeliveryDateTimeRange in availableDeliveryDateTimeRanges.Where(w => w.DayOfWeek == i))
                    {
                        var shippingMethodCapacities = await _shippingMethodCapacityService.GetShippingMethodCapacitiesAsync(shippingMethodId: shippingMethodId, availableDeliveryDateTimeRangeId: availableDeliveryDateTimeRange.Id);

                        foreach (var shippingMethodCapacity in shippingMethodCapacities)
                        {
                            model.TimeRangeLists[i].Add(new TimeRangeModel
                            {
                                DayOfWeek = i,
                                ShippingMethodCapacityId = shippingMethodCapacity.Id,
                                Capacity = shippingMethodCapacity.Capacity,
                                AvailableDeliveryTimeRangeId = timeDic.TryGetValue(timeRange.Time, out int Rangeid) ? Rangeid : timeRange.Id
                            });
                        }
                    }
                }
            }
            model.AvailableDeliveryTimeRanges = model.AvailableDeliveryTimeRanges.OrderBy(a => a.DisplayOrder).ToList();

            var sortedList = new List<List<TimeRangeModel>>();


            for (int i = 0; i < model.TimeRangeLists.Count; i++)
            {
                sortedList.Add(new List<TimeRangeModel>());
                sortedList[i].AddRange(model.AvailableDeliveryTimeRanges.Select(a => new TimeRangeModel() { DayOfWeek = 100 }).ToList());

                foreach (var range in model.TimeRangeLists[i])
                {
                    var indexOfTimeRangeinGrid = model.AvailableDeliveryTimeRanges.FindIndex(0, a => a.Id == range.AvailableDeliveryTimeRangeId);

                    var item = sortedList[i][indexOfTimeRangeinGrid] = model.TimeRangeLists[i].First(a => a.AvailableDeliveryTimeRangeId == range.AvailableDeliveryTimeRangeId);
                }
            }

            model.TimeRangeLists = sortedList;
            model.ShippingMethodId = shippingMethodId;
            model.StoreId = storeId;

            return View("~/Plugins/Delsami.OrderFulfillment.DeliveryScheduling/Areas/Admin/Views/DeliverySchedulingAdmin/TimeTableList.cshtml", model);

        }

        [HttpPost]
        public virtual async Task<IActionResult> TimeTable(TimeTableModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            if (!model.StoreId.HasValue)
                model.StoreId = (await _storeContext.GetCurrentStoreAsync()).Id;

            if (ModelState.IsValid)
            {
                var warnings = new List<string>();

                var placedOrdersCounts = await _orderShippingMethodCapacityMappingService.GetPlacedOrdersCountAsync(storeId: model.StoreId.Value);

                foreach (var timeRange in model.TimeRanges)
                {
                    var shippingMethodCapacity = await _shippingMethodCapacityService.GetShippingMethodCapacityByIdAsync(timeRange.ShippingMethodCapacityId);
                    if (shippingMethodCapacity == null)
                        throw new NullReferenceException(nameof(shippingMethodCapacity));

                    shippingMethodCapacity.Capacity = timeRange.Capacity;

                    await _shippingMethodCapacityService.UpdateShippingMethodCapacityAsync(shippingMethodCapacity);

                    var poc = placedOrdersCounts.Where(t => t.ShippingMethodId == shippingMethodCapacity.ShippingMethodId && t.AvailableDeliveryTimeRangeId == timeRange.AvailableDeliveryTimeRangeId && (int)t.DeliveryDateOnUtc.DayOfWeek == timeRange.DayOfWeek && t.DeliveryDateOnUtc >= DateTime.UtcNow.Date).ToList();

                    foreach (var item in poc.Where(t => t.OrdersCount > timeRange.Capacity))
                    {
                        var availableDeliveryDateTimeRange = await _availableDeliveryDateTimeRangeService.GetAvailableDeliveryDateTimeRangeByIdAsync(shippingMethodCapacity.AvailableDeliveryDateTimeRangeId);
                        if (availableDeliveryDateTimeRange == null)
                            throw new NullReferenceException(nameof(availableDeliveryDateTimeRange));

                        var availableDeliveryTimeRange = await _availableDeliveryTimeRangeService.GetAvailableDeliveryTimeRangeByIdAsync(availableDeliveryDateTimeRange.AvailableDeliveryTimeRangeId);
                        if (availableDeliveryTimeRange == null)
                            throw new NullReferenceException(nameof(availableDeliveryTimeRange));

                        warnings.Add(item.DeliveryDateOnUtc.ToShortDateString() + " " + await _localizationService.GetLocalizedAsync(availableDeliveryTimeRange, d => d.Time));
                    }
                }
                if (warnings.Any())
                {
                    warnings = warnings.Distinct().ToList();

                    _notificationService.WarningNotification(string.Format(await _localizationService.GetResourceAsync("Delsami.Plugin.OrderFulfillment.DeliveryScheduling.AvailableDeliveryDateTime.OverloadingOrder"), string.Join(" - ", warnings)));
                }

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Delsami.Plugin.OrderFulfillment.DeliveryScheduling.SuccessNotification"));

                return RedirectToAction("TimeTable", new { storeId = model.StoreId, shippingMethodId = model.ShippingMethodId });
            }
            else
            {
                var shippingMethods = await _shippingService.GetAllShippingMethodsAsync();

                shippingMethods = shippingMethods.Where(w => !_deliverySchedulingSetting.DisabledShippingMethodIdsAsInt.Contains(w.Id)).ToList();

                model.AvailableShippingMethods.Add(new SelectListItem { Text = await _localizationService.GetResourceAsync("Delsami.Plugin.OrderFulfillment.DeliveryScheduling.DefaultSelect"), Value = "0" });
                model.AvailableShippingMethods.AddRange(shippingMethods.Select(s => new SelectListItem
                {
                    Text = s.Name,
                    Value = s.Id.ToString()
                }).ToList());

                var stores = await _storeService.GetAllStoresAsync();
                if (stores.Count > 1)
                {
                    foreach (var store in stores)
                    {
                        model.AvailableStores.Add(new SelectListItem
                        {
                            Text = store.Name,
                            Value = store.Id.ToString(),
                            Selected = store.Id == (await _storeContext.GetCurrentStoreAsync()).Id
                        });
                    }
                }

                var availableDeliveryTimeRanges = await _availableDeliveryTimeRangeService.GetAvailableDeliveryTimeRangesAsync(model.StoreId);

                model.HasConfirmedTimeRange = await availableDeliveryTimeRanges.AnyAwaitAsync(async a => (await _availableDeliveryDateTimeRangeService.GetAvailableDeliveryDateTimeRangesAsync(availableDeliveryTimeRangeIds: new List<int> { a.Id })).Any());

                return View("~/Plugins/Delsami.OrderFulfillment.DeliveryScheduling/Areas/Admin/Views/DeliverySchedulingAdmin/TimeTable.cshtml", model);
            }
        }

        public virtual async Task<IActionResult> TimeTablePopup(int shippingMethodId, int? storeId)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            if (!storeId.HasValue)
                storeId = (await _storeContext.GetCurrentStoreAsync()).Id;

            var shippingMethod = await _shippingService.GetShippingMethodByIdAsync(shippingMethodId);

            if (shippingMethod == null)
                return Content("");

            var firstSelectableDay = DateTime.UtcNow.AddDays(_deliverySchedulingSetting.StartDisplayDaysOffset);

            var model = await _availableDeliveryDateTimeTableModelFactory.PrepareAvailableDeliveryDateTimesTableAsync(shippingMethod.Id, storeId.Value, (await _workContext.GetWorkingLanguageAsync()).Id, firstSelectableDay);

            return View("~/Plugins/Delsami.OrderFulfillment.DeliveryScheduling/Areas/Admin/Views/DeliverySchedulingAdmin/TimeTablePopup.cshtml", model);
        }

        public virtual async Task<IActionResult> OrderDeliveryList()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            var shippingMehtods = await _shippingService.GetAllShippingMethodsAsync();
            var model = new OrderDeliverySearchModel();

            model.AvailableShippingMethods.Add(new SelectListItem { Text = await _localizationService.GetResourceAsync("Admin.Common.All"), Value = "0" });
            model.AvailableShippingMethods.AddRange(shippingMehtods.Select(s => new SelectListItem
            {
                Text = s.Name,
                Value = s.Id.ToString()
            }).ToList());

            //stores
            model.AvailableStores.Add(new SelectListItem { Text = await _localizationService.GetResourceAsync("Admin.Common.All"), Value = "0" });
            foreach (var s in await _storeService.GetAllStoresAsync())
                model.AvailableStores.Add(new SelectListItem { Text = s.Name, Value = s.Id.ToString() });

            //prepare model
            model = PrepareOrderDeliverySearchModel(model);

            return View("~/Plugins/Delsami.OrderFulfillment.DeliveryScheduling/Areas/Admin/Views/DeliverySchedulingAdmin/OrderDeliveryList.cshtml", model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> GetOrderDeliveryList(OrderDeliverySearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            var orderShippingMethodCapacities = await _orderShippingMethodCapacityMappingService.GetOrderShippingMethodCapacitiesAsync(shippingMethodId: searchModel.SelectedShippingMethodId.HasValue ? searchModel.SelectedShippingMethodId.Value : 0, fromDate: searchModel.FromDate, toDate: searchModel.ToDate, storeId: searchModel.StoreId, pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            var model = await new OrderDeliveryListModel().PrepareToGridAsync(searchModel, orderShippingMethodCapacities, () =>
            {
                return orderShippingMethodCapacities.SelectAwait(async s =>
                {
                    var shippingMethodCapacity = await _shippingMethodCapacityService.GetShippingMethodCapacityByIdAsync(s.ShippingMethodCapacityId);
                    if (shippingMethodCapacity == null)
                        throw new NullReferenceException(nameof(shippingMethodCapacity));

                    var availableDeliveryDateTimeRange = await _availableDeliveryDateTimeRangeService.GetAvailableDeliveryDateTimeRangeByIdAsync(shippingMethodCapacity.AvailableDeliveryDateTimeRangeId);
                    if (availableDeliveryDateTimeRange == null)
                        throw new NullReferenceException(nameof(availableDeliveryDateTimeRange));

                    var availableDeliveryTimeRange = await _availableDeliveryTimeRangeService.GetAvailableDeliveryTimeRangeByIdAsync(availableDeliveryDateTimeRange.AvailableDeliveryTimeRangeId);
                    if (availableDeliveryTimeRange == null)
                        throw new NullReferenceException(nameof(availableDeliveryTimeRange));

                    var store = await _storeService.GetStoreByIdAsync(availableDeliveryTimeRange.StoreId);

                    var order = await _orderService.GetOrderByIdAsync(s.OrderId);

                    return new OrderDeliveryModel
                    {
                        DeliveryDate = s.DeliveryDateOnUtc.ToShortDateString(),
                        DeliveryTime = shippingMethodCapacity != null ? await _localizationService.GetLocalizedAsync(availableDeliveryTimeRange, a => a.Time) : await _localizationService.GetResourceAsync("Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Unknown"),
                        OrderId = s.OrderId,
                        ShippingMethodName = order != null ? order.ShippingMethod : await _localizationService.GetResourceAsync("Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Unknown"),
                        StoreName = store != null ? store.Name : await _localizationService.GetResourceAsync("Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Unknown"),
                        OrderName = "Order details"
                    };
                });
            });

            return Json(model);
        }

        [HttpPost, ActionName("OrderDeliveryList")]
        [FormValueRequired("pdf-all")]
        public virtual async Task<IActionResult> PdfAll(OrderDeliverySearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            //load orders
            var orderShippingMethodCapacities = await _orderShippingMethodCapacityMappingService.GetOrderShippingMethodCapacitiesAsync(shippingMethodId: searchModel.SelectedShippingMethodId.HasValue ? searchModel.SelectedShippingMethodId.Value : 0, fromDate: searchModel.FromDate, toDate: searchModel.ToDate, storeId: searchModel.StoreId);

            var orders = new List<Order>();

            var ids = orderShippingMethodCapacities.Select(t => t.OrderId)
                .ToArray();
            orders.AddRange(await _orderService.GetOrdersByIdsAsync(ids));

            //a vendor should have access only to his products
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor != null)
            {
                orders = await orders.WhereAwait(HasAccessToOrderAsync).ToListAsync();
            }

            try
            {
                byte[] bytes;
                await using (var stream = new MemoryStream())
                {
                    await _orderShippingMethodCapacityMappingService.PrintOrdersToPdfAsync(stream, orders, _orderSettings.GeneratePdfInvoiceInCustomerLanguage ? null : await _workContext.GetWorkingLanguageAsync(), currentVendor);
                    bytes = stream.ToArray();
                }

                return File(bytes, "application/zip", "orders.zip");
            }
            catch (Exception exc)
            {
                await _notificationService.ErrorNotificationAsync(exc);
                return RedirectToAction("OrderDeliveryList");
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> PdfSelected(string selectedIds)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var orders = new List<Order>();
            if (selectedIds != null)
            {
                var ids = selectedIds
                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => Convert.ToInt32(x))
                    .ToArray();
                orders.AddRange(await _orderService.GetOrdersByIdsAsync(ids));
            }

            //ensure that we at least one order selected
            if (!orders.Any())
            {
                _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Admin.Orders.NoOrders"));
                return RedirectToAction("OrderDeliveryList");
            }

            //a vendor should have access only to his products
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor != null)
            {
                orders = await orders.WhereAwait(HasAccessToOrderAsync).ToListAsync();
            }

            try
            {
                byte[] bytes;
                await using (var stream = new MemoryStream())
                {
                    await _orderShippingMethodCapacityMappingService.PrintOrdersToPdfAsync(stream, orders, _orderSettings.GeneratePdfInvoiceInCustomerLanguage ? null : await _workContext.GetWorkingLanguageAsync(), currentVendor);
                    bytes = stream.ToArray();
                }

                return File(bytes, "application/zip", "orders.zip");
            }
            catch (Exception exc)
            {
                await _notificationService.ErrorNotificationAsync(exc);
                return RedirectToAction("OrderDeliveryList");
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> OverriddenStartDisplayDaysOffsetsList(OverriddenStartDisplayDaysOffsetSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            //prepare model
            var model = await PrepareOverriddenStartDisplayDaysOffsetListModelAsync(searchModel);

            return Json(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> CreateOverriddenStartDisplayDaysOffset(OverriddenStartDisplayDaysOffsetModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            if (model != null)
                await _overriddenStartDisplayDaysOffsetService.InsertOverriddenStartDisplayDaysOffsetAsync(new OverriddenStartDisplayDaysOffset()
                {
                    CategoryId = model.CategoryId,
                    StartDisplayDaysOffset = model.StartDisplayDaysOffset
                });

            return new NullJsonResult();
        }

        [HttpPost]
        public virtual async Task<IActionResult> UpdateOverriddenStartDisplayDaysOffset(OverriddenStartDisplayDaysOffsetModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            if (model != null)
            {
                var overriddenStartDisplayDaysOffset = await _overriddenStartDisplayDaysOffsetService.GetOverriddenStartDisplayDaysOffsetByIdAsync(model.Id);
                if (overriddenStartDisplayDaysOffset != null)
                {
                    overriddenStartDisplayDaysOffset.CategoryId = model.CategoryId;
                    overriddenStartDisplayDaysOffset.StartDisplayDaysOffset = model.StartDisplayDaysOffset;

                    await _overriddenStartDisplayDaysOffsetService.UpdateOverriddenStartDisplayDaysOffsetAsync(overriddenStartDisplayDaysOffset);
                }
            }

            return new NullJsonResult();
        }

        [HttpPost]
        public virtual async Task<IActionResult> DeleteOverriddenStartDisplayDaysOffset(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            if (id == 0)
                return new NullJsonResult();

            var overriddenStartDisplayDaysOffset = await _overriddenStartDisplayDaysOffsetService.GetOverriddenStartDisplayDaysOffsetByIdAsync(id);
            if (overriddenStartDisplayDaysOffset != null)
                await _overriddenStartDisplayDaysOffsetService.DeleteOverriddenStartDisplayDaysOffsetAsync(overriddenStartDisplayDaysOffset);

            return new NullJsonResult();
        }

        [HttpPost]
        public virtual async Task<IActionResult> GetAvailableCategoriesForOverrideStartDisplayDaysOffsetJsonModel(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            var overrideStartDisplayDaysOffsets = await _overriddenStartDisplayDaysOffsetService.GetAllOverriddenStartDisplayDaysOffsetsAsync();

            //already added categories should be excluded
            var categoriesToExclude = overrideStartDisplayDaysOffsets.Select(t => t.CategoryId);

            var overrideStartDisplayDaysOffset = overrideStartDisplayDaysOffsets.SingleOrDefault(t => t.Id == id);
            if (overrideStartDisplayDaysOffset != null)
                categoriesToExclude = categoriesToExclude.Where(t => t != overrideStartDisplayDaysOffset.CategoryId);//Exclude the editing record from excluding

            var availableCategories = new List<SelectListItem>();
            await _baseAdminModelFactory.PrepareCategoriesAsync(availableCategories, false);

            availableCategories = availableCategories.Where(t => !categoriesToExclude.Contains(int.Parse(t.Value))).ToList();

            return Json(availableCategories);
        }
    }
}
