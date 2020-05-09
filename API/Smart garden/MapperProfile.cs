using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Smart_garden.Entites;
using Smart_garden.Models;
using Smart_garden.Models.BoardKeyDto;
using Smart_garden.Models.CompositesObjects;
using Smart_garden.Models.FCMTokenDto;
using Smart_garden.Models.MeasurementDto;
using Smart_garden.Models.ScheduleDto;
using Smart_garden.Models.SensorDto;
using Smart_garden.Models.SensorPortDto;
using Smart_garden.Models.SystemDto;
using Smart_garden.Models.SystemStateDto;
using Smart_garden.Models.UserDto;
using Smart_garden.Models.UserVegetablesDto;
using Smart_garden.Models.ZoneDto;

namespace Smart_garden
{
    public class MapperProfile: Profile
    {
        public MapperProfile()
        {
            CreateMap<UserForCreationDto, User>();
            CreateMap<User, UserForCreationDto>();
            CreateMap<User, UserDto>().ForMember(dest => dest.Name, src => src.MapFrom(name =>
                $"{name.FirstName} {name.LastName}"));
            
            CreateMap<IrigationSystemForCreationDto, IrigationSystem>();
            CreateMap<IrigationSystem, IrigationSystemDto>();

            CreateMap<Sensor, SensorDto>();
            //            CreateMap<SensorForCreationDto, Sensor>().ForMember(dest => dest.Value, src => src.MapFrom(value =>
            //                 Math.Round(value.Value, 1)));
            CreateMap<SensorForCreationDto, Sensor>();
            CreateMap<SystemStateForCreationDto, SystemState>();
            CreateMap<SystemState, SystemStateDto>();
            
            CreateMap<SystemStateForUpdateDto, SystemState>();
            CreateMap<BoardsKeys, BoardKeyForUpdateDto>();
            CreateMap<BoardsKeys, BoardKeyDto>();

            CreateMap<Schedule,ScheduleDto>();
            CreateMap<ScheduleForCreationDto, Schedule>();
            CreateMap<ScheduleForUpdateDto, Schedule>();

            CreateMap<Zone, ZoneDto>();
            CreateMap<ZoneForCreationDto,Zone>();

            CreateMap<Zone, ZoneDtoForArduino>();
            CreateMap<ZoneDto, ZoneDtoForArduino>();
            CreateMap<ZoneSensorComposite, SensorForCreationDto>()
                .ForMember(dest => dest.SystemId, src => src.MapFrom(id => id.SystemId))
                .ForMember(dest => dest.Type, src => src.MapFrom(type => type.Type))
                .ForMember(dest => dest.PortId, src => src.MapFrom(port => port.PortId));



            CreateMap<ZoneSensorComposite, ZoneForCreationDto>()
                .ForMember(dest => dest.Name, src => src.MapFrom(name => name.Name))
                .ForMember(dest => dest.MoistureStart, src => src.MapFrom(moisture => moisture.MoistureStart))
                .ForMember(dest => dest.MoistureStop, src => src.MapFrom(moisture => moisture.MoistureStop));

            CreateMap<SensorPort, SensorPortDto>();

            CreateMap<MeasurementForCreationDto, Measurement>();
            CreateMap<Measurement,MeasurementDto>();

            CreateMap<FCMTokenForCreateDto, FCMToken>();

            CreateMap<UserVegetables, UserVegetablesForGetDto>();
            CreateMap<UserVegetablesForCreationDto, UserVegetables>();


        }
    }
}
