using NHibernate;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NW.Core.Work
{
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// Opens database connection and begins transaction.
        /// </summary>
        ITransaction BeginTransaction(ISession _session, IsolationLevel isoLevel);
         
        /// <summary>
        /// Commits transaction and closes database connection.
        /// </summary>
        void Commit(ITransaction transaction);

        /// <summary>
        /// Rollbacks transaction and closes database connection.
        /// </summary>
        void Rollback(ITransaction transaction);
    }
}
