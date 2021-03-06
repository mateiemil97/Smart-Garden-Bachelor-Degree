﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Smart_garden.Entites;
using Smart_garden.Models.SensorDto;

namespace Smart_garden.Repository.SensorRepository
{
    public interface ISensorRepository: IRepository<Sensor>
    {
        object GetSensorsBySystem(int systemId);
        IQueryable<Sensor> GetSensorBySystem(int systemId, int sensorId);
//        Sensor GetLatestSensorValueByType(int systemId, string type);
        Sensor GetSensorById(int id);

        Sensor GetSensorBySystemAndPortName(int systemId, string portName);
        IQueryable<SensorInfoForStatistics> GetInfoForStatistics(int systemId);
    }
}
