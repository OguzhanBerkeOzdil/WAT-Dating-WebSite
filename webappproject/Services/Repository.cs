using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Linq.Expressions;

namespace webappproject.Services
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected Context context = new Context(new DbContextOptions<Context>());

        public DbSet<T> Table()
        {
            return Table<T>();
        }
        public DbSet<T> Table<T>() where T : class
        {
            return context.Set<T>();
        }
        public void Add(T model)
        {
            Table<T>().Add(model);
            context.SaveChanges();
        }

        public List<T> Get(Expression<Func<T, bool>> metot)
        {
            return Table<T>().Where(metot).ToList();
        }

        public List<T> GetAll()
        {
            return Table<T>().ToList();
        }

        public T GetById(int id)
        {
            return GetById<T>(id);
        }

        public A GetById<A>(int id) where A : class
        {
            return GetSingle<A>(t => typeof(A).GetProperty("Id").GetValue(t).ToString() == id.ToString());
        }
        public T GetSingle<T>(Func<T, bool> metot) where T : class
        {
            return Table<T>().FirstOrDefault(metot);
        }

        public void Remove(int id)
        {
            Table<T>().Remove(GetById<T>(id));
            context.SaveChanges();
        }

        public void Update(T model, int id)
        {
            Update<T>(model, id);
            context.SaveChanges();
        }

        public void Update<A>(A model, int id) where A : class
        {
            A guncellenecekNesne = GetById<A>(id);
            var tumPropertyler = typeof(A).GetProperties();
            foreach (var property in tumPropertyler)
                if (property.Name != "Id")
                    property.SetValue(guncellenecekNesne, property.GetValue(model));
        }
    }
}
