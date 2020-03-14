using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Smart_garden.Entites;

namespace Smart_garden.Repository.FCMTokenRepository
{
    public interface IFCMTokenRepository: IRepository<FCMToken>
    {
        IQueryable<FCMToken> GetTokenByIrrigationSystem(int id);
        void DeleteTokenByIrrigationSystemId(int id);
    }
}
