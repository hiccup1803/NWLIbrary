using System.Data;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Tool.hbm2ddl;
using NW.Core.Entities;
using NW.Core.Work;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Data.NHibernate.Work
{
    public class UnitOfWork : IUnitOfWork
    {
        /// <summary>
        /// Gets current instance of the NhUnitOfWork.
        /// It gets the right instance that is related to current thread.
        /// </summary>
        public UnitOfWork Current
        {
            get
            {
                if (_current == null)
                    _current = new UnitOfWork(sessionFactory);
                return _current;
            }
        }
        [ThreadStatic]
        private UnitOfWork _current;

        /// <summary>
        /// Reference to the session factory.
        /// </summary>
        private readonly ISessionFactory sessionFactory;

        /// <summary>
        /// Creates a new instance of NhUnitOfWork.
        /// </summary>
        /// <param name="sessionFactory"></param>
        public UnitOfWork(ISessionFactory _sessionFactory)
        {
            sessionFactory = _sessionFactory;
        }

        /// <summary>
        /// Opens database connection and begins transaction.
        /// </summary>
        public ITransaction BeginTransaction(ISession _session, IsolationLevel isoLevel = IsolationLevel.ReadCommitted)
        {
            return _session.BeginTransaction(isoLevel);
        }

        /// <summary>
        /// Commits transaction and closes database connection.
        /// </summary>
        public void Commit(ITransaction transaction)
        {
            transaction.Commit();
        }

        /// <summary>
        /// Rollbacks transaction and closes database connection.
        /// </summary>
        public void Rollback(ITransaction transaction)
        {
            transaction.Rollback();
        }

        public void Dispose()
        {
            //Session.Dispose();
        }
    }
}
