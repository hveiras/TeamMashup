using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using TeamMashup.Core.Enums;

namespace TeamMashup.Core.Domain
{
    public class Comment : IEntitySet, ISubscriptionQueryable
    {
        public long Id { get; set; }

        public long UserId { get; set; }

        public long SubscriptionId { get; set; }

        public string Message { get; set; }

        public virtual ICollection<CommentReply> Replies {get; set;}

        public int ScopeValue { get; set; }

        public CommentScope Scope 
        {
            get { return (CommentScope)ScopeValue; }
            set { ScopeValue = (int)value; } 
        }

        public DateTime CreatedDate { get; set; }

        public Comment()
        {
            CreatedDate = DateTime.UtcNow;
        }

        public Comment(long userId, string message, CommentScope scope) : this()
        {
            UserId = userId;
            Message = message;
            Scope = scope;
        }

        public Comment(long userId, long subscriptionId, string message, CommentScope scope) : this(userId, message, scope)
        {
            SubscriptionId = subscriptionId;
        }
    }

    public class CommentConfiguration : EntityTypeConfiguration<Comment>
    {
        public CommentConfiguration()
        {
            Ignore(x => x.Scope);

            Property(x => x.Message)
                .IsRequired();
        }
    }
}
