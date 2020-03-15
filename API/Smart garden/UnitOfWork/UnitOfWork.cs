using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Smart_garden.Entites;
using Smart_garden.Repository;
using Smart_garden.Repository.BoardsKeyRepository;
using Smart_garden.Repository.FCMTokenRepository;
using Smart_garden.Repository.MeasurementRepository;
using Smart_garden.Repository.ScheduleRepository;
using Smart_garden.Repository.SensorPortRepository;
using Smart_garden.Repository.SensorRepository;
using Smart_garden.Repository.SystemRepository;
using Smart_garden.Repository.SystemStateRepository;
using Smart_garden.Repository.ZoneRepository;

namespace Smart_garden.UnitOfWork
{
    public class UnitOfWork: IDisposable, IUnitOfWork
    {
        private SmartGardenContext _context;

        public IIrigationSystemRepository IrigationSystemRepository { get; }
        public IRepository<User> UserGenericRepository { get; }

        public ISensorRepository SensorRepository { get; }
        public ISystemStateRepository SystemStateRepository { get; }
        public IBoardsKeysRepository BoardsKeyRepository { get; }
        public IScheduleRepository ScheduleRepository { get; }
        public IZoneRepository ZoneRepository { get; }
        public  ISensorPortRepository SensorPortRepository { get; }
        public IMeasurementRepository MeasurementRepository { get; }
        public IFCMTokenRepository FCMTokenRepository { get; }
        public UnitOfWork(
            SmartGardenContext context,
            IRepository<User> userRepository,
            IIrigationSystemRepository irigationSystemRepository,
            ISensorRepository sensorRepository,
            ISystemStateRepository systemStateRepository,
            IBoardsKeysRepository boardsKeysRepository,
            IScheduleRepository scheduleRepository,
            IZoneRepository zoneRepository,
            ISensorPortRepository sensorPortRepository,
            IMeasurementRepository measurementRepository,
            IFCMTokenRepository fcmTokenRepository
        )
        {
            _context = context;
            UserGenericRepository = userRepository;
            IrigationSystemRepository = irigationSystemRepository;
            SensorRepository = sensorRepository;
            SystemStateRepository = systemStateRepository;
            BoardsKeyRepository = boardsKeysRepository;
            ScheduleRepository = scheduleRepository;
            ZoneRepository = zoneRepository;
            SensorPortRepository = sensorPortRepository;
            MeasurementRepository = measurementRepository;
            FCMTokenRepository = fcmTokenRepository;
        }
        public bool Save()
        {
            return (_context.SaveChanges() >= 1);
        }


       private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
