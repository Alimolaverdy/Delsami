using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Services.Localization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Delsami.Plugin.Misc.Core.Helpers
{
    public class EnumHelper : IEnumHelper
    {
        #region Fields

        private readonly ILocalizationService _localizationService;


        #endregion

        #region Ctor

        public EnumHelper(ILocalizationService localizationService)
        {
            _localizationService = localizationService;
        }

        #endregion

        #region Methods

        public async Task<string> GetDescriptionAsync(Enum value)
        {
            FieldInfo field = value.GetType().GetField(value.ToString());

            if (field != null)
            {
                DescriptionAttribute attribute = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
                if (attribute != null)
                {
                    var descrption = attribute.Description;

                    return await _localizationService.GetResourceAsync(descrption);
                }
            }

            // If no description is found, return the enum name itself
            return value.ToString();
        }


        public static List<SelectListItem> ToSelectList<TEnum>() where TEnum : struct, Enum
        {
            return Enum.GetValues(typeof(TEnum))
                       .Cast<TEnum>()
                       .Select(e => new SelectListItem
                       {
                           Value = Convert.ToInt32(e).ToString(),
                           Text = GetEnumDescription(e)
                       })
                       .ToList();
        }

        private static string GetEnumDescription<TEnum>(TEnum value)
        {
            FieldInfo fi = value!.GetType().GetField(value.ToString()!);

            if (fi != null)
            {
                DescriptionAttribute attribute = fi.GetCustomAttribute<DescriptionAttribute>();
                if (attribute != null)
                {
                    return attribute.Description;
                }
            }

            return value.ToString() ?? string.Empty;
        }
        #endregion
    }
}