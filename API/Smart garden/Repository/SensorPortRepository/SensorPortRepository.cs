using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Smart_garden.Entites;

namespace Smart_garden.Repository.SensorPortRepository
{
    public class SensorPortRepository: Repository<SensorPort>, ISensorPortRepository
    {
        private SmartGardenContext _context;
        public SensorPortRepository(SmartGardenContext context) : base(context)
        {
            _context = context;
        }

        public IQueryable<SensorPort> GetUsedPorts(int systemId)
        {
            var ports = from sys in _context.IrigationSystem
                join sns in _context.Sensor on sys.Id equals sns.SystemId
                join port in _context.SensorPort on sns.PortId equals port.Id
                where sys.Id == systemId
                select port;
            return ports;
        }

        public  IQueryable<SensorPort> GetAllPorts()
        {
            return _context.SensorPort.ToList().AsQueryable();
        }
    }
}
