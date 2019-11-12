using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Smart_garden.Entites;
using Smart_garden.Repository;
using Smart_garden.Repository.MeasurementRepository;
using Smart_garden.Repository.SensorRepository;
using Smart_garden.Repository.SystemRepository;

namespace Smart_garden.UnitOfWork
{
    public class UnitOfWork: IDisposable, IUnitOfWork
    {
        private SmartGardenContext _context;

        public IIrigationSystemRepository IrigationSystemRepository { get; }
        public IRepository<User> UserGenericRepository { get; }

        public ISensorRepository SensorRepository { get; }
        public IMeasurementRepository MeasurementRepository { get; }

        public UnitOfWork(
            SmartGardenContext context,
            IRepository<User> userRepository,
            IIrigationSystemRepository irigationSystemRepository,
            ISensorRepository sensorRepository,
            IMeasurementRepository measurementRepository
            )
        {
            _context = context;
            UserGenericRepository = userRepository;
            IrigationSystemRepository = irigationSystemRepository;
            SensorRepository = sensorRepository;
            MeasurementRepository = measurementRepository;
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
