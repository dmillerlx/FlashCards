using FlashCards.DBContext;
using FlashCards.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace FlashCards.API
{
    public class FlashCardsRandomController : ApiController
    {
        [HttpGet]
        public IHttpActionResult Get()
        {
            FlashCardEntry ent = null;

            using (var db = new FlashCardsDb())
            {
                //Get count of entries using question number field.        
                var lastEntry = db.FlashCards.OrderByDescending(i => i.FlashCardQuestionNumber).FirstOrDefault();

                if (lastEntry == null)
                    return NotFound();

                //Create random number generator
                Random rnd = new Random((int)System.DateTime.Now.Ticks);

                //set max tries
                int count = 0;

                do
                {
                    //get random number value
                    int value = rnd.Next(lastEntry.FlashCardQuestionNumber + 1) ;

                    var result = (from c in db.FlashCards
                                  where c.FlashCardQuestionNumber == value
                                  select new
                                  {
                                      FlashCardEntryId = c.FlashCardEntryId,
                                      Question = c.Question,
                                      Answer = c.Answer,
                                      FlashCardQuestionNumber = c.FlashCardQuestionNumber
                                  }).SingleOrDefault();

                    if (result != null)
                    {
                        ent = new FlashCardEntry();
                        ent.Answer = result.Answer;
                        ent.FlashCardEntryId = result.FlashCardEntryId;
                        ent.Question = result.Question;
                    }

                } while (ent == null && count < 10);


                if (ent == null)
                    return NotFound();



                return Ok(ent);
            }

        }
    }
}
