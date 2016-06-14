using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using ukad_testtask.DAL.Abstract;

namespace ukad_testtask.DAL
{
    public class DataService : IDataService
    {
        ApplicationDbContext _context;

        public DataService(ApplicationDbContext context)
        {
            _context = context;
        }

        public IQueryable<T> Get<T>(Expression<Func<T, bool>> predicate = null) where T: class
        {
            var dbSet = _context.Set<T>();
            IQueryable<T> query = dbSet;

            if (predicate != null)
            {
                query = query.Where(predicate);
            }
            return query;
        }


        public T GetByID<T>(int id) where T : class
        {
            var dbSet = _context.Set<T>();
            return dbSet.Find(id);
        }

        public void Insert<T>(T entity) where T : class
        {
            var dbSet = _context.Set<T>();
            dbSet.Add(entity);
        }

        public void Delete<T>(int id) where T : class
        {
            var dbSet = _context.Set<T>();
            Delete(dbSet.Find(id));
        }

        public void Delete<T>(T entity) where T : class
        {
            var dbSet = _context.Set<T>();
            if (_context.Entry(entity).State == EntityState.Detached)
            {
                dbSet.Attach(entity);
            }
            dbSet.Remove(entity);
        }

        public void Update<T>(T entity) where T : class
        {
            var dbSet = _context.Set<T>();
            dbSet.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        private bool _disposed = false; 

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }

                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }


    }
}