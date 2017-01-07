using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Entites;
using DAL.Repositories;
using System.Data.Common;
using System.Data.Entity;

namespace DAL.UnitOfWork
{
    public class UnitOfWork<TCtx> : IUnitOfWork where TCtx : IDbContext
    {
        private readonly IDbContext _context;
        private Dictionary<Type, object> _repositories;
        private bool _disposed = false;
        private DbTransaction _transaction;
        private readonly ICurrentUser _currentUser;

        public UnitOfWork(TCtx context, ICurrentUser currentUser, IEnumerable<IBaseRepository> reposiotories)
        {
            this._context = context;
            this._currentUser = currentUser;
            this._repositories = reposiotories.GroupBy(r => r.EntityType).ToDictionary(r => r.Key, r => (object)r.FirstOrDefault());
        }

        public IRepository<TEntity> GetRepository<TEntity>(bool isLazyLoadingEnabled = true) where TEntity : BaseEntity
        {
            this._context.Configuration.LazyLoadingEnabled = isLazyLoadingEnabled;
            if (this._repositories.Keys.Contains(typeof(TEntity)))
            {
                return this._repositories[typeof(TEntity)] as IRepository<TEntity>;
            }
            else
            {
                throw new Exception($"Repository of {typeof(TEntity).Name} not existed");
            }
        }

        public IDisposable BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            if (_context.Database.Connection.State != ConnectionState.Open)
            {
                _context.Database.Connection.Open();
            }
            _transaction = _context.Database.Connection.BeginTransaction(isolationLevel);
            _context.Database.UseTransaction(_transaction);
            return _transaction;
        }

        public IEnumerable<DbEntityEntry<TEntity>> ChangeTracker<TEntity>() where TEntity : BaseEntity
        {
            return _context.ChangeTracker.Entries<TEntity>().ToArray();
        }

        public void CommitTransaction()
        {
            if (_transaction == null)
            {
                throw new ApplicationException("Cannot roll back a transaction while there is no transaction running.");
            }
            try
            {
                IEnumerable<BaseEntity> itemChanges = _context.ChangeTracker.Entries().Where(item => item.State == EntityState.Added || item.State == EntityState.Modified || item.State == EntityState.Deleted).Select(e => e.Entity as BaseEntity);

                _context.SaveChanges();

                _transaction.Commit();
                _transaction.Dispose();
            }
            catch (Exception ex)
            {
                RollbackTransaction();
                throw ex;
            }
        }

        public IQueryable<TEntity> ExecWithFunction<TEntity>(string sqlQuery, params object[] parameters) where TEntity : class
        {
            throw new NotImplementedException();
        }

        public int ExecWithStoreProcedure(string query, params object[] parameters)
        {
            throw new NotImplementedException();
        }

        public IList<TEntity> ExecWithStoreProcedureWithCommand<TEntity>(string query, params object[] parameters) where TEntity : class
        {
            throw new NotImplementedException();
        }

        public SqlConnection GetSqlConnection()
        {
            try
            {
                return _context.Database.Connection as SqlConnection;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public void RollbackTransaction()
        {
            if (_transaction == null)
            {
                throw new ApplicationException("Cannot roll back a transaction while there is no transaction running.");
            }
            _transaction.Rollback();
            _transaction.Dispose();
        }

        public int Save()
        {
            return _context.SaveChanges();
        }

        public int SaveAndReload<TEntity>(TEntity entity) where TEntity : BaseEntity
        {
            int saveChanges = this.Save();
            if (saveChanges > 0)
            {
                _context.Entry(entity).Reload();
            }
            return saveChanges;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    if (_transaction != null)
                    {
                        _transaction.Dispose();
                    }
                    _context.Dispose();
                }
                this._disposed = true;
            }
        }

    }
}
