using AutoMapper;
using Delsami.Plugin.Shipping.Core.Areas.Admin.Models.LogisticsSchedules;
using Delsami.Plugin.Shipping.Core.Domain;
using Nop.Core.Infrastructure.Mapper;
using System;

namespace Delsami.Plugin.Shipping.Core.Areas.Admin.Mappers
{
    public class LogisticsScheduleMapperConfiguration : Profile, IOrderedMapperProfile
    {
        public LogisticsScheduleMapperConfiguration()
        {
            CreateLogisticsScheduleMap();
        }

        private void CreateLogisticsScheduleMap()
        {
            CreateMap<LogisticsSchedule, LogisticsScheduleModel>()
                .ForMember(dest => dest.StartTimeStr, cfg => cfg.MapFrom(src => src.StartTime.ToString()))
                .ForMember(dest => dest.EndTimeStr, cfg => cfg.MapFrom(src => src.EndTime.ToString()));

            CreateMap<LogisticsScheduleModel, LogisticsSchedule>()
                .ForMember(dest => dest.StartTime, cfg => cfg.MapFrom(src => TimeSpan.Parse(src.StartTimeStr)))
                .ForMember(dest => dest.EndTime, cfg => cfg.MapFrom(src => TimeSpan.Parse(src.EndTimeStr)));
               
        }

        public int Order => 1;

    }
}
