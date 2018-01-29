using System.Collections.Generic;

namespace MiniCrawler.Models
{
    public class SearchResult
    {
        public List<string> Links { get; set; }
        public List<WordRankVM> KeyWordRankingList { get; set; }
        public string PageName { get; set; }
        public string ParentDirectory { get; set; }
        public string SearchContent { get; set; }
        public string TextContent { get; set; }
        public int ParentID { get; set; }
        public string PageURL { get; set; }
        public int PageID { get; set; }
        public string Title { get; set; }
        public int IndexedSiteID { get; set; }
        
    }
}