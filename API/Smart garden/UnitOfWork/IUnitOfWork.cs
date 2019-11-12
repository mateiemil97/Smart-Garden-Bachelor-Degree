using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Smart_garden.Entites;
using Smart_garden.Repository;
using Smart_garden.Repository.SensorRepository;
using Smart_garden.Repository.SystemRepository;

namespace Smart_garden.UnitOfWork
{
    public interface IUnitOfWork
    {
        IIrigationSystemRepository IrigationSystemRepository { get; }
        IRepository<User> UserGenericRepository { get; }
        ISensorRepository SensorRepository { get; }
        bool Save();
    }
}
