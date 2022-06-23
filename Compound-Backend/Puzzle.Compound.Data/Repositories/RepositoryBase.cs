using Microsoft.EntityFrameworkCore;
using Puzzle.Compound.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Puzzle.Compound.Data.Repositories
{
    public abstract class RepositoryBase<T> where T : class
    {
        #region Properties

        private CompoundDbContext dataContext;
        private DbSet<T> dbSet;

        /// <summary>
        /// Entities
        /// </summary>
        protected virtual DbSet<T> Entities
        {
            get
            {
                if (dbSet == null)
                    dbSet = dataContext.Set<T>();
                return dbSet;
            }
        }

        /// <summary>
        /// Gets a table
        /// </summary>
        public virtual IQueryable<T> Table
        {
            get
            {
                return this.Entities;
            }
        }

        /// <summary>
        /// Gets a table with "no tracking" enabled (EF feature) Use it only when you load record(s) only for read-only operations
        /// </summary>
        public virtual IQueryable<T> TableNoTracking
        {
            get
            {
                return this.Entities.AsNoTracking();
            }
        }

        public virtual CompoundDbContext DbContext
        {
            get { return dataContext; }
        }
        #endregion

        public RepositoryBase(CompoundDbContext dbContext)
        {
            dataContext = dbContext;
            dbSet = dbContext.Set<T>();
        }


        #region Implementation

        public virtual T Add(T entity)
        {
            SaveTransaction(entity);
            return dbSet.Add(entity).Entity;
        }

        public virtual void Update(T entity)
        {
            SaveTransaction(entity);
            dbSet.Attach(entity);
            dataContext.Entry(entity).State = EntityState.Modified;
        }

        public virtual void Delete(T entity)
        {
            SaveTransaction(entity);
            dbSet.Remove(entity);
        }

        public virtual void Delete(Expression<Func<T, bool>> where)
        {
            IEnumerable<T> objects = dbSet.Where<T>(where).AsEnumerable();
            foreach (T obj in objects)
            {
                SaveTransaction(obj);
                dbSet.Remove(obj);
            }
        }

        public virtual bool SaveTransaction(T entity)
        {
            return true;
        }

        public virtual T GetById(Guid id)
        {
            return dbSet.Find(id);
        }

        public virtual Task<T> GetByIdAsync(Guid id)
        {
            return dbSet.FindAsync(id).AsTask();
        }

        public virtual T GetById(string id)
        {
            return dbSet.Find(id);
        }

        public virtual IEnumerable<T> Take(int top, Func<T, object> OrderByColumn)
        {
            return dbSet.OrderByDescending(OrderByColumn).Take(top);
        }

        public virtual IEnumerable<T> GetAll()
        {
            return dbSet.ToList();
        }

        public virtual IEnumerable<T> GetMany(Expression<Func<T, bool>> where)
        {
            return dbSet.Where(where).ToList();
        }

        //public virtual IEnumerable<TSource> DistinctBy<TSource, TKey>
        //    (this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        //{
        //    HashSet<TKey> seenKeys = new HashSet<TKey>();
        //    foreach (TSource element in source)
        //    {
        //        if (seenKeys.Add(keySelector(element)))
        //        {
        //            yield return element;
        //        }
        //    }
        //}

        public T Get(Expression<Func<T, bool>> where)
        {
            return dbSet.Where(where).FirstOrDefault<T>();
        }
        #endregion

    }
}
