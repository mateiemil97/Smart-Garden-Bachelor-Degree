using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Smart_garden.Entites;
using Smart_garden.Models.CompositesObjects;
using Smart_garden.Models.ZoneDto;

namespace Smart_garden.Repository.SystemRepository
{
    public class IrigationSystemRepository: Repository<Entites.IrigationSystem>, IIrigationSystemRepository
    {
        private readonly SmartGardenContext _context;
        public IrigationSystemRepository(SmartGardenContext context): base(context)
        {
            _context = context;
        }

        public IEnumerable<object> GetSystemsByUser(int id)
        {
            var system = (from sys in _context.IrigationSystem
                    join user in _context.User on sys.UserId equals user.Id
                    join boardKeys in _context.BoardKey on sys.BoardKeyId equals boardKeys.Id
                          where user.Id == id
                    select new
                    {
                        UserId = user.Id,
                        SystemId = sys.Id,
                        SeriesKey = boardKeys.SeriesKey,
                        Name = sys.Name
                    }
                );
            return system;
        }

        public bool ExistUser(int id)
        {
            var user = this._context.IrigationSystem.FirstOrDefault(u => u.UserId == id);

            return user != null;
        }

        public bool ExistSeries(string series)
        {
            var isSeries = _context.BoardKey.FirstOrDefault(s => s.SeriesKey == series);

            return isSeries != null;
        }

        public bool ExistIrigationSystem(int id)
        {
            var isSystem = _context.IrigationSystem.FirstOrDefault(a => a.Id == id);

            return isSystem != null;
        }

        public IQueryable GetSystemBySeries(string series)
        {
            var system = from sys in _context.IrigationSystem
                join brdKey in _context.BoardKey
                    on sys.BoardKeyId equals brdKey.Id
                where brdKey.SeriesKey == series
                select new
                {
                    IrigationSystemId = sys.Id,
                    Registered = brdKey.Registered
                };
            return system;
        }
     
        public IEnumerable<DataForArduino> GetDataForArduino(int systemId)
        {
            var data = (from sys in _context.IrigationSystem
                join sch in _context.Schedule
                    on sys.Id equals sch.SystemId

                join remote in _context.SystemState
                    on sys.Id equals remote.SystemId
                join tkn in _context.FCMToken
                    on sys.Id equals tkn.SystemId into tk
                from tkn in tk.DefaultIfEmpty()
                    
                        where sys.Id == systemId
                select new DataForArduino()
                {
                    Manual = remote.Manual,
                    Working = remote.Working,
                    Start = sch.Start,
                    Stop = sch.Stop,
                    FCMToken = (tkn == null ? String.Empty : tkn.Token),
                    TemperatureMax = sch.TemperatureMax,
                    TemperatureMin = sch.TemperatureMin,
                });
            return data;
        }

        public void DeleteIrrigationSystem(int id)
        {
            throw new NotImplementedException();
        }
    }
}
