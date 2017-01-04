using DAL.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Entites;
using DAL.Repositories;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;

namespace DAL.UnitTest
{
    public class InMemoryUnitOfWork : IUnitOfWork
    {
        public DbContext Context
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public Database Database
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public IDisposable BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<DbEntityEntry<TEntity>> ChangeTracker<TEntity>() where TEntity : BaseEntity
        {
            throw new NotImplementedException();
        }

        public void CommitTransaction()
        {
            throw new NotImplementedException();
        }

        public IQueryable<TEntity> ExecWithFunction<TEntity>(string sqlQuery, params object[] parameters) where TEntity : BaseEntity
        {
            throw new NotImplementedException();
        }

        public int ExecWithStoreProcedure(string query, params object[] parameters)
        {
            throw new NotImplementedException();
        }

        public IList<TEntity> ExecWithStoreProcedureWithCommand<TEntity>(string query, params object[] parameters) where TEntity : BaseEntity
        {
            throw new NotImplementedException();
        }

        public IRepository<TEntity> GetRepository<TEntity>(bool isLazyLoadingEnabled = true) where TEntity : BaseEntity
        {
            throw new NotImplementedException();
        }

        public SqlConnection GetSqlConnection()
        {
            throw new NotImplementedException();
        }

        public void RollbackTransaction()
        {
            throw new NotImplementedException();
        }

        public bool Save()
        {
            throw new NotImplementedException();
        }

        public bool SaveAndReload<T>(T entity) where T : BaseEntity
        {
            throw new NotImplementedException();
        }
    }
}
