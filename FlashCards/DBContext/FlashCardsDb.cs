using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using FlashCards.Models;

namespace FlashCards.DBContext
{
    public class FlashCardsDb : DbContext
    {
        public DbSet<FlashCardEntry> FlashCards { get; set; }
    }
}