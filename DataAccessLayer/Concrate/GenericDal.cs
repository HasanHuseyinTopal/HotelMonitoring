using DataAccessLayer.Abstract;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;


namespace DataAccessLayer.Concrate
{
    public class GenericDal<TEntity> : IGenericDAL<TEntity> where TEntity : class, new()
    {
        CpContext _context;
        public GenericDal(CpContext context)
        {
            _context = context;

        }
        public void Add(TEntity entity)
        {
            _context.Set<TEntity>().Add(entity);
        }
        public void Update(TEntity entity)
        {
            _context.Set<TEntity>().Update(entity);
        }

        public void Delete(TEntity entity)
        {
            _context.Set<TEntity>().Remove(entity);
        }
        public TEntity GetOne(Expression<Func<TEntity, bool>> filter)
        {
            return _context.Set<TEntity>().FirstOrDefault(filter);
        }

        public IQueryable<TEntity> GetAll(Expression<Func<TEntity, bool>> filter = null)
        {
            return filter == null ? _context.Set<TEntity>().AsQueryable() : _context.Set<TEntity>().Where(filter).AsQueryable();
        }

    }
}
