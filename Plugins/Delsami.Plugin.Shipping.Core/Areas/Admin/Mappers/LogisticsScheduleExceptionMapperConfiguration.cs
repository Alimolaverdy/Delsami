using AutoMapper;
using Delsami.Plugin.Shipping.Core.Areas.Admin.Models.LogisticsSchedulesException;
using Delsami.Plugin.Shipping.Core.Domain;
using Nop.Core.Infrastructure.Mapper;
using System;

namespace Delsami.Plugin.Shipping.Core.Areas.Admin.Mappers
{
    public class LogisticsScheduleExceptionMapperConfiguration : Profile, IOrderedMapperProfile
    {
        public LogisticsScheduleExceptionMapperConfiguration()
        {
            CreateLogisticsScheduleMap();
        }

        private void CreateLogisticsScheduleMap()
        {
            CreateMap<LogisticsScheduleException, LogisticsScheduleExceptionModel>()
                .ForMember(dest => dest.StartTimeStr, cfg => cfg.MapFrom(src => src.StartTime.ToString()))
                .ForMember(dest => dest.EndTimeStr, cfg => cfg.MapFrom(src => src.EndTime.ToString()));

            CreateMap<LogisticsScheduleExceptionModel, LogisticsScheduleException>()
                .ForMember(dest => dest.StartTime, cfg => cfg.MapFrom(src => TimeSpan.Parse(src.StartTimeStr)))
                .ForMember(dest => dest.EndTime, cfg => cfg.MapFrom(src => TimeSpan.Parse(src.EndTimeStr)));
               
        }

        public int Order => 1;

    }
}
