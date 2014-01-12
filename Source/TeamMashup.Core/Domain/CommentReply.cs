using System;

namespace TeamMashup.Core.Domain
{
    public class CommentReply : IEntitySet
    {
        public long Id { get; set; }

        public long UserId { get; set; }

        public string Message { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
