using FlashCards.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FlashCards.Controllers
{
    public class HomeController : Controller
    {
        private FlashCardsDb db = null;

        public ActionResult Index()
        {
            ViewBag.FlashCardCount = "--";

            //Get number of flash cards
            using (var db = new FlashCardsDb())
            {
                //Get count of entries using question number field.        
                var lastEntry = db.FlashCards.OrderByDescending(i => i.FlashCardQuestionNumber).FirstOrDefault();

                if (lastEntry != null)
                {
                    ViewBag.FlashCardCount = lastEntry.FlashCardQuestionNumber;
                }
            }

            ViewBag.Title = "Home Page";

            return View();
        }

        public ActionResult AddQuestions()
        {
            ViewBag.Title = "Add Questions";

            return View();
        }
    }
}
