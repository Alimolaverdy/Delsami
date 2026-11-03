using Nop.Core.Caching;

namespace Delsami.Plugin.Misc.Core.Infrastructure.Cache
{
    public static partial class DelsamiModelCacheDefaults
    {
        public static CacheKey SpecificaionAttributeOptionPictureModelKey => new("Nop.pres.specificationattributeoption.picture-{0}-{1}-{2}-{3}-{4}-{5}", SpecificaionAttributeOptionPicturePrefixCacheKey, SpecificaionAttributeOptionPicturePrefixCacheKeyById);
        public static string SpecificaionAttributeOptionPicturePrefixCacheKey => "Nop.pres.specificationattributeoption.picture";
        public static string SpecificaionAttributeOptionPicturePrefixCacheKeyById => "Nop.pres.specificationattributeoption.picture-{0}-";
        public static CacheKey CheckLogisticIsActiveCacheKey => new("Nop.pres.check.logisctic.active-{0}-{1}-{2}", CheckLogisticIsActivePrefixCacheKey);
        public static string CheckLogisticIsActivePrefixCacheKey => "Nop.pres.check.logisctic.active";
    }
}