namespace MiniCrawler.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class IndexedSites
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public IndexedSites()
        {
            IndexedPages = new HashSet<IndexedPages>();
        }

        [Key]
        public int IndexedSiteID { get; set; }

        [StringLength(300)]
        public string Domain { get; set; }

        [StringLength(300)]
        public string InitialPage { get; set; }

        public DateTime? DateCreated { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<IndexedPages> IndexedPages { get; set; }
    }
}
