using CoreProject.Common.Paging;
using DAL.Search;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public interface IRepository<T>: IBaseRepository
    {
        void Insert(T entity);

        void Update(T entity);

        void Attach(T entity, EntityState state = EntityState.Unchanged);

        void Delete(T entity);

        void DeleteRange(IEnumerable<T> items);

        void DeleteRange(Expression<Func<T, bool>> criteria);

        void SoftDelete(T entity);

        T FindOne(Expression<Func<T, bool>> criteria);

        TDto FindOne<TDto>(Expression<Func<T, bool>> criteria);

        T FindOne();

        T FindOne(long id);

        IQueryable<T> GetAll(Expression<Func<T, bool>> criteria, bool includeHidden = false);

        IQueryable<T> GetAll(bool includeHidden = false);

        IQueryable<TDto> GetAll<TDto>(Expression<Func<T, bool>> criteria, bool includeHidden = false);

        IQueryable<TDto> GetAll<TDto>(bool includeHidden = false);

        IQueryable<T> GetUnSecuredQuery(Expression<Func<T, bool>> criteria = null, bool includeHidden = false);

        IQueryable<TDto> GetPaging<TDto>(List<SortExpression<T>> sortExpressions, out int totalRecord,
            Expression<Func<T, bool>> predicate = null,
            string[] includePaths = null,
            int? page = 0,
            int? pageSize = null);

        IQueryable<T> GetPaging(List<SortExpression<T>> sortExpressions, out int totalRecord,
            Expression<Func<T, bool>> predicate = null,
            string[] includePaths = null,
            int? page = 0,
            int? pageSize = null);

        //int SaveChanges();

        bool HasChange();

        Action<T> OnChange { get; set; }
        
        //int ExecWithStoreProcedure(string query, params object[] parameters);

        //IList<T> ExecWithStoreProcedureWithCommand(string query, params object[] parameters);

        //IQueryable<T> ExecWithFunction(string sqlQuery, params object[] parameters);

        PagedListResult<T> Search(SearchQuery<T> searchQuery, bool includeHidden = false);

        void BulkInsert(IEnumerable<T> items, bool keepIdentity = false, string identityFieldName = "Id");

        void BulkInsert(System.Data.DataTable table, string tableName);

        void BulkUpdateOneField(IEnumerable<T> entities, string fieldName, string fieldValue);
    }
}
