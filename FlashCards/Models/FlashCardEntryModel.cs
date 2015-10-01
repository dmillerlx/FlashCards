using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FlashCards.Models
{
    public class FlashCardEntry
    {
        public int FlashCardEntryId { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
    }
}