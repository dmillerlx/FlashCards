using FlashCards.DBContext;
using FlashCards.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace FlashCards.API
{
    public class FlashCardsController : ApiController
    {
        private FlashCardsDb db = null;

        public FlashCardsController()
        {
            db = new FlashCardsDb();
            db.Configuration.ProxyCreationEnabled = false;
        }


        // GET api/<controller>
        public IEnumerable<FlashCardEntry> Get()
        {
            return db.FlashCards;
        }

        // GET api/<controller>/5
        [ResponseType(typeof(FlashCardEntry))]
        public IHttpActionResult Get(int id)
        {
            using (var db = new FlashCardsDb())
            {
                var result = (from c in db.FlashCards
                             where c.FlashCardEntryId == id
                             select new 
                             {
                                 FlashCardEntryId = c.FlashCardEntryId,
                                 Question = c.Question,
                                 Answer = c.Answer
                             }).SingleOrDefault();

                if (result == null)
                    return NotFound();

                FlashCardEntry ent = new FlashCardEntry();
                ent.Answer = result.Answer;
                ent.FlashCardEntryId = result.FlashCardEntryId;
                    ent.Question = result.Question;

                return Ok(ent);
            }

        }

        // POST api/<controller>
        public void Post([FromBody]string value)
        {
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}