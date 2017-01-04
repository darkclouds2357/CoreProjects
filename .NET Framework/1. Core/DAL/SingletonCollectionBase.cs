using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;

namespace DAL
{
    public class SingletonCollectionBase
    {
        protected Dictionary<Type, EntitySetBase> MappingCache = new Dictionary<Type, EntitySetBase>();
    }
}
