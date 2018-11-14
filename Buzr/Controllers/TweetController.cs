using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Buzr.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tweetinvi;
using Tweetinvi.Models;
using Tweetinvi.Parameters;

namespace Buzr.Controllers
{
    public class TweetController : Controller
    {
        private readonly ITwitterCredentials _credentials;
    
        public TweetController()
        {
            _credentials = MyCredentials.GenerateCredentials();
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