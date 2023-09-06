using System.Linq.Expressions;

namespace webappproject.Services
{
    public interface IRepository<T> where T : class
    {
        /// <summary>
        /// Tablodaki tüm verileri getirir.
        /// </summary>
        /// <returns></returns>
        List<T> GetAll();

        /// <summary>
        /// İstediğimiz şekilde koşullandırarak veri tabanından veri getirmemizi sağlayan method.
        /// </summary>
        /// <param name="metot"></param>
        /// <returns></returns>
        List<T> Get(Expression<Func<T, bool>> metot);

        /// <summary>
        /// Verilen id ye göre tablodaki o müşteriye ait tüm bilgileri getirir.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        T GetById(int id);


        A GetSingle<A>(Func<A, bool> metot) where A : class;

        /// <summary>
        /// Veri tabanına kayıt ekler.
        /// </summary>
        /// <param name="model"></param>
        void Add(T model);


        /// <summary>
        /// Veri tabanından verilen id ye sahip kayıdı silinir.
        /// </summary>
        /// <param name="id"></param>
        void Remove(int id);


        /// <summary>
        /// Gönderilen model ile veri tabanında olan kayıt güncellenir.
        /// </summary>
        /// <param name="model"></param>
        void Update(T model, int id);

        void Update<A>(A model, int id) where A : class;
    }
}
