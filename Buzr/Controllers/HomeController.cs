using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Buzr.Models;
using BoomTown.FuzzySharp;
using Tweetinvi.Models;
using Tweetinvi;

namespace Buzr.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult TwitterAuth()
        {
            var appCreds = new ConsumerCredentials(MyCredentials.CONSUMER_KEY, MyCredentials.CONSUMER_SECRET);
            var redirectURL = "https://" + Request.Host.Value + "/Home/ValidateTwitterAuth";
            var authenticationContext = AuthFlow.InitAuthentication(appCreds, redirectURL);

            return new RedirectResult(authenticationContext.AuthorizationURL);
        }

        public ActionResult ValidateTwitterAuth()
        {
            if (Request.Query.ContainsKey("oauth_verifier") && Request.Query.ContainsKey("authorization_id"))
            {
                var verifierCode = Request.Query["oauth_verifier"];
                var authorizationId = Request.Query["authorization_id"];

                var userCreds = AuthFlow.CreateCredentialsFromVerifierCode(verifierCode, authorizationId);
                var user = Tweetinvi.User.GetAuthenticatedUser(userCreds);

                ViewBag.User = user;
            }

            return View();
        }

        public IActionResult Index()
        {
            using (var db = new CommentContext())
            {
                db.CommentReplies.Add(new CommentReply
                {
                    CommentText = "What is ur opening hours?",
                    ReplyText = "9-15 Mondays to Fridays and 10-14 Saturdays."
                });
                db.SaveChanges();
            }
            return RedirectToAction("TwitterAuth");
        }

        public IActionResult About()
        {
            var searchString = "What r ur opening hour?";
            using (var db = new CommentContext())
            {
                var bestMatches = db.CommentReplies.OrderByDescending(x => Fuzzy.PartialRatio(searchString, x.CommentText)).Take(3).ToList();
                return View("Contact", bestMatches);
            }
        }

        public IActionResult Contact()
        {
            using (var db = new CommentContext())
            {
                var replies = db.CommentReplies.ToList();
                return View(replies);
            }
        }

        public IActionResult Privacy()
        {

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
