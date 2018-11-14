using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Buzr.Models
{
    public class CommentReply
    {
        public int Id { get; set; }
        public int CommentId { get; set; }
        public string CommentText { get; set; }
        public string ReplyText { get; set; }
    }
}
