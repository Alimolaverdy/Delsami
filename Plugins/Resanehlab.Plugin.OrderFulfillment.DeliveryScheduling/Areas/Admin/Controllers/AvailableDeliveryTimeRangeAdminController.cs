using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Areas.Admin.Models;
using Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Domain;
using Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Services;
using Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Settings;
using Nop.Core;
using Nop.Core.Domain.Shipping;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Services.Shipping;
using Nop.Services.Stores;
using Nop.Web.Areas.Admin.Controllers;
using Nop.Web.Framework.Models.Extensions;
using Nop.Web.Framework.Mvc;
using NUglify.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Areas.Admin.Controllers
{
    public class AvailableDeliveryTimeRangeAdminController : BaseAdminController
    {
        #region Fields

        private readonly IAvailableDeliveryTimeRangeService _availableDeliveryTimeRangeService;
        private readonly IPermissionService _permissionService;
        private readonly IStoreContext _storeContext;
        private readonly IStoreService _storeService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly ILanguageService _languageService;
        private readonly IAvailableDeliveryDateTimeRangeService _availableDeliveryDateTimeRangeService;
        private readonly IShippingService _shippingService;
        private readonly IShippingMethodCapacityService _shippingMethodCapacityService;
        private readonly IExceptionDateService _exceptionDateService;
        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;
        private readonly DeliverySchedulingSetting _deliverySchedulingSetting;
        private Dictionary<int, int> DaysOfWeekMapperDic = new Dictionary<int, int> {
            {1,6 },
            {2,0 },
            {3,1 },
            {4,2 },
            {5,3 },
            {6,4 },
            {7,5 }
        };

        private List<string> dayNames = new List<string>() {
            "شنبه",
            "یکشنبه",
            "دوشنبه",
            "سه شنبه",
            "چهارشنبه",
            "پنج شنبه",
            "جمعه",
        };

        #endregion

        #region Ctor

        public AvailableDeliveryTimeRangeAdminController(IAvailableDeliveryTimeRangeService availableDeliveryTimeRangeService,
            IPermissionService permissionService,
            IStoreContext storeContext,
            DeliverySchedulingSetting deliverySchedulingSetting,
            IStoreService storeService,
            ILocalizedEntityService localizedEntityService,
            ILanguageService languageService,
            IAvailableDeliveryDateTimeRangeService availableDeliveryDateTimeRangeService,
            IShippingService shippingService,
            IShippingMethodCapacityService shippingMethodCapacityService,
            IExceptionDateService exceptionDateService,
            ILocalizationService localizationService,
            ISettingService settingService,
            INotificationService notificationService)
        {
            _availableDeliveryTimeRangeService = availableDeliveryTimeRangeService;
            _deliverySchedulingSetting = deliverySchedulingSetting;
            _permissionService = permissionService;
            _storeContext = storeContext;
            _storeService = storeService;
            _localizedEntityService = localizedEntityService;
            _languageService = languageService;
            _availableDeliveryDateTimeRangeService = availableDeliveryDateTimeRangeService;
            _shippingService = shippingService;
            _shippingMethodCapacityService = shippingMethodCapacityService;
            _exceptionDateService = exceptionDateService;
            _localizationService = localizationService;
            _notificationService = notificationService;
        }

        #endregion

        #region Methods

        public virtual async Task<IActionResult> List(int? storeId = null)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            storeId ??= (await _storeContext.GetCurrentStoreAsync()).Id;

            var availableDeliveryTimeRanges = await _availableDeliveryTimeRangeService.GetAvailableDeliveryTimeRangesAsync(storeId);

            var model = new AvailableDeliveryTimeRangeListModel
            {
                IsConfirmed = await availableDeliveryTimeRanges.AnyAwaitAsync(async w => (await _availableDeliveryDateTimeRangeService.GetAvailableDeliveryDateTimeRangesAsync(availableDeliveryTimeRangeIds: new List<int> { w.Id })).Any()),
                StoreId = storeId,
                ItemsCount = availableDeliveryTimeRanges.Count()
            };

            var stores = await _storeService.GetAllStoresAsync();
            if (stores.Count > 1)
            {
                List<SelectListItem> availableStores = new List<SelectListItem>
                {
                    new SelectListItem
                    {
                        Text = await _localizationService.GetResourceAsync("Delsami.Plugin.OrderFulfillment.DeliveryScheduling.DefaultSelect"),
                        Value = ""
                    }
                };
                availableStores.AddRange(await stores.SelectAwait(async s => new SelectListItem
                {
                    Text = await _localizationService.GetLocalizedAsync(s, d => d.Name),
                    Value = Url.Action("List", new { storeId = s.Id }),
                    Selected = storeId == s.Id
                }).ToListAsync());

                model.AvailableStores = availableStores;
            }

            return View("~/Plugins/Delsami.OrderFulfillment.DeliveryScheduling/Areas/Admin/Views/AvailableDeliveryTimeRangeAdmin/List.cshtml", model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> List(AvailableDeliveryTimeRangeSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            var availableDeliveryTimeRanges = await _availableDeliveryTimeRangeService.GetAvailableDeliveryTimeRangesAsync(searchModel.StoreId, searchModel.Page - 1, searchModel.PageSize);
            var shippingMethods = await _shippingService.GetAllShippingMethodsAsync();

            var model = await new AvailableDeliveryTimeRangeListPartialModel().PrepareToGridAsync(searchModel, availableDeliveryTimeRanges, () =>
            {
                return availableDeliveryTimeRanges.SelectAwait(async x =>
                 {
                     var m = new AvailableDeliveryTimeRangeModel
                     {
                         Id = x.Id,
                         DisplayOrder = x.DisplayOrder,
                         Time = await _localizationService.GetLocalizedAsync(x, d => d.Time),
                         IsConfirmed = (await _availableDeliveryDateTimeRangeService.GetAvailableDeliveryDateTimeRangesAsync(availableDeliveryTimeRangeIds: new List<int> { x.Id })).Any(),
                         StartHour = x.StartHour,
                         EndHour = x.EndHour,
                         ShippingMethodTexts = string.Join(" - ", shippingMethods.Where(a => x.ShippingMethodIds.Split("-").ToList().Contains(a.Id.ToString())).Select(a => a.Name).ToList()),
                         SelectedDayText = string.Join(" - ", x.DaysOfWeek.Split("-").Select(int.Parse).ToList().Select(a => dayNames[DaysOfWeekMapperDic.FirstOrDefault(f => f.Value == a).Key - 1]).ToList()),

                     };
                     return m;
                 });
            });

            return Json(model);
        }

        public virtual async Task<IActionResult> Create(int? storeId)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            storeId ??= (await _storeContext.GetCurrentStoreAsync()).Id;

            var adt = await _availableDeliveryDateTimeRangeService.GetAvailableDeliveryDateTimeRangesAsync(isActive: true, storeId: storeId);
            if (adt != null && !adt.Any())
            {
                var model = new AvailableDeliveryTimeRangeModel()
                {
                    StoreId = storeId.Value
                };

                await AddLocalesAsync(_languageService, model.Locales);
                model.AvailableDays = PrepareDayOfWeek();
                model.AvailableShippingMethod = await GetAvailableShippingMethods();

                return View("~/Plugins/Delsami.OrderFulfillment.DeliveryScheduling/Areas/Admin/Views/AvailableDeliveryTimeRangeAdmin/Create.cshtml", model);
            }
            else
                return RedirectToAction("List", new { storeId = storeId });
        }

        [HttpPost]
        public virtual async Task<IActionResult> Create(AvailableDeliveryTimeRangeModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            model.StoreId = model.StoreId != 0 ? model.StoreId : (await _storeContext.GetCurrentStoreAsync()).Id;

            string daysOfWeek = null;
            if (model.SelectedDayIds != null)
                daysOfWeek = string.Join("-", model.SelectedDayIds.Select(a => DaysOfWeekMapperDic[a]));

            var shippngMethods = string.Join("-", model.ShippingMethodIds);

            var entity = new AvailableDeliveryTimeRange
            {
                StoreId = model.StoreId,
                DisplayOrder = model.DisplayOrder,
                Time = $"ارسال بین ساعت {model.StartHour.Hours} الی {model.EndHour.Hours}",
                StartHour = model.StartHour,
                EndHour = model.EndHour,
                DaysOfWeek = daysOfWeek,
                ShippingMethodIds = shippngMethods,
                ExceptionDateId = model.ExceptionDateId,

            };

            await _availableDeliveryTimeRangeService.InsertAvailableDeliveryTimeRangeAsync(entity);

            await UpdateLocalesAsync(entity, model);

            if (model.ExceptionDateId == null)
            {

                //var exceptionDates = await _exceptionDateService.GetExceptionDatesAsync(entity.StoreId);

                //foreach (var exceptionDate in exceptionDates)
                //{
                //    var availableDeliveryDateTimeRange = new AvailableDeliveryDateTimeRange
                //    {
                //        AvailableDeliveryTimeRangeId = entity.Id,
                //        ExceptionDateId = exceptionDate.Id,
                //        StartDateOnUtc = DateTime.UtcNow.Date,
                //    };

                //    await _availableDeliveryDateTimeRangeService.InsertAvailableDeliveryDateTimeRangeAsync(availableDeliveryDateTimeRange);
                //    foreach (var shippingMethodId in model.ShippingMethodIds)
                //    {
                //        await _shippingMethodCapacityService.InsertShippingMethodCapacityAsync(new ShippingMethodCapacity
                //        {
                //            ShippingMethodId = shippingMethodId,
                //            AvailableDeliveryDateTimeRangeId = availableDeliveryDateTimeRange.Id,
                //            Capacity = 0,
                //        });
                //    }
                //}
                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Delsami.Plugin.OrderFulfillment.DeliveryScheduling.SuccessNotification"));


                return RedirectToAction("List", new { storeId = entity.StoreId });
            }
            else
            {

                return Json(new { Result = true });
            }
        }


        public virtual async Task<IActionResult> Delete(int availableDeliveryTimeRangeId, int? storeId)
        {
            var adt = await _availableDeliveryTimeRangeService.GetAvailableDeliveryTimeRangeByIdAsync(availableDeliveryTimeRangeId);
            if (adt != null && !(await _availableDeliveryDateTimeRangeService.GetAvailableDeliveryDateTimeRangesAsync(availableDeliveryTimeRangeIds: new List<int> { adt.Id })).Any())
            {
                await _availableDeliveryTimeRangeService.DeleteAvailableDeliveryTimeRangeAsync(adt);
                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Delsami.Plugin.OrderFulfillment.DeliveryScheduling.SuccessNotification"));
            }

            return RedirectToAction("List", new { storeId = storeId ?? null });
        }

        [HttpPost]
        public virtual async Task<IActionResult> DeleteByException(int id)
        {
            var adt = await _availableDeliveryTimeRangeService.GetAvailableDeliveryTimeRangeByIdAsync(id);
            if (adt != null)
            {
                await _availableDeliveryTimeRangeService.DeleteAvailableDeliveryTimeRangeAsync(adt);
                var dtr = await _availableDeliveryDateTimeRangeService.GetAvailableDeliveryDateTimeRangesAsync(isActive: true, availableDeliveryTimeRangeIds: new List<int> { id }, justExceptionsDateTimeRange: true);
                dtr.ForEach(item =>
                {
                    var endDate = item.EndDateOnUtc = item.StartDateOnUtc == DateTime.UtcNow.Date ? DateTime.Now.AddDays(-1).Date : DateTime.UtcNow.Date;
                    item.EndDateOnUtc = endDate;
                });
                await _availableDeliveryDateTimeRangeService.UpdateAvailableDeliveryDateTimeRangesAsync(dtr);

                var shippingCapacites = await _shippingMethodCapacityService.GetByAvailableDeliveryDateTimeRangeIdsAsync(dtr.Select(a => a.Id).ToList());
                await _shippingMethodCapacityService.DeleteByDateTimeIdsAsync(shippingCapacites.Select(a => a.AvailableDeliveryDateTimeRangeId).ToList());


                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Delsami.Plugin.OrderFulfillment.DeliveryScheduling.SuccessNotification"));
                return new NullJsonResult();

            }

            throw new ArgumentException("No time range found with the specified id", nameof(id));

        }

        public virtual async Task<IActionResult> Confirm(int? storeId = null)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            storeId ??= (await _storeContext.GetCurrentStoreAsync()).Id;

            var availablityDeliveryTimeRanges = await _availableDeliveryTimeRangeService.GetAvailableDeliveryTimeRangesAsync(storeId);
            if (availablityDeliveryTimeRanges.Count() == 0)
                return RedirectToAction("List");

            //var shippingMethods = await _shippingService.GetAllShippingMethodsAsync();

            var endDate = await _availableDeliveryDateTimeRangeService.GetLatestEndDateAsync(storeId.Value);

            var availablityTimeRanges = await availablityDeliveryTimeRanges.WhereAwait(async w => !(await _availableDeliveryDateTimeRangeService.GetAvailableDeliveryDateTimeRangesAsync(availableDeliveryTimeRangeIds: new List<int> { w.Id })).Any()).ToListAsync();

            foreach (var availablityTimeRange in availablityTimeRanges)
            {
                var days = availablityTimeRange.DaysOfWeek.Split("-").Select(int.Parse);

                foreach (var day in days)
                {
                    var availableDeliveryDateTimeRange = new AvailableDeliveryDateTimeRange
                    {
                        AvailableDeliveryTimeRangeId = availablityTimeRange.Id,
                        DayOfWeek = day,
                        StartDateOnUtc = endDate != DateTime.MinValue ? endDate.AddDays(1).Date : DateTime.Now.Date,
                    };
                    await _availableDeliveryDateTimeRangeService.InsertAvailableDeliveryDateTimeRangeAsync(availableDeliveryDateTimeRange);
                    var shippingMethodIds = availablityTimeRange.ShippingMethodIds.Split("-").Select(a => int.Parse(a));
                    foreach (var shippingMethod in shippingMethodIds)
                    {
                        await _shippingMethodCapacityService.InsertShippingMethodCapacityAsync(new ShippingMethodCapacity
                        {
                            AvailableDeliveryDateTimeRangeId = availableDeliveryDateTimeRange.Id,
                            Capacity = 0,
                            ShippingMethodId = shippingMethod
                        });
                    }
                }
            }

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Delsami.Plugin.OrderFulfillment.DeliveryScheduling.SuccessNotification"));

            return RedirectToAction("List", new { storeId = storeId });
        }

        public virtual async Task<IActionResult> DeleteAll(int? storeId)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            storeId ??= (await _storeContext.GetCurrentStoreAsync()).Id;

            var availableDeliveryDateTimeRangs = await _availableDeliveryDateTimeRangeService.GetAvailableDeliveryDateTimeRangesAsync(isActive: true, storeId: storeId);

            foreach (var availableDeliveryDateTimeRang in availableDeliveryDateTimeRangs)
            {
                availableDeliveryDateTimeRang.EndDateOnUtc = DateTime.Now.AddDays(-1).Date;

                await _availableDeliveryDateTimeRangeService.UpdateAvailableDeliveryDateTimeRangeAsync(availableDeliveryDateTimeRang);
            }

            var availableDeliveryTimeRanges = await _availableDeliveryTimeRangeService.GetAvailableDeliveryTimeRangesAsync(storeId);
            var availableDeliveryDateTimeRanges = await _availableDeliveryDateTimeRangeService.GetAvailableDeliveryDateTimeRangesAsync(availableDeliveryTimeRangeIds: availableDeliveryTimeRanges.Select(t => t.Id).ToList());

            foreach (var addtr in availableDeliveryDateTimeRanges.Where(w => w.ExceptionDateId.HasValue))
            {
                var exceptionDate = await _exceptionDateService.GetExceptionDateByIdAsync(addtr.ExceptionDateId.Value);
                if (exceptionDate == null)
                    throw new NullReferenceException(nameof(exceptionDate));

                if (exceptionDate.IsCustomTimeRange) continue;

                await _exceptionDateService.DeleteExceptionDateAsync(exceptionDate);
            }

            foreach (var availableDeliveryTimeRange in availableDeliveryTimeRanges)
                await _availableDeliveryTimeRangeService.DeleteAvailableDeliveryTimeRangeAsync(availableDeliveryTimeRange);


            var customExceptionsDateTimeIds = (await _availableDeliveryDateTimeRangeService.GetForCustomExceptionsAsync(storeId.Value))
                                                                                        .Select(a=>a.Id)
                                                                                        .ToList();

            await _shippingMethodCapacityService.DeleteAllNotCustomExceptionAsync(customExceptionsDateTimeIds);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Delsami.Plugin.OrderFulfillment.DeliveryScheduling.SuccessNotification"));


            return RedirectToAction("List", new { storeId = storeId });
        }

        #endregion

        #region Utilties

        [NonAction]
        protected virtual async Task UpdateLocalesAsync(AvailableDeliveryTimeRange availableDeliveryTimeRange, AvailableDeliveryTimeRangeModel model)
        {
            foreach (var localized in model.Locales)
            {
                await _localizedEntityService.SaveLocalizedValueAsync(availableDeliveryTimeRange,
                                                               x => x.Time,
                                                               localized.Time,
                                                               localized.LanguageId);
            }
        }

        private IList<SelectListItem> PrepareDayOfWeek()
        {
            var days = new List<SelectListItem>();
            for (int i = 1; i < dayNames.Count + 1; i++)
            {
                var item = new SelectListItem
                {

                    Value = i.ToString(),
                    Text = dayNames[i - 1],
                };
                days.Add(item);
            }
            return days;
        }

        private async Task<IList<SelectListItem>> GetAvailableShippingMethods()
        {
            var shippingMethods = await _shippingService.GetAllShippingMethodsAsync();
            shippingMethods = shippingMethods.Where(w => !_deliverySchedulingSetting.DisabledShippingMethodIdsAsInt.Contains(w.Id)).ToList();

            var shippingMethodsItems = shippingMethods.Select(a => new SelectListItem()
            {
                Text = a.Name,
                Value = a.Id.ToString(),
            }).ToList();

            return shippingMethodsItems;

        }

        #endregion
    }
}