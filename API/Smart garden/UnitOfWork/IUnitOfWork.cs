﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Smart_garden.Entites;
using Smart_garden.Repository;
using Smart_garden.Repository.BoardsKeyRepository;
using Smart_garden.Repository.SensorRepository;
using Smart_garden.Repository.SystemRepository;
using Smart_garden.Repository.SystemStateRepository;

namespace Smart_garden.UnitOfWork
{
    public interface IUnitOfWork
    {
        IIrigationSystemRepository IrigationSystemRepository { get; }
        IRepository<User> UserGenericRepository { get; }
        ISensorRepository SensorRepository { get; }
        ISystemStateRepository SystemStateRepository { get; }
        IBoardsKeysRepository BoardsKeyRepository { get; }

        bool Save();
    }
}
