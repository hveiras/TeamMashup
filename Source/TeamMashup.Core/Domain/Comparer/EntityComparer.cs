using System.Collections.Generic;

namespace TeamMashup.Core.Domain
{
    public class EntityComparer : IEqualityComparer<IEntitySet>
    {
        public bool Equals(IEntitySet entity1, IEntitySet entity2)
        {
            return entity1.Id == entity2.Id;
        }

        public int GetHashCode(IEntitySet entity)
        {
            return entity.Id.GetHashCode();
        }
    }
}
