using DAL.Entites;
using System;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Linq;

namespace DAL.Repositories.Extension
{
    public static class RepositoryExtension
    {
        private static string _efNamespaceName;
        private static MetadataWorkspace _metadataWorkspace;


        public static void Init(Type objectType, IDbContext context)
        {
            _metadataWorkspace = (context as IObjectContextAdapter).ObjectContext.MetadataWorkspace;
            _efNamespaceName = _metadataWorkspace.GetItems<EntityType>(DataSpace.CSpace).First().NamespaceName;
        }

        private static string FindForeignKey(Type entityType, Type toRole)
        {
            string namespaceName = $"{_efNamespaceName}.{entityType.Name}";

            var entityMetadata = _metadataWorkspace.GetItem<EntityType>(namespaceName, DataSpace.CSpace);
            // EntityType is many to one - CollectionType is one to many.
            var entityNavigationProperties = entityMetadata.NavigationProperties.Where(x => x.TypeUsage.EdmType.BuiltInTypeKind == BuiltInTypeKind.EntityType && x.Name == toRole.Name);

            foreach (var navProp in entityNavigationProperties)
            {
                var relType = navProp.RelationshipType as AssociationType;
                if (relType != null)
                {
                    // if Entity is Provision, efFkProp.Name is Forgeign Key Name
                    var efFkProp = relType.ReferentialConstraints.FirstOrDefault()?.ToProperties.FirstOrDefault();
                    return efFkProp.Name;
                }
            }
            return null;
        }
    }
}
