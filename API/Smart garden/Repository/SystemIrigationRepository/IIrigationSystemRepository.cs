﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Smart_garden.Entites;
using Smart_garden.Models.CompositesObjects;

namespace Smart_garden.Repository.SystemRepository
{
    public interface IIrigationSystemRepository: IRepository<IrigationSystem>
    {
        IEnumerable<object> GetSystemsByUser(int id);
        bool ExistUser(int id);
        bool ExistSeries(string series); 
        bool ExistIrigationSystem(int id);
        IQueryable GetSystemBySeries(string series);
        Task<IEnumerable<DataForArduino>> GetDataForArduino(int systemId);
        void DeleteIrrigationSystem(int id);
    }
}
