using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Smart_garden.Entites;

namespace Smart_garden.Repository.SystemRepository
{
    public class IrigationSystemRepository: Repository<Entites.IrigationSystem>, IIrigationSystemRepository
    {
        private readonly SmartGardenContext _context;
        public IrigationSystemRepository(SmartGardenContext context): base(context)
        {
            _context = context;
        }

        public object GetSystemByUser(int id)
        {
            var system = (from sys in _context.IrigationSystem
                    join user in _context.User on sys.UserId equals user.Id
                    select new
                    {
                        UserId = user.Id,
                        SystemId = sys.Id,
                        SeriesKey = sys.SeriesKey
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
            var isSeries = _context.IrigationSystem.FirstOrDefault(s => s.SeriesKey == series);

            return isSeries != null;
        }

        public bool ExistIrigationSystem(int id)
        {
            var isSystem = _context.IrigationSystem.FirstOrDefault(a => a.Id == id);

            return isSystem != null;
        }
    }
}
