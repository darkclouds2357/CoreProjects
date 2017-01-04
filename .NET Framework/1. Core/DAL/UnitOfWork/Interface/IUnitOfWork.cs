using DAL.Entites;
using DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Data.Entity.Infrastructure;
using System.Linq;

namespace DAL.UnitOfWork
{
    public interface IUnitOfWork
    {
        IDisposable BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);

        void RollbackTransaction();

        void CommitTransaction();

        DbContext Context { get; }

        Database Database { get; }

        IRepository<TEntity> GetRepository<TEntity>(bool isLazyLoadingEnabled = true) where TEntity : BaseEntity;

        IQueryable<TEntity> ExecWithFunction<TEntity>(string sqlQuery, params object[] parameters) where TEntity : BaseEntity;

        int ExecWithStoreProcedure(string query, params object[] parameters);

        IList<TEntity> ExecWithStoreProcedureWithCommand<TEntity>(string query, params object[] parameters) where TEntity : BaseEntity;

        SqlConnection GetSqlConnection();

        IEnumerable<DbEntityEntry<TEntity>> ChangeTracker<TEntity>() where TEntity : BaseEntity;

        bool SaveAndReload<T>(T entity) where T : BaseEntity;

        bool Save();
    }
}
