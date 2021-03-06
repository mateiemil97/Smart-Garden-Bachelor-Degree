﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Smart_garden.Entites;

namespace Smart_garden.Repository.BoardsKeyRepository
{
    public class BoardsKeysRepository: Repository<BoardsKeys>, IBoardsKeysRepository
    {
        private readonly SmartGardenContext _context;

        public BoardsKeysRepository(SmartGardenContext context) : base(context)
        {
            _context = context;
        }


        public object GetSystemBySeries(string series)
        {
            var system = (from sys in _context.IrigationSystem
                join brdKey in _context.BoardKey
                    on sys.BoardKeyId equals brdKey.Id
                where brdKey.SeriesKey == series
                select new
                {
                    IrigationSystemId = sys.Id,
                    Registered = brdKey.Registered
                }).FirstOrDefault();
            return system;
        }

        public object GetSeriesBySystem(int id)
        {
            var system = (from brdKey in _context.BoardKey
                    join sys in _context.IrigationSystem
                        on brdKey.Id equals sys.BoardKeyId
                    select new
                    {
                        Id = brdKey.Id,
                        Registered = brdKey.Registered,
                        SeriesKey = brdKey.SeriesKey
                    }
                ).FirstOrDefault();
            return system;
        }

        public IQueryable<BoardsKeys> GetBoardKeyBySeries(string serie)
        {
            var boardInfo = from brd in _context.BoardKey
                where brd.SeriesKey == serie
                select brd;
            return boardInfo;
        }
    }
}
