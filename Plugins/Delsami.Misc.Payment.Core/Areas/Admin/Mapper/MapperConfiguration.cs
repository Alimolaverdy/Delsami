using AutoMapper;
using Delsami.Plugin.Payment.Core.Areas.Admin.Models;
using Nop.Core.Infrastructure.Mapper;

namespace Delsami.Plugin.Payment.Core.Areas.Admin.Mapper
{
    public class MapperConfiguration : Profile, IOrderedMapperProfile
    {

        public MapperConfiguration()
        {
            CreatePaymentMap();
        }

        private void CreatePaymentMap()
        {
            CreateMap<Domain.Payment, PaymentModel>()
                .ReverseMap();
        }

        public int Order => 1;

    }
}
