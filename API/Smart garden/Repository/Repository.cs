using System.Collections;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Smart_garden.Entites;

namespace Smart_garden.Repository
{
    public class Repository<T>: IRepository<T> where T: class
    {
        private SmartGardenContext _context;
        private DbSet<T> table = null;

        public Repository(SmartGardenContext context)
        {
            _context = context;
            table = _context.Set<T>();
        }
        public virtual IEnumerable GetAll()
        {
            return table.ToList();
        }

        public virtual T Get(int id)
        {
            return table.Find(id);
        }

        public void Create(T obj)
        {
            table.Add(obj);
        }

        public void Delete(T a)
        {
            table.Remove(a);
        }

        public void Update(T obj)
        {
            table.Update(obj);
        }

        public T Exist(int id)
        {
            return table.Find(id);
        }

    }
}