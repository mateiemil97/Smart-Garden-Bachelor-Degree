using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Smart_garden.Entites;

namespace Smart_garden.Repository.FCMTokenRepository
{
    public class FCMTokenRepository: Repository<FCMToken>, IFCMTokenRepository
    {
        private SmartGardenContext _context;

        public FCMTokenRepository(SmartGardenContext context) : base(context)
        {
            _context = context;
        }

        public IQueryable<FCMToken> GetTokenByIrrigationSystem(int id)
        {
            var token = (from sys in _context.IrigationSystem
                join tok in _context.FCMToken
                    on sys.Id equals tok.SystemId
                where sys.Id == id
                select new FCMToken()
                {
                    Id = tok.Id,
                    Token = tok.Token,
                    SystemId = sys.Id
                });
            return token;
        }
    }
}
