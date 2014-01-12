using System;
using System.Collections.Generic;

namespace TeamMashup.Models.Internal
{
    public class CommentPageModel
    {
        public ICollection<CommentModel> Comments {get; set;}

        public CommentPageModel()
        {
            this.Comments = new List<CommentModel>();
        }
    }

    public class CommentModel
    {
        public long Id { get; set; }

        public long UserId { get; set; }

        public string UserName { get; set; }

        public string Message { get; set; }

        public ICollection<CommentReplyModel> Replies { get; set; }
    }

    public class CommentReplyModel
    {
        public long Id { get; set; }

        public long CommentId { get; set; }

        public long UserId { get; set; }

        public string UserName { get; set; }

        public string Message { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}