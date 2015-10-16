using FlashCards.DBContext;
using FlashCards.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
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

        [HttpPost]
        
        public bool Post(string addQuestionPassword, [FromBody]string questionList)
        {
            if (addQuestionPassword != "fred.51")
                return false;


            //Parse incoming list into entries
            List<FlashCardEntry> entries = ParseData(questionList);
            
            //Add new entries into the datbase

            using (var db = new FlashCardsDb())
            {
                try
                {
                    
                    foreach (FlashCardEntry e in entries)
                    {
                        bool found = false;
                        foreach (FlashCardEntry dbEntry in db.FlashCards)
                        {
                            if (dbEntry.Question.ToUpper().Trim() == e.Question.ToUpper().Trim())
                            {
                                //update answer if found
                                dbEntry.Answer = e.Answer;
                                found = true;
                                break;
                            }

                            if (found)
                                break;
                        }

                        //not found, so insert
                        if (!found)
                            db.FlashCards.Add(e);
                    }

                    db.SaveChanges();


                    //Now redo the numbering
                    int number = 0;
                    foreach (FlashCardEntry ent in db.FlashCards)
                    {
                        ent.FlashCardQuestionNumber = number++;
                    }

                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    return false;
                }

                

                
            }

            return true;

        }

        private List<FlashCardEntry> ParseData(string data)
        {
            StringReader sr = new StringReader(data);

            string line = sr.ReadLine();

            string question = string.Empty;
            StringBuilder answerBuilder = new StringBuilder();

            List<FlashCardEntry> entries = new List<FlashCardEntry>();

            do
            {
                if (line != null)
                {
                    line = line.Trim();

                    //On empty line, start over
                    if (string.IsNullOrEmpty(line))
                    {
                        if (!string.IsNullOrEmpty(question) && answerBuilder.Length > 0)
                        {
                            entries.Add(new FlashCardEntry() { Question = question, Answer = answerBuilder.ToString() });
                        }

                        question = string.Empty;
                        answerBuilder.Clear();
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(question))
                        {
                            question = line;
                        }
                        else
                        {
                            answerBuilder.AppendLine(line);
                        }
                    }
                }
                line = sr.ReadLine();
            } while (line != null);

            //Finally check again to see if quesiton/answer are populated.  This can occur if there is no white space
            //at the end of the data stream
            if (!string.IsNullOrEmpty(question) && answerBuilder.Length > 0)
            {
                entries.Add(new FlashCardEntry() { Question = question, Answer = answerBuilder.ToString() });
            }

            return entries;
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

        [ResponseType(typeof(FlashCardEntry))]
        public IHttpActionResult GetRandomEntry()
        {
            FlashCardEntry ent = null;

            using (var db = new FlashCardsDb())
            {
                //Get count of entries
                var entries = db.FlashCards.Count();

                //Create random number generator
                Random rnd = new Random((int)System.DateTime.Now.Ticks);

                //set max tries
                int count = 0;

                do
                {
                    //get random number value
                    int value = rnd.Next(entries);

                    var result = (from c in db.FlashCards
                                  where c.FlashCardEntryId == value
                                  select new
                                  {
                                      FlashCardEntryId = c.FlashCardEntryId,
                                      Question = c.Question,
                                      Answer = c.Answer
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

        public bool Delete(int id, [FromBody]string deleteQuestionPassword)
        {

            if (deleteQuestionPassword != "fred.51")
                return false;

            try
            {
                using (var db = new FlashCardsDb())
                {
                    FlashCardEntry e = (from n in db.FlashCards where n.FlashCardEntryId == id select n).FirstOrDefault();
                    if (e == null)
                        return false;

                    db.FlashCards.Remove(e);
                    db.SaveChanges();

                    //Now redo the numbering
                    int number = 0;
                    foreach (FlashCardEntry ent in db.FlashCards)
                    {
                        ent.FlashCardQuestionNumber = number++;
                    }

                    db.SaveChanges();

                    return true;
                }
            }
            catch (Exception ex)
            {

                return false;
            }
        }
    }
}