using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Smart_garden.Repository
{
    public interface IRepository<T> where T: class
    {
        IEnumerable GetAll();
        T Get(int id);
        void Create(T obj);
        void Delete(T a);
        void Update(T obj);
        T Exist(int id);
    
    }
}
