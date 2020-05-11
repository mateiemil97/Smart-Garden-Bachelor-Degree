using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Smart_garden.Entites;

namespace Smart_garden.Repository.UserVegetablesRepository
{
    public interface IUserVegetablesRepository: IRepository<UserVegetables>
    {
        IQueryable<UserVegetables> GetUserVegetables(int userId);
        IQueryable<UserVegetables> GetUserVegetable(int userId, int vegetableId);
    }
}
