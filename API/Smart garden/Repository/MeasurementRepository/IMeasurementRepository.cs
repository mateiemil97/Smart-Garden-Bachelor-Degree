﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Smart_garden.Entites;
using Smart_garden.Models.MeasurementDto;

namespace Smart_garden.Repository.MeasurementRepository
{
    public interface IMeasurementRepository: IRepository<Measurement>
    {
        MeasurementDto GetLatestMeasurementValueByType(int systemId, int sensorId);
        MeasurementDto GetLatestMeasurementOfTemperature(int systemId);
    }
}
