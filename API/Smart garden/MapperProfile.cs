using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Smart_garden.Entites;
using Smart_garden.Models;
using Smart_garden.Models.BoardKeyDto;
using Smart_garden.Models.SensorDto;
using Smart_garden.Models.SystemDto;
using Smart_garden.Models.SystemStateDto;
using Smart_garden.Models.UserDto;

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

            CreateMap<BoardsKeys, BoardKeyForUpdateDto>();
            CreateMap<BoardsKeys, BoardKeyDto>();

        }
    }
}
