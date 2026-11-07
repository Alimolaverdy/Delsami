using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Areas.Admin.Models;
using Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Domain;
using Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Services;
using Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Settings;
using Nop.Core;
using Nop.Services.Configuration;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Services.Shipping;
using Nop.Services.Stores;
using Nop.Web.Areas.Admin.Controllers;
using Nop.Web.Framework.Models.Extensions;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;
using NUglify.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Delsami.Plugin.OrderFulfillment.DeliveryScheduling.Areas.Admin.Controllers;

public class ExceptionDateAdminController : BaseAdminController
{
    private readonly IPermissionService _permissionService;
    private readonly IStoreContext _storeContext;
    private readonly IShippingService _shippingService;
    private readonly IAvailableDeliveryDateTimeRangeService _availableDeliveryDateTimeRangeService;
    private readonly IExceptionDateService _exceptionDateService;
    private readonly IStoreService _storeService;
    private readonly ILanguageService _languageService;
    private readonly IAvailableDeliveryTimeRangeService _availableDeliveryTimeRangeService;
    private readonly ILocalizedEntityService _localizedEntityService;
    private readonly IShippingMethodCapacityService _shippingMethodCapacityService;
    private readonly IDateTimeHelper _dateTimeHelper;
    private readonly ILocalizationService _localizationService;
    private readonly DeliverySchedulingSetting _deliverySchedulingSetting;
    private readonly INotificationService _notificationService;

    public ExceptionDateAdminController(
        IPermissionService permissionService,
        IStoreContext storeContext,
        IShippingService shippingService,
        IAvailableDeliveryDateTimeRangeService availableDeliveryDateTimeRangeService,
        IExceptionDateService exceptionDateService,
        IStoreService storeService,
        ILanguageService languageService,
        IAvailableDeliveryTimeRangeService availableDeliveryTimeRangeService,
        ILocalizedEntityService localizedEntityService,
        IShippingMethodCapacityService shippingMethodCapacityService,
        IDateTimeHelper dateTimeHelper,
        ILocalizationService localizationService,
        DeliverySchedulingSetting deliverySchedulingSetting,
        ISettingService settingService,
        INotificationService notificationService)
    {
        _permissionService = permissionService;
        _storeContext = storeContext;
        _shippingService = shippingService;
        _availableDeliveryDateTimeRangeService = availableDeliveryDateTimeRangeService;
        _exceptionDateService = exceptionDateService;
        _storeService = storeService;
        _languageService = languageService;
        _availableDeliveryTimeRangeService = availableDeliveryTimeRangeService;
        _localizedEntityService = localizedEntityService;
        _shippingMethodCapacityService = shippingMethodCapacityService;
        _dateTimeHelper = dateTimeHelper;
        _localizationService = localizationService;
        _deliverySchedulingSetting = deliverySchedulingSetting;
        _notificationService = notificationService;
    }

    [NonAction]
    protected virtual async Task UpdateLocalesAsync(ExceptionDate exceptionDate, ExceptionDateModel model)
    {
        foreach (var localized in model.Locales)
        {
            await _localizedEntityService.SaveLocalizedValueAsync(exceptionDate,
                                                           x => x.Title,
                                                           localized.Title,
                                                           localized.LanguageId);
        }
    }
    [NonAction]
    protected virtual async Task UpdateTimeLocalesAsync(AvailableDeliveryTimeRange availableDeliveryTimeRange, AvailableDeliveryTimeRangeModel model)
    {
        foreach (var localized in model.Locales)
        {
            await _localizedEntityService.SaveLocalizedValueAsync(availableDeliveryTimeRange,
                                                           x => x.Time,
                                                           localized.Time,
                                                           localized.LanguageId);
        }
    }

    public virtual async Task<IActionResult> List(int? storeId)
    {
        if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
            return AccessDeniedView();

        if (!storeId.HasValue)
            storeId = (await _storeContext.GetCurrentStoreAsync()).Id;

        var model = new ExceptionDateListModel()
        {
            StoreId = storeId,
            HasAvailableDeliveryTimeRange =await HasAvailableDeliveryTimeRange(storeId.Value)
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
                Selected = s.Id == storeId
            }).ToListAsync());
            model.AvailableStores = availableStores;
        }

        return View("~/Plugins/Delsami.OrderFulfillment.DeliveryScheduling/Areas/Admin/Views/ExceptionDateAdmin/List.cshtml", model);
    }

    [HttpPost]
    public virtual async Task<IActionResult> List(ExceptionDateSearchModel searchModel)
    {
        if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
            return AccessDeniedView();

        var exceptionDates = await _exceptionDateService.GetExceptionDatesAsync(searchModel.StoreId, searchModel.Page - 1, searchModel.PageSize);

        var model = await new ExceptionDateListPartialModel().PrepareToGridAsync(searchModel, exceptionDates, () =>
        {
            return exceptionDates.SelectAwait(async x =>
            {
                var m = new ExceptionDateModel
                {
                    DateOnUtc = await _dateTimeHelper.ConvertToUserTimeAsync(x.DateOnUtc, DateTimeKind.Utc),
                    Title = await _localizationService.GetLocalizedAsync(x, d => d.Title),
                    Id = x.Id
                };
                return m;
            });
        });

        return Json(model);
    }
    [HttpPost]
    public virtual async Task<IActionResult> ExceptionTimeRangeList(AvailableDeliveryTimeRangeSearchModel searchModel)
    {
        if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
            return AccessDeniedView();
        if (searchModel.StoreId == 0)
            searchModel.StoreId = (await _storeContext.GetCurrentStoreAsync()).Id;

        var exceptionTimeRanges = await _availableDeliveryTimeRangeService.GetTimeRangeByExceptionDateIdAsync(searchModel.ExceptionDateId.Value, searchModel.StoreId);
        var shippingMethods = await _shippingService.GetAllShippingMethodsAsync();

        var model = await new AvailableDeliveryTimeRangeListPartialModel().PrepareToGridAsync(searchModel, exceptionTimeRanges, () =>
        {
            return exceptionTimeRanges.SelectAwait(async x =>
            {
                var m = new AvailableDeliveryTimeRangeModel
                {
                    IsConfirmed = (await _availableDeliveryDateTimeRangeService.GetAvailableDeliveryDateTimeRangesAsync(availableDeliveryTimeRangeIds: new List<int> { x.Id }, justExceptionsDateTimeRange: searchModel.IsCustomTimeRange)).Any(),
                    EndHour = x.EndHour,
                    StartHour = x.StartHour,
                    ShippingMethodIds = x.ShippingMethodIds,
                    ShippingMethodTexts = string.Join(" - ", shippingMethods.Where(a => x.ShippingMethodIds.Contains(a.Id)).Select(a => a.Name).ToList()),
                    Time = x.Time,
                    Id = x.Id
                };
                return m;
            });
        });
        return Json(model);
    }

    [HttpPost]
    public virtual async Task<IActionResult> ConfirmingTimeRanges(int exceptionDateId, int? storeId = null)
    {
        if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
            return AccessDeniedView();

        storeId ??= (await _storeContext.GetCurrentStoreAsync()).Id;

        var availablityDeliveryTimeRanges = await _availableDeliveryTimeRangeService.GetTimeRangeByExceptionDateIdAsync(exceptionDateId, storeId.Value);
        if (availablityDeliveryTimeRanges?.Count() == 0)
        {
            _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Delsami.Plugin.OrderFulfillment.DeliveryScheduling.YouCouldntAddExceptionDateWithoutTimeRange"));
            return Json(new { Result = false });
        }

        var endDate = await _availableDeliveryDateTimeRangeService.GetLatestEndDateAsync(storeId.Value);

        var availablityTimeRanges = await availablityDeliveryTimeRanges.WhereAwait(async w => !(await _availableDeliveryDateTimeRangeService.GetAvailableDeliveryDateTimeRangesAsync(availableDeliveryTimeRangeIds: new List<int> { w.Id }, justExceptionsDateTimeRange: true)).Any()).ToListAsync();

        foreach (var availablityTimeRange in availablityTimeRanges)
        {


            var availableDeliveryDateTimeRange = new AvailableDeliveryDateTimeRange
            {
                AvailableDeliveryTimeRangeId = availablityTimeRange.Id,
                StartDateOnUtc = endDate != DateTime.MinValue ? endDate.AddDays(1).Date : DateTime.Now.Date,
                ExceptionDateId = exceptionDateId,
            };
            await _availableDeliveryDateTimeRangeService.InsertAvailableDeliveryDateTimeRangeAsync(availableDeliveryDateTimeRange);
            foreach (var shippingMethod in availablityTimeRange.ShippingMethodIds)
            {
                await _shippingMethodCapacityService.InsertShippingMethodCapacityAsync(new ShippingMethodCapacity
                {
                    AvailableDeliveryDateTimeRangeId = availableDeliveryDateTimeRange.Id,
                    Capacity = 0,
                    ShippingMethodId = shippingMethod
                });
            }

        }

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Delsami.Plugin.OrderFulfillment.DeliveryScheduling.SuccessNotification"));

        return Json(new { Result = true });
    }

    public virtual async Task<IActionResult> Create(int? storeId)
    {
        if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
            return AccessDeniedView();

        if (!storeId.HasValue)
            storeId = (await _storeContext.GetCurrentStoreAsync()).Id;

        var stores = await _storeService.GetAllStoresAsync();
        var model = new ExceptionDateModel()
        {
            StoreId = storeId,
            AvailableShippingMethods = await GetAvailableShippingMethods(),
            DateOnUtc = DateTime.UtcNow,
        };

        await AddLocalesAsync(_languageService, model.Locales);

        return View("~/Plugins/Delsami.OrderFulfillment.DeliveryScheduling/Areas/Admin/Views/ExceptionDateAdmin/Create.cshtml", model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    public virtual async Task<IActionResult> Create(ExceptionDateModel model, bool continueEditing)
    {
        if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
            return AccessDeniedView();

        if (!model.StoreId.HasValue)
            model.StoreId = (await _storeContext.GetCurrentStoreAsync()).Id;

        if (ModelState.IsValid)
        {
            if (await _exceptionDateService.GetExceptionDateAsync(model.DateOnUtc, model.StoreId.Value) == null)
            {
                var entity = new ExceptionDate
                {
                    Title = model.Title,
                    DateOnUtc = model.DateOnUtc.Date,
                    StoreId = model.StoreId.Value,
                    IsCustomTimeRange = model.IsCustomTimeRange,

                };

                await _exceptionDateService.InsertExceptionDateAsync(entity);

                await UpdateLocalesAsync(entity, model);


                if (!model.IsCustomTimeRange)
                {

                    var allTimeRanges = await _availableDeliveryTimeRangeService.GetAvailableDeliveryTimeRangesAsync(entity.StoreId);

                    string date = ((int)model.DateOnUtc.DayOfWeek).ToString();
                    var allValidTimeRanges = allTimeRanges.Where(a => a.DaysOfWeek.Contains(date));

                    foreach (var timeRange in allValidTimeRanges)
                    {
                        var availableDeliveryDateTimeRange = new AvailableDeliveryDateTimeRange
                        {
                            ExceptionDateId = entity.Id,
                            AvailableDeliveryTimeRangeId = timeRange.Id,
                            StartDateOnUtc = DateTime.UtcNow.Date
                        };

                        await _availableDeliveryDateTimeRangeService.InsertAvailableDeliveryDateTimeRangeAsync(availableDeliveryDateTimeRange);

                        var shippingMethodIds = timeRange.ShippingMethodIds.Split("-").Select(int.Parse);

                        foreach (var shippingMethodId in shippingMethodIds)
                        {
                            await _shippingMethodCapacityService.InsertShippingMethodCapacityAsync(new ShippingMethodCapacity
                            {
                                AvailableDeliveryDateTimeRangeId = availableDeliveryDateTimeRange.Id,
                                Capacity = 0,
                                ShippingMethodId = shippingMethodId,
                            });
                        }
                    }
                }

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Delsami.Plugin.OrderFulfillment.DeliveryScheduling.SuccessNotification"));

                if (continueEditing)
                {
                    //selected tab
                    SaveSelectedTabName();

                    return RedirectToAction("Edit", new { storeId = model.StoreId, id = entity.Id });
                }
                return RedirectToAction("list", new { storeId = model.StoreId });
            }
            else
            {
                ModelState.AddModelError("", await _localizationService.GetResourceAsync("Delsami.Plugin.OrderFulfillment.DeliveryScheduling.ExceptionDate.SameDateError"));
            }
        }

        model.AvailableShippingMethods = await GetAvailableShippingMethods();
        return View("~/Plugins/Delsami.OrderFulfillment.DeliveryScheduling/Areas/Admin/Views/ExceptionDateAdmin/Create.cshtml", model);
    }

    public virtual async Task<IActionResult> Edit(int id, int? storeId = null, int? shippingMethodId = null)
    {
        if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
            return AccessDeniedView();

        if (!storeId.HasValue)
            storeId = (await _storeContext.GetCurrentStoreAsync()).Id;

        var exceptionDate = await _exceptionDateService.GetExceptionDateByIdAsync(id);
        var stores = await _storeService.GetAllStoresAsync();

        var model = new ExceptionDateModel()
        {
            Title = exceptionDate.Title,
            StoreId = storeId,
            Id = id,
            ExceptionTimeRange = exceptionDate.IsCustomTimeRange ? new() : null,
            IsCustomTimeRange = exceptionDate.IsCustomTimeRange,
            SelectedShippingMethodId = shippingMethodId ?? 0,
            DateOnUtc = exceptionDate.DateOnUtc,
            AvailableStores = await (await _storeService.GetAllStoresAsync()).SelectAwait(async s => new SelectListItem
            {
                Text = await _localizationService.GetLocalizedAsync(s, d => d.Name),
                Value = s.Id.ToString()
            }).ToListAsync()
        };

        var shippingMethods = await GetAvailableShippingMethods();

        if (exceptionDate.IsCustomTimeRange)
            model.ExceptionTimeRange.AvailableShippingMethod = shippingMethods;


        var items = new List<SelectListItem>
        {
            new SelectListItem
            {
                Text = await _localizationService.GetResourceAsync("Delsami.Plugin.OrderFulfillment.DeliveryScheduling.DefaultSelect"),
                Value = ""
            }
        };
        items.AddRange(shippingMethods);

        model.AvailableShippingMethods = items;

        await AddLocalesAsync(_languageService, model.Locales, async (locale, languageId) =>
        {
            locale.Title = await _localizationService.GetLocalizedAsync(exceptionDate, x => x.Title, languageId, false, false);
        });

        return View("~/Plugins/Delsami.OrderFulfillment.DeliveryScheduling/Areas/Admin/Views/ExceptionDateAdmin/Edit.cshtml", model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    public virtual async Task<IActionResult> Edit(ExceptionDateModel model, bool continueEditing)
    {
        if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
            return AccessDeniedView();

        if (!model.StoreId.HasValue)
            model.StoreId = (await _storeContext.GetCurrentStoreAsync()).Id;

        var stores = await _storeService.GetAllStoresAsync();

        if (ModelState.IsValid)
        {
            var exceptionDate = await _exceptionDateService.GetExceptionDateByIdAsync(model.Id);
            exceptionDate.Title = model.Title;

            await _exceptionDateService.UpdateExceptionDateAsync(exceptionDate);

            await UpdateLocalesAsync(exceptionDate, model);

            if (model.TimeRanges != null)
            {
                foreach (var timeRange in model.TimeRanges)
                {
                    var shippingMethodCapacity = await _shippingMethodCapacityService.GetShippingMethodCapacityByIdAsync(timeRange.ShippingMethodCapacityId);
                    shippingMethodCapacity.Capacity = timeRange.Capacity;

                    await _shippingMethodCapacityService.UpdateShippingMethodCapacityAsync(shippingMethodCapacity);
                }
            }

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Delsami.Plugin.OrderFulfillment.DeliveryScheduling.SuccessNotification"));

            if (continueEditing)
            {
                //selected tab
                SaveSelectedTabName();
                return RedirectToAction("Edit", new { id = model.Id, shippingMethodId = model.SelectedShippingMethodId, storeId = model.StoreId });
            }
            return RedirectToAction("List", new { storeId = model.StoreId });
        }
        else
        {
            SaveSelectedTabName();

            model.AvailableStores = await stores.SelectAwait(async s => new SelectListItem
            {
                Text = await _localizationService.GetLocalizedAsync(s, d => d.Name),
                Value = s.Id.ToString()
            }).ToListAsync();

            var items = new List<SelectListItem> { new SelectListItem { Text = await _localizationService.GetResourceAsync("Delsami.Plugin.OrderFulfillment.DeliveryScheduling.DefaultSelect"), Value = "" } };

            items.AddRange(await GetAvailableShippingMethods());

            model.AvailableShippingMethods = items;

            return View("~/Plugins/Delsami.OrderFulfillment.DeliveryScheduling/Areas/Admin/Views/ExceptionDateAdmin/Edit.cshtml", model);
        }
    }

    public virtual async Task<IActionResult> TimeTableList(int exceptionDateId, int shippingMethodId, bool isCustomTimeRange, string selectedDateStr = null)
    {
        if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
            return AccessDeniedView();

        var exceptionDate = await _exceptionDateService.GetExceptionDateByIdAsync(exceptionDateId);


        var availableDeliveryTimeRanges = (await _availableDeliveryTimeRangeService.GetAvailableDeliveryTimeRangesAsync(exceptionDate.StoreId, justExceptionsTimeRange: isCustomTimeRange))
                                                                                   .Where(a => a.ShippingMethodIds.Contains(shippingMethodId.ToString()));
        DateTime? selectedDate = null;

        if (!string.IsNullOrWhiteSpace(selectedDateStr))
            selectedDate = Delsami.Plugin.Misc.Core.Helpers.DateTimeHelper.GetDateTimeFromPersian(WebUtility.UrlDecode(selectedDateStr));

        if (selectedDate.HasValue && !isCustomTimeRange)
        {

            string date = ((int)selectedDate.Value.DayOfWeek).ToString();
            availableDeliveryTimeRanges = availableDeliveryTimeRanges.Where(a => a.DaysOfWeek.Contains(date));
        }
        else
        {
            availableDeliveryTimeRanges = availableDeliveryTimeRanges.Where(a => a.ExceptionDateId == exceptionDateId);

        }

        var model = new ExceptionDateModel();

        foreach (var timeRange in availableDeliveryTimeRanges)
        {
            if (model.AvailableDeliveryTimeRanges.Count() != availableDeliveryTimeRanges.Count())
            {
                model.AvailableDeliveryTimeRanges.Add(new AvailableDeliveryTimeRangeModel
                {
                    Time = await _localizationService.GetLocalizedAsync(timeRange, d => d.Time),
                    StartHour = timeRange.StartHour,
                    EndHour = timeRange.EndHour,
                    Id = timeRange.Id,
                    DisplayOrder = timeRange.DisplayOrder,
                });
            }

            var availableDeliveryDateTimeRanges = await _availableDeliveryDateTimeRangeService.GetAvailableDeliveryDateTimeRangesAsync(availableDeliveryTimeRangeIds: new List<int> { timeRange.Id }, justExceptionsDateTimeRange: isCustomTimeRange);

            var availableDeliveryDateTimeRange = availableDeliveryDateTimeRanges.Where(w => w.ExceptionDateId == exceptionDateId).FirstOrDefault();
            if (availableDeliveryDateTimeRange != null)
            {

                var shippingMethodCapacities = await _shippingMethodCapacityService.GetShippingMethodCapacitiesAsync(shippingMethodId: shippingMethodId, availableDeliveryDateTimeRangeId: availableDeliveryDateTimeRange.Id);

                foreach (var shippingMethodCapacity in shippingMethodCapacities)
                {
                    model.TimeRanges.Add(new TimeRangeModel
                    {
                        ShippingMethodCapacityId = shippingMethodCapacity.Id,
                        Capacity = shippingMethodCapacity.Capacity
                    });
                }
            }
        }

        model.SelectedShippingMethodId = shippingMethodId;
        model.StoreId = exceptionDate.StoreId;

        return View("~/Plugins/Delsami.OrderFulfillment.DeliveryScheduling/Areas/Admin/Views/ExceptionDateAdmin/TimeTableList.cshtml", model);
    }

    public virtual async Task<IActionResult> Delete(int id)
    {
        if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
            return AccessDeniedView();

        var exceptionDate = await _exceptionDateService.GetExceptionDateByIdAsync(id);
        await _exceptionDateService.DeleteExceptionDateAsync(exceptionDate);

        var dateTimeRangelist = await _availableDeliveryDateTimeRangeService.GetByExceptionId(id);
        if (dateTimeRangelist != null && dateTimeRangelist.Any())
            await _shippingMethodCapacityService.DeleteByDateTimeIdsAsync(dateTimeRangelist.Select(a => a.Id).ToList());

        dateTimeRangelist.ForEach(a => a.EndDateOnUtc = DateTime.Now.AddDays(-1).Date);
        await _availableDeliveryDateTimeRangeService.UpdateAvailableDeliveryDateTimeRangesAsync(dateTimeRangelist);


        if (exceptionDate.IsCustomTimeRange)
        {
            var customExcetionTimeRanges = await _availableDeliveryTimeRangeService.GetTimeRangeByExceptionDateIdAsync(exceptionDate.Id, (await _storeContext.GetCurrentStoreAsync()).Id);
            if (customExcetionTimeRanges != null && customExcetionTimeRanges.Any())
            {
                var timeRangelist = await _availableDeliveryTimeRangeService.GetAllByIdsAsync(customExcetionTimeRanges.Select(a => a.Id).ToList());
                ; foreach (var item in timeRangelist)
                {
                    item.Deleted = true;
                    await _availableDeliveryTimeRangeService.DeleteAvailableDeliveryTimeRangeAsync(item);
                }
            }
        }


        return new NullJsonResult();
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


    private async Task<bool> HasAvailableDeliveryTimeRange(int storeId)
    {
        return (await (await _availableDeliveryTimeRangeService.GetAvailableDeliveryTimeRangesAsync(storeId,justExceptionsTimeRange:true))
                                                               .AnyAwaitAsync(async a => (await _availableDeliveryDateTimeRangeService.GetAvailableDeliveryDateTimeRangesAsync(availableDeliveryTimeRangeIds: new List<int> { a.Id }, justExceptionsDateTimeRange: true))
                                                               .Any())) ||

               (await (await _availableDeliveryTimeRangeService.GetAvailableDeliveryTimeRangesAsync(storeId))
                                                               .AnyAwaitAsync(async a => (await _availableDeliveryDateTimeRangeService.GetAvailableDeliveryDateTimeRangesAsync(availableDeliveryTimeRangeIds: new List<int> { a.Id }))
                                                               .Any()));
    }
}
