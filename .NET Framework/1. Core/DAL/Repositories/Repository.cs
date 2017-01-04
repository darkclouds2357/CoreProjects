using DAL.Entites;
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
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Data.Common;
using System.ComponentModel;
using LinqKit;
using CoreProject.Common.Extensions;

namespace DAL.Repositories
{
    public class Repository<T> : SingletonCollectionBase, IRepository<T> where T : BaseEntity
    {
        private readonly ICurrentUser _currentUser;
        private readonly IDbContext _context;
        private readonly DbSet<T> _dbSet;
        private readonly T _simpleInstance;
        private readonly string _tableName;
        private readonly IDbConnection _connection;
        private readonly Type _entityType = typeof(T);

        private readonly Expression<Func<T, bool>> _securedCondition;

        private IQueryable<T> _securedTable;

        public Repository(IDbContext dbContext, ICurrentUser currentUser)
        {
            var objectContext = (dbContext as IObjectContextAdapter).ObjectContext;
            this._currentUser = currentUser;
            this._context = dbContext;
            this._dbSet = dbContext.Set<T>();
            this._tableName = objectContext.CreateObjectSet<T>().EntitySet.Name;
            this._simpleInstance = Activator.CreateInstance<T>();
            this._connection = this._context.Database.Connection;
            try
            {
                this._securedCondition = this._simpleInstance.SetSecuredCondition<T>();
            }
            catch (Exception)
            {
                this._securedCondition = null;
            }

        }

        protected IQueryable<T> SecuredTable
        {
            get
            {
                if (_securedTable == null)
                {
                    _securedTable = (_securedCondition == null || _currentUser.IsSystemUser) ? _dbSet : _dbSet.Where(_securedCondition);
                }
                return _securedTable;
            }
        }
        
        #region Properties
        public Action<T> OnChange { get; set; }

        #endregion

        #region Insert/Update/Delete

        public void Attach(T entity, EntityState state = EntityState.Unchanged)
        {
            _dbSet.Attach(entity);
            switch (state)
            {
                case EntityState.Added:
                    _context.Entry(entity).State = EntityState.Added;
                    break;
                case EntityState.Deleted:
                    _context.Entry(entity).State = EntityState.Deleted;
                    break;
                case EntityState.Modified:
                    _context.Entry(entity).State = EntityState.Modified;
                    break;
                default:
                    _context.Entry(entity).State = EntityState.Unchanged;
                    break;
            }
        }

        public void Insert(T entity)
        {
            if (entity.CreatedDate == null)
                entity.CreatedDate = DateTime.UtcNow;
            if (entity.CreatedBy == null)
                entity.CreatedBy = _currentUser?.UserId.ToString();
            entity.IsActive = true;

            _dbSet.Add(entity);
        }

        public void Update(T entity)
        {
            DbEntityEntry entry = _context.Entry(entity);
            if (entry.State == EntityState.Detached)
            {
                _dbSet.Attach(entity);
            }
            entry.State = EntityState.Modified;
        }

        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }

        public void DeleteRange(Expression<Func<T, bool>> criteria)
        {
            var items = _dbSet.Where(criteria);
            _dbSet.RemoveRange(items);
        }

        public void DeleteRange(IEnumerable<T> items)
        {
            _dbSet.RemoveRange(items);
        }

        public void SoftDelete(T entity)
        {
            entity.IsActive = false;
            entity.UpdatedBy = _currentUser?.UserId.ToString();
            entity.UpdatedDate = DateTime.UtcNow;
            this.Update(entity);
        }

        public void BulkInsert(DataTable table, string tableName)
        {
            //connection string is the only way this works, 
            //  don't try reusing the connection from Entity Framework
            try
            {
                using (var bulkCopy = new SqlBulkCopy(this._connection.ConnectionString))
                {
                    bulkCopy.BatchSize = table.Rows.Count;
                    bulkCopy.DestinationTableName = tableName;
                    foreach (DataColumn column in table.Columns)
                    {
                        bulkCopy.ColumnMappings.Add(column.ColumnName, column.ColumnName);
                    }

                    bulkCopy.WriteToServer(table);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("BulkInsert error.", ex);
            }
        }

        public void BulkInsert(IEnumerable<T> items, bool keepIdentity = false, string identityFieldName = "Id")
        {
            try
            {
                using (var bulkCopy = new SqlBulkCopy(this._connection.ConnectionString))
                {
                    bulkCopy.BatchSize = items.Count();
                    bulkCopy.DestinationTableName = this._tableName;

                    var table = new System.Data.DataTable();
                    var props = TypeDescriptor.GetProperties(this._entityType)
                                               //Dirty hack to make sure we only have system data types 
                                               //i.e. filter out the relationships/collections
                                               .Cast<PropertyDescriptor>()
                                               .Where(propertyInfo => propertyInfo.PropertyType.Namespace.Equals("System"))
                                               .ToArray();

                    if (keepIdentity)
                        props = props.Where(x => x.Name != identityFieldName).ToArray();

                    foreach (var propertyInfo in props)
                    {
                        bulkCopy.ColumnMappings.Add(propertyInfo.Name, propertyInfo.Name);
                        table.Columns.Add(propertyInfo.Name, Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType);
                    }

                    var values = new object[props.Length];
                    foreach (var item in items)
                    {
                        for (var i = 0; i < values.Length; i++)
                        {
                            values[i] = props[i].GetValue(item);
                        }

                        table.Rows.Add(values);
                    }

                    try
                    {
                        bulkCopy.WriteToServer(table);
                    }
                    catch (Exception ex)
                    {
                        //fallback to insert one-by-one (ambient transactions will cause SqlBulkCopy to fail)
                        _dbSet.AddRange(items);
                        _context.SaveChanges();
                        throw new Exception("BulkInsert error.", ex);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("BulkInsert error before WriteToServer.", ex);
            }
        }

        public void BulkUpdateOneField(IEnumerable<T> entities, string fieldName, string fieldValue)
        {
            var ids = string.Join(", ", entities.Select(s => s.Id));
            string query = $"UPDATE {_tableName} SET {fieldName} = {fieldValue} WHERE ID in ({ids})";
            _context.Database.ExecuteSqlCommand(query);
        }

        #endregion
               
        #region Query Data
        public T FindOne()
        {
            return SecuredTable.FirstOrDefault();
        }

        public T FindOne(long id)
        {
            return FindOne(e => e.Id == id);
        }

        public T FindOne(Expression<Func<T, bool>> criteria)
        {
            return SecuredTable.Where(criteria).AsExpandable().FirstOrDefault();
        }

        public IQueryable<T> GetAll(bool includeHidden = false)
        {
            var query = !includeHidden ? SecuredTable.Where(e => e.IsActive) : SecuredTable;
            return query;
        }

        public IQueryable<T> GetAll(Expression<Func<T, bool>> criteria, bool includeHidden = false)
        {
            return GetAll(includeHidden).AsExpandable().Where(criteria);
        }

        public IQueryable<T> GetPaging(List<SortExpression<T>> sortExpressions, out int totalRecord, Expression<Func<T, bool>> predicate = null, string[] includePaths = null, int? page = 0, int? pageSize = default(int?))
        {
            IQueryable<T> query = GetAll();

            if (predicate != null)
            {
                query = query.AsExpandable().Where(predicate);

            }
            totalRecord = query.AsExpandable().Count();

            if (includePaths != null)
            {
                int includePathCount = includePaths.Count();
                for (var i = 0; i < includePathCount; i++)
                {
                    query = query.Include(includePaths[i]);
                }
            }

            if (sortExpressions != null)
            {
                IOrderedQueryable<T> orderedQuery = null;
                int sortExpressionCount = sortExpressions.Count();
                for (var i = 0; i < sortExpressionCount; i++)
                {
                    if (i == 0)
                    {
                        orderedQuery = sortExpressions[i].SortDirection == ListSortDirection.Ascending ? query.OrderBy(sortExpressions[i].SortBy) : query.OrderByDescending(sortExpressions[i].SortBy);
                    }
                    else
                    {
                        if (orderedQuery != null)
                            orderedQuery = sortExpressions[i].SortDirection == ListSortDirection.Ascending ? orderedQuery.ThenBy(sortExpressions[i].SortBy) : orderedQuery.ThenByDescending(sortExpressions[i].SortBy);
                    }
                }

                if (page != null)
                {
                    if (pageSize != null)
                        query = orderedQuery.Skip((page.Value - 1) * pageSize.Value);
                }
            }

            if (pageSize != null)
            {
                query = query.Take(pageSize.Value);
            }
            return query;
        }

        public IQueryable<T> GetUnSecuredQuery(Expression<Func<T, bool>> criteria = null, bool includeHidden = false)
        {
            var query = criteria == null ? _dbSet : _dbSet.Where(criteria);
            return !includeHidden ? query.Where(e => e.IsActive) : query;
        }

        public bool HasChange()
        {
            return _context.ChangeTracker.Entries<T>()
                    .Where(
                        item =>
                            item.State == EntityState.Added || item.State == EntityState.Modified || item.State == EntityState.Deleted)
                    .Select(entry => entry.Entity)
                    .Any();
        }

        public PagedListResult<T> Search(SearchQuery<T> searchQuery, bool includeHidden = false)
        {
            IQueryable<T> sequence = GetAll(includeHidden);

            //Applying filters
            sequence = ManageFilters(searchQuery, sequence);

            //Include Properties
            sequence = ManageIncludeProperties(searchQuery, sequence);

            //Resolving Sort Criteria
            //This code applies the sorting criterias sent as the parameter
            sequence = ManageSortCriterias(searchQuery, sequence);

            return GetTheResult(searchQuery, sequence);
        }

        #region Map Entity To DTO 

        public TDto FindOne<TDto>(Expression<Func<T, bool>> criteria)
        {
            return SecuredTable.Where(criteria).Select<T, TDto>().FirstOrDefault();
        }

        public IQueryable<TDto> GetAll<TDto>(bool includeHidden = false)
        {
            return GetAll(includeHidden).Select<T, TDto>();
        }

        public IQueryable<TDto> GetAll<TDto>(Expression<Func<T, bool>> criteria, bool includeHidden = false)
        {
            return GetAll(includeHidden).Where(criteria).Select<T, TDto>();
        }
        public IQueryable<TDto> GetPaging<TDto>(List<SortExpression<T>> sortExpressions, out int totalRecord, Expression<Func<T, bool>> predicate = null, string[] includePaths = null, int? page = 0, int? pageSize = default(int?))
        {
            return GetPaging(sortExpressions, out totalRecord, predicate, includePaths, page, pageSize)
                .Select<T, TDto>();
        }

        #endregion


        #endregion
        
        #region Internal Methods
        private IQueryable<T> ManageFilters(SearchQuery<T> searchQuery, IQueryable<T> sequence)
        {
            if (searchQuery.Filters != null && searchQuery.Filters.Count > 0)
            {
                foreach (var filterClause in searchQuery.Filters)
                {
                    sequence = sequence.AsExpandable().Where(filterClause);
                }
            }
            return sequence;
        }

        private IQueryable<T> ManageIncludeProperties(SearchQuery<T> searchQuery, IQueryable<T> sequence)
        {
            if (!string.IsNullOrWhiteSpace(searchQuery.IncludeProperties))
            {
                var properties = searchQuery.IncludeProperties.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var includeProperty in properties)
                {
                    sequence = sequence.AsExpandable().Include(includeProperty);
                }
            }
            return sequence;
        }

        private IQueryable<T> ManageSortCriterias(SearchQuery<T> searchQuery, IQueryable<T> sequence)
        {
            if (searchQuery.SortCriterias != null && searchQuery.SortCriterias.Count > 0)
            {
                var sortCriteria = searchQuery.SortCriterias[0];
                var orderedSequence = sortCriteria.ApplyOrdering(sequence, false);

                if (searchQuery.SortCriterias.Count > 1)
                {
                    for (var i = 1; i < searchQuery.SortCriterias.Count; i++)
                    {
                        var sc = searchQuery.SortCriterias[i];
                        orderedSequence = sc.ApplyOrdering(orderedSequence, true);
                    }
                }
                sequence = orderedSequence;
            }
            else
            {
                sequence = ((IOrderedQueryable<T>)sequence).OrderBy(x => (true));
            }
            return sequence;
        }

        private PagedListResult<T> GetTheResult(SearchQuery<T> searchQuery, IQueryable<T> sequence)
        {
            //Counting the total number of object.
            var resultCount = sequence.AsExpandable().Count();

            var result =
                sequence.AsExpandable()
                    .Skip(((int)searchQuery.Skip - 1) * (int)searchQuery.Take)
                    .Take((int)searchQuery.Take);


            return new PagedListResult<T>
            {
                Entities = result.ToList(),
                Count = resultCount
            };
        }

        #endregion
    }
}
