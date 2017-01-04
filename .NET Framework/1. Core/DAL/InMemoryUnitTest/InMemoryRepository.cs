using DAL.Entites;
using DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoreProject.Common.Paging;
using DAL.Search;
using System.Data;
using System.Data.Entity;
using System.Linq.Expressions;

namespace DAL.UnitTest
{
    public class InMemoryRepository<T> : SingletonCollectionBase, IRepository<T> where T : BaseEntity
    {
        public Action<T> OnChange { get; set; }

        public void Attach(T entity, EntityState state = EntityState.Unchanged)
        {
            throw new NotImplementedException();
        }

        public void BulkInsert(DataTable table, string tableName)
        {
            throw new NotImplementedException();
        }

        public void BulkInsert(IEnumerable<T> items, bool keepIdentity = false, string identityFieldName = "Id")
        {
            throw new NotImplementedException();
        }

        public void BulkUpdateOneField(IEnumerable<T> entities, string fieldName, string fieldValue)
        {
            throw new NotImplementedException();
        }

        public int Delete(T entity)
        {
            throw new NotImplementedException();
        }

        public int DeleteRange(Expression<Func<T, bool>> criteria)
        {
            throw new NotImplementedException();
        }

        public int DeleteRange(IEnumerable<T> items)
        {
            throw new NotImplementedException();
        }

        public T FindOne()
        {
            throw new NotImplementedException();
        }

        public T FindOne(long id)
        {
            throw new NotImplementedException();
        }

        public T FindOne(Expression<Func<T, bool>> criteria)
        {
            throw new NotImplementedException();
        }

        public TDto FindOne<TDto>(Expression<Func<T, bool>> criteria)
        {
            throw new NotImplementedException();
        }

        public IQueryable<T> GetAll(bool includeHidden = false)
        {
            throw new NotImplementedException();
        }

        public IQueryable<T> GetAll(Expression<Func<T, bool>> criteria, bool includeHidden = false)
        {
            throw new NotImplementedException();
        }

        public IQueryable<TDto> GetAll<TDto>(bool includeHidden = false)
        {
            throw new NotImplementedException();
        }

        public IQueryable<TDto> GetAll<TDto>(Expression<Func<T, bool>> criteria, bool includeHidden = false)
        {
            throw new NotImplementedException();
        }

        public IQueryable<T> GetPaging(List<SortExpression<T>> sortExpressions, out int totalRecord, Expression<Func<T, bool>> predicate = null, string[] includePaths = null, int? page = 0, int? pageSize = default(int?))
        {
            throw new NotImplementedException();
        }

        public IQueryable<TDto> GetPaging<TDto>(List<SortExpression<T>> sortExpressions, out int totalRecord, Expression<Func<T, bool>> predicate = null, string[] includePaths = null, int? page = 0, int? pageSize = default(int?))
        {
            throw new NotImplementedException();
        }

        public IQueryable<T> GetUnSecuredQuery(Expression<Func<T, bool>> criteria = null, bool includeHidden = false)
        {
            throw new NotImplementedException();
        }

        public bool HasChange()
        {
            throw new NotImplementedException();
        }

        public int Insert(T entity)
        {
            throw new NotImplementedException();
        }

        public PagedListResult<T> Search(SearchQuery<T> searchQuery, bool includeHidden = false)
        {
            throw new NotImplementedException();
        }

        public int SoftDelete(T entity)
        {
            throw new NotImplementedException();
        }

        public int Update(T entity)
        {
            throw new NotImplementedException();
        }
    }
}
