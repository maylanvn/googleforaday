namespace MiniCrawler.Models
{
    using System.ComponentModel.DataAnnotations;

    public partial class PageKeyWords
    {
        [Key]
        public int PageKeyWordID { get; set; }

        public int? PageID { get; set; }

        [StringLength(50)]
        public string Keyword { get; set; }

        public int? KeywordCount { get; set; }

        public virtual IndexedPages IndexedPages { get; set; }
    }
}
