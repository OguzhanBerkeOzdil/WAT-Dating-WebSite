using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Linq.Expressions;

namespace webappproject.Services
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly Context context;

        public Repository(Context context)
        {
            this.context = context;
        }

        public DbSet<T> Table()
        {
            return context.Set<T>();
        }
        public DbSet<TEntity> Table<TEntity>() where TEntity : class
        {
            return context.Set<TEntity>();
        }
        public void Add(T model)
        {
            context.Set<T>().Add(model);
            context.SaveChanges();
        }

        public List<T> Get(Expression<Func<T, bool>> metot)
        {
            return context.Set<T>().Where(metot).ToList();
        }

        public List<T> GetAll()
        {
            return context.Set<T>().ToList();
        }

        public T? GetById(int id)
        {
            return GetById<T>(id);
        }

        public A? GetById<A>(int id) where A : class
        {
            return GetSingle<A>(t => typeof(A).GetProperty("Id")?.GetValue(t)?.ToString() == id.ToString());
        }
        public TEntity? GetSingle<TEntity>(Func<TEntity, bool> metot) where TEntity : class
        {
            return context.Set<TEntity>().FirstOrDefault(metot);
        }

        public void Remove(int id)
        {
            var entity = GetById<T>(id);
            if (entity != null)
            {
                context.Set<T>().Remove(entity);
                context.SaveChanges();
            }
        }

        public void Update(T model, int id)
        {
            Update<T>(model, id);
            context.SaveChanges();
        }

        public void Update<A>(A model, int id) where A : class
        {
            A? guncellenecekNesne = GetById<A>(id);
            if (guncellenecekNesne == null) return;
            
            var tumPropertyler = typeof(A).GetProperties();
            foreach (var property in tumPropertyler)
                if (property.Name != "Id")
                    property.SetValue(guncellenecekNesne, property.GetValue(model));
        }
    }
}
