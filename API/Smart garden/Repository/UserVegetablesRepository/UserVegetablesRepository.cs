using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Smart_garden.Entites;

namespace Smart_garden.Repository.UserVegetablesRepository
{
    public class UserVegetablesRepository: Repository<UserVegetables>,IUserVegetablesRepository
    {
        private SmartGardenContext _context;

        public UserVegetablesRepository(SmartGardenContext context): base(context)
        {
            _context = context;
        }

        IQueryable<UserVegetables> IUserVegetablesRepository.GetUserVegetables(int userId)
        {
            var vegetables = (from v in _context.UserVegetableses
                    join u in _context.User
                        on v.UserId equals u.Id
                    where u.Id == userId
                    select new UserVegetables()
                    {
                        Id = v.Id,
                        Name = v.Name,
                        StartMoisture = v.StartMoisture,
                        StopMoisture = v.StopMoisture,
                        UserId = u.Id
                    }
                );
            return vegetables;
        }

        IQueryable<UserVegetables> IUserVegetablesRepository.GetUserVegetable(int userId, int vegetableId)
        {
            var vegetable = (from v in _context.UserVegetableses
                    join u in _context.User
                        on v.UserId equals u.Id
                    where u.Id == userId && v.Id == vegetableId
                    select new UserVegetables()
                    {
                        Id = v.Id,
                        Name = v.Name,
                        StartMoisture = v.StartMoisture,
                        StopMoisture = v.StopMoisture,
                        UserId = u.Id
                    }
                );
            return vegetable;
        }

        

    }
}
