using AutoMapper;
using Delsami.Plugin.Shipping.Core.Areas.Admin.Models.Cartons;
using Delsami.Plugin.Shipping.Core.Domain;
using Nop.Core.Infrastructure.Mapper;

namespace Delsami.Plugin.Shipping.Core.Areas.Admin.Mappers
{
    public class CartonMapperConfiguration : Profile, IOrderedMapperProfile
    {
        public CartonMapperConfiguration()
        {
            CreateCustomerCarMap();
        }

        private void CreateCustomerCarMap()
        {
            CreateMap<Carton, CartonModel>()
                .ReverseMap();
        }

        public int Order => 1;

    }
}
