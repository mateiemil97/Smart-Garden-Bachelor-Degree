using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Smart_garden.Entites;
using Smart_garden.Repository;
using Smart_garden.Repository.BoardsKeyRepository;
using Smart_garden.Repository.MeasurementRepository;
using Smart_garden.Repository.ScheduleRepository;
using Smart_garden.Repository.SensorPortRepository;
using Smart_garden.Repository.SensorRepository;
using Smart_garden.Repository.SystemRepository;
using Smart_garden.Repository.SystemStateRepository;
using Smart_garden.Repository.ZoneRepository;

namespace Smart_garden.UnitOfWork
{
    public interface IUnitOfWork
    {
        IIrigationSystemRepository IrigationSystemRepository { get; }
        IRepository<User> UserGenericRepository { get; }
        ISensorRepository SensorRepository { get; }
        ISystemStateRepository SystemStateRepository { get; }
        IBoardsKeysRepository BoardsKeyRepository { get; }
        IScheduleRepository ScheduleRepository { get; }
        IZoneRepository ZoneRepository { get; }
        ISensorPortRepository SensorPortRepository { get; }
        IMeasurementRepository MeasurementRepository { get; }
        bool Save();
    }
}
