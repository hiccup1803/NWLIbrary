using System.Data;
using NHibernate;
using NW.Core.Entities;
using NW.Core.Repositories;
using NW.Core.Work;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate.Linq;
using NW.Data.NHibernate.Work;

namespace NW.Data.NHibernate.Repositories
{
    public class Repository<T, TPK> : IRepository<T, TPK> where T : Entity<TPK>
    {
        protected ISession Session { get; set; }
        public Repository(ISession _session)
        {
            Session = _session;
        }

        /// <summary>
        /// Used to get a IQueryable that is used to retrive object from entire table.
        /// </summary>
        /// <returns>IQueryable to be used to select entities from database</returns>
        public IQueryable<T> GetAll()
        {
            return Session.Query<T>();
        }

        /// <summary>
        /// Gets an entity.
        /// </summary>
        /// <param name="key">Primary key of the entity to get</param>
        /// <returns>Entity</returns>
        public T Get(TPK key)
        {
            return Session.Get<T>(key);
        }

        /// <summary>
        /// Inserts a new entity.
        /// </summary>
        /// <param name="entity">Entity</param>
        public T Insert(T entity)
        {
            Session.Save(entity);
            return entity;
        }

        /// <summary>
        /// Updates an existing entity.
        /// </summary>
        /// <param name="entity">Entity</param>
        public T Update(T entity)
        {
            Session.Update(entity);
            return entity;
        }

        /// <summary>
        /// Deletes an entity.
        /// </summary>
        /// <param name="id">Id of the entity</param>
        public void Delete(TPK id)
        {
            Session.Delete(Session.Load<T>(id));
        }
    }
}
