using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Smart_garden.Entites;

namespace Smart_garden.Repository.BoardsKeyRepository
{
    public interface IBoardsKeysRepository: IRepository<BoardsKeys>
    {
        object GetSystemBySeries(string series);
        object GetSeriesBySystem(int id);
    }
}
