using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MiniCrawler.Models
{
    public class WordRankVM
    {
        public string Keyword { get; set; }
        public int Rank { get; set; }
        public string PageName { get; set; }
        public string PageURL { get; set; }
        public string Title { get; set; }
        
    }
}