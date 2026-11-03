using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Services.Common;
using Nop.Services.Html;
using Nop.Services.Localization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Delsami.Plugin.Widgets.AddressLocation.Services
{
    public class CustomAddressAttributeFormatter : AddressAttributeFormatter, IAddressAttributeFormatter
    {
        #region Fields

        private readonly IWorkContext _workContext;
        private readonly IAddressAttributeParser _addressAttributeParser;
        private readonly ILocalizationService _localizationService;
        private readonly IHtmlFormatter _htmlFormatter;
        private readonly IAddressAttributeService _addressAttributeService;
        private readonly AddressLocationSettings _addressLocationSettings;

        #endregion

        #region Ctor

        public CustomAddressAttributeFormatter(IAddressAttributeParser addressAttributeParser,
                                               IAddressAttributeService addressAttributeService,
                                               IHtmlFormatter htmlFormatter,
                                               ILocalizationService localizationService,
                                               IWorkContext workContext,
                                               AddressLocationSettings addressLocationSettings) : base(addressAttributeParser,
                                                                                                       addressAttributeService,
                                                                                                       htmlFormatter,
                                                                                                       localizationService,
                                                                                                       workContext)
        {
            _workContext = workContext;
            _addressAttributeParser = addressAttributeParser;
            _localizationService = localizationService;
            _htmlFormatter = htmlFormatter;
            _addressAttributeService = addressAttributeService;
            _addressLocationSettings = addressLocationSettings;
        }

        #endregion

        #region Methods

        public override async Task<string> FormatAttributesAsync(string attributesXml, string separator = "<br />", bool htmlEncode = true)
        {
            var result = new StringBuilder();
            var currentLanguage = await _workContext.GetWorkingLanguageAsync();
            var attributes = (await _addressAttributeParser.ParseAddressAttributesAsync(attributesXml))
                                        .ToList();

            for (var i = 0; i < attributes.Count; i++)
            {
                var attribute = attributes[i];
                var valuesStr = _addressAttributeParser.ParseValues(attributesXml, attribute.Id);
                for (var j = 0; j < valuesStr.Count; j++)
                {
                    var valueStr = valuesStr[j];
                    var formattedAttribute = string.Empty;
                    if (!attribute.ShouldHaveValues())
                    {
                        //no values
                        if (attribute.AttributeControlType == AttributeControlType.MultilineTextbox)
                        {
                            //multiline textbox
                            var attributeName = await _localizationService.GetLocalizedAsync(attribute, a => a.Name, currentLanguage.Id);
                            //encode (if required)
                            if (htmlEncode)
                                attributeName = WebUtility.HtmlEncode(attributeName);
                            formattedAttribute = $"{attributeName}: {_htmlFormatter.FormatText(valueStr, false, true, false, false, false, false)}";
                            //we never encode multiline textbox input
                        }
                        else if (attribute.AttributeControlType == AttributeControlType.FileUpload)
                        {
                            //file upload
                            //not supported for address attributes
                        }
                        else
                        {
                            //other attributes (textbox, datepicker)
                            formattedAttribute = $"{await _localizationService.GetLocalizedAsync(attribute, a => a.Name, currentLanguage.Id)}: {valueStr}";
                            //encode (if required)
                            if (htmlEncode)
                                formattedAttribute = WebUtility.HtmlEncode(formattedAttribute);
                        }
                    }
                    else
                    {
                        if (int.TryParse(valueStr, out var attributeValueId))
                        {
                            var attributeValue = await _addressAttributeService.GetAddressAttributeValueByIdAsync(attributeValueId);
                            if (attributeValue != null)
                            {
                                formattedAttribute = $"{await _localizationService.GetLocalizedAsync(attribute, a => a.Name, currentLanguage.Id)}: {await _localizationService.GetLocalizedAsync(attributeValue, a => a.Name, currentLanguage.Id)}";
                            }
                            //encode (if required)
                            if (htmlEncode)
                                formattedAttribute = WebUtility.HtmlEncode(formattedAttribute);
                        }
                    }

                    if (string.IsNullOrEmpty(formattedAttribute))
                        continue;

                    if (i != 0 || j != 0)
                        result.Append(separator);

                    if (attributes[i].Id == _addressLocationSettings.LongitudeAddressAttributeId)
                        formattedAttribute = $"<span id=\"longitude-value\" style=\"display:none\">{formattedAttribute}</span>";

                    if (attributes[i].Id == _addressLocationSettings.LatitudeAddressAttributeId)
                        formattedAttribute = $"<span id=\"Latitude-value\" style=\"display:none\">{formattedAttribute}</span>";

                    if (attributes[i].Id == _addressLocationSettings.DigiKalaAddressAttributeId)
                        formattedAttribute = $"<span id=\"digiKalaAddressId-value\" style=\"display:none\">{formattedAttribute}</span>";


                    result.Append(formattedAttribute);
                }
            }

            return result.ToString();
        }

        #endregion
    }
}
