using System;
using System.ComponentModel.DataAnnotations;

namespace TeamMashup.Core.Domain
{
    public abstract class Entity : IEntitySet, IDeletableEntity
    {
        public long Id { get; protected set; }

        [Required]
        public bool Deleted { get; protected set; }

        [Required]
        public DateTime CreatedDate { get; protected set; }

        [Required]
        public DateTime ModifiedDate { get; protected set; }

        public DateTime? DeletedDate { get; protected set; }

        public Entity()
        {
            //TODO: This is a bug that will cause Created and Modificated date to be changed all the time.
            CreatedDate = DateTime.UtcNow;
            ModifiedDate = DateTime.UtcNow;
        }

        protected virtual bool TryDeleteCascade(out string errorKey)
        {
            errorKey = string.Empty;
            return true;
        }

        public bool TryDelete(out string errorKey)
        {
            if (!TryDeleteCascade(out errorKey))
                return false;

            Deleted = true;
            DeletedDate = DateTime.UtcNow;

            return true;
        }
    }
}