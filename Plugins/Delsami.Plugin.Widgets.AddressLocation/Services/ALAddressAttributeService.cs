using System;
using System.Linq;
using System.Threading.Tasks;
using Delsami.Plugin.Widgets.AddressLocation.Infrastructure;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Data;
using Nop.Services.Common;

namespace Delsami.Plugin.Widgets.AddressLocation.Services
{
    public class ALAddressAttributeService : AddressAttributeService, IALAddressAttributeService
    {
        #region Fields

        private readonly IRepository<AddressAttribute> _addressAttributeRepository;
        private readonly IRepository<AddressAttributeValue> _addressAttributeValueRepository;
        private readonly IStaticCacheManager _staticCacheManager;

        #endregion

        #region Ctor

        public ALAddressAttributeService(IRepository<AddressAttribute> addressAttributeRepository,
            IRepository<AddressAttributeValue> addressAttributeValueRepository,
            IStaticCacheManager staticCacheManager) : base(addressAttributeRepository, addressAttributeValueRepository, staticCacheManager)
        {
            _addressAttributeRepository = addressAttributeRepository;
            _addressAttributeValueRepository = addressAttributeValueRepository;
            _staticCacheManager = staticCacheManager;
        }

        public async Task<AddressAttribute> GetAddressAttributeByNameAsync(string name)
        {
            var addressAttribute =await _addressAttributeRepository.Table.Where(w => w.Name.ToLower() == name.ToLower()).FirstOrDefaultAsync();

            return addressAttribute;
        }

        public async Task InsertLatAddressAttribute()
        {
            var latAddressAttribute = new AddressAttribute
            {
                Name = AddressLocationConstants.LATITUDE_NAME,
                AttributeControlType = AttributeControlType.TextBox,
                AttributeControlTypeId = (int)AttributeControlType.TextBox,
                IsRequired = true,
                DisplayOrder = int.MaxValue
            };

            await _addressAttributeRepository.InsertAsync(latAddressAttribute);
        }

        public async Task InsertLngAddressAttribute()
        {
            var lngAddressAttribute = new AddressAttribute
            {
                Name = AddressLocationConstants.LONGITUDE_NAME,
                AttributeControlType = AttributeControlType.TextBox,
                AttributeControlTypeId = (int)AttributeControlType.TextBox,
                IsRequired = true,
                DisplayOrder = int.MaxValue
            };

            await _addressAttributeRepository.InsertAsync(lngAddressAttribute);
        }

        #endregion
    }
}
