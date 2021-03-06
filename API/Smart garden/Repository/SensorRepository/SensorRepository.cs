﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Remotion.Linq.Clauses;
using Smart_garden.Entites;
using Smart_garden.Migrations;
using Smart_garden.Models.SensorDto;

namespace Smart_garden.Repository.SensorRepository
{
    public class SensorRepository:Repository<Sensor>, ISensorRepository
    {
        private readonly SmartGardenContext _context;
        public SensorRepository(SmartGardenContext context) : base(context)
        {
            _context = context;
        }

        public object GetSensorsBySystem(int systemId)
        {
            var sensors = (from sensor in _context.Sensor
                    join systems in _context.IrigationSystem
                        on sensor.SystemId equals systems.Id
                           where sensor.SystemId == systemId
                    select new
                    {
                        IrigationSystemId = systems.Id,
                        SensorId = sensor.Id,
                        Type = sensor.Type,
                        Port = sensor.PortId
                    }
                );
            return sensors;
        }

        public IQueryable<Sensor> GetSensorBySystem(int systemId, int sensorId)
        {
            var sensor = (from sys in _context.IrigationSystem
                    join sns in _context.Sensor
                        on sys.Id equals sns.SystemId
                    where sns.SystemId == sys.Id 
                    select new Sensor
                    {
                        Id = sns.Id,
                        SystemId = sys.Id,
                        Type = sns.Type,
                        PortId = sns.PortId
                    }
                );
            return sensor;
        }

//        public Sensor GetLatestSensorValueByType(int systemId, string type)
//        {
//            var measurement = (from sys in _context.IrigationSystem
//                join sns in _context.Sensor
//                    on sys.Id equals sns.SystemId
//                where sns.Type == type
//                //orderby sns.DateTime descending
//                select sns).Take(1).SingleOrDefault();
//
//            return measurement;
//        }

        public Sensor GetSensorById(int id)
        {
            var sensor = _context.Sensor.FirstOrDefault(sns => sns.Id == id);
            return sensor;
        }

        public Sensor GetSensorBySystemAndPortName(int systemId, string portName)
        {
            var sensor = (from sys in _context.IrigationSystem
                    join sns in _context.Sensor on sys.Id equals sns.SystemId
                    join port in _context.SensorPort on sns.PortId equals port.Id
                    where sys.Id == systemId && port.Port == portName
                    select sns
                ).FirstOrDefault();
            return sensor;
        }

        public IQueryable<SensorInfoForStatistics> GetInfoForStatistics(int systemId)
        {
            var info = (from sns in _context.Sensor
                    join zone in _context.Zone on sns.Id equals zone.SensorId into snsZone
                    from zone in snsZone.DefaultIfEmpty()
                    where sns.SystemId == systemId
                    select new SensorInfoForStatistics()
                    {
                        SensorId = sns.Id,
                        Type = sns.Type,
                        Name = zone.Name,
                        MoistureStop = zone.MoistureStop,
                        MoistureStart = zone.MoistureStart
                    }
                );
            return info;
        }


    }
}
