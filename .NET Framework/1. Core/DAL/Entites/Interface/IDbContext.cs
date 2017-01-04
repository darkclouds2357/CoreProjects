using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace DAL.Entites
{
    public interface IDbContext : IDisposable, IObjectContextAdapter
    {
        DbSet<T> Set<T>() where T : BaseEntity;
        
        Database Database { get; }

        DbChangeTracker ChangeTracker { get; }

        DbContextConfiguration Configuration { get; }

        DbEntityEntry<T> Entry<T>(T entity) where T : BaseEntity;

        int SaveChanges();

        void Dispose();

        DbSet Set(Type entityType);
    }
}
