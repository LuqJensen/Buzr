using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Buzr.Models;
using BoomTown.FuzzySharp;

namespace Buzr.Controllers
{
    public class HomeController : Controller
    {
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
            return View();
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
