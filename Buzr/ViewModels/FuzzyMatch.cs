using Buzr.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tweetinvi.Models;

namespace Buzr.ViewModels
{
    public class FuzzyMatch
    {
        public ITweet Tweet { get; set; }
        public IEnumerable<CommentReply> CommentReplies { get; set; }
    }
}
