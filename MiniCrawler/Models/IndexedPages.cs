namespace MiniCrawler.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class IndexedPages
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public IndexedPages()
        {
            PageKeyWords = new HashSet<PageKeyWords>();
        }

        [Key]
        public int PageID { get; set; }

        [StringLength(250)]
        public string PageName { get; set; }

        [StringLength(400)]
        public string PageURL { get; set; }

        public int? ParentID { get; set; }

        public DateTime DateCreated { get; set; }

        [StringLength(300)]
        public string ParentDirectory { get; set; }

        public bool? IsIndexed { get; set; }

        [StringLength(50)]
        public string Title { get; set; }

        public int? IndexedSiteID { get; set; }

        public virtual IndexedSites IndexedSites { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PageKeyWords> PageKeyWords { get; set; }
    }
}
