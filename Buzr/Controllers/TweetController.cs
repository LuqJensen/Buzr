using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Buzr.Models;
using BoomTown.FuzzySharp;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tweetinvi;
using Tweetinvi.Models;
using Tweetinvi.Parameters;
using Buzr.ViewModels;
using System.Text.RegularExpressions;

namespace Buzr.Controllers
{
    public class TweetController : Controller
    {
        private static ITwitterCredentials _credentials;
    
        public TweetController()
        {
            _credentials = MyCredentials.GenerateCredentials();
        }

        [HttpGet]
        public ActionResult FuzzyMatches(long id)
        {
            var tweet = Tweet.GetTweet(id);
            var text = tweet.Text;
            using (var db = new CommentContext())
            {
                var bestMatches = db.CommentReplies.OrderByDescending(x => Fuzzy.PartialRatio(x.CommentText, text)).Take(3).ToList();
                var model = new FuzzyMatch
                {
                    Tweet = tweet,
                    CommentReplies = bestMatches
                };
                return View(model);
            }
        }

        [HttpPost]
        public ActionResult Reply(long id, string text)
        {
            var tweet = Tweet.GetTweet(id);
            var publishedTweet = Auth.ExecuteOperationWithCredentials(_credentials, () =>
            {
                var reply = $"@{tweet.CreatedBy.ScreenName} {text}";
                return Tweet.PublishTweetInReplyTo(reply, id);
            });
            if (publishedTweet != null)
            {
                using (var db = new CommentContext())
                {
                    var reply = new CommentReply
                    {
                        CommentText = Regex.Replace(tweet.Text, @"^@[^\s]+[\s]", ""),
                        ReplyText = text
                    };
                    db.CommentReplies.Add(reply);
                    db.SaveChanges();
                }
            }
            return RedirectToAction("TweetPublished", new { id = publishedTweet?.Id, actionPerformed = "Publish", success = publishedTweet != null });
        }
    
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }
    
        [HttpPost]
        public ActionResult Index(string tweet, IFormFile file)
        {
            var fileBytes = GetByteArrayFromFile(file);
    
            var publishedTweet = Auth.ExecuteOperationWithCredentials(_credentials, () =>
            {
                var publishOptions = new PublishTweetOptionalParameters();
                if (fileBytes != null)
                {
                    publishOptions.MediaBinaries.Add(fileBytes);
                }
    
                return Tweet.PublishTweet(tweet, publishOptions);
            });
    
            return RedirectToAction("TweetPublished", new { id = publishedTweet?.Id, actionPerformed = "Publish", success = publishedTweet != null });
        }
    
        private byte[] GetByteArrayFromFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return null;
            }
    
            using (var memoryStream = new MemoryStream())
            {
                file.OpenReadStream().CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }
    
        public ActionResult TweetPublished(long? id, string actionPerformed, bool success = true)
        {
            ViewBag.TweetId = id;
            ViewBag.ActionType = actionPerformed;
            ViewBag.Success = success;
            return View();
        }
    }
}