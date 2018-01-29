namespace MiniCrawler.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class CrawlerDB : DbContext
    {
        public CrawlerDB()
            : base("name=CrawlerDB")
        {
        }

        public virtual DbSet<IndexedPages> IndexedPages { get; set; }
        public virtual DbSet<IndexedSites> IndexedSites { get; set; }
        public virtual DbSet<PageKeyWords> PageKeyWords { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<IndexedPages>()
                .Property(e => e.PageName)
                .IsUnicode(false);

            modelBuilder.Entity<IndexedPages>()
                .Property(e => e.PageURL)
                .IsUnicode(false);

            modelBuilder.Entity<IndexedPages>()
                .Property(e => e.ParentDirectory)
                .IsUnicode(false);

            modelBuilder.Entity<IndexedSites>()
                .Property(e => e.Domain)
                .IsUnicode(false);

            modelBuilder.Entity<IndexedSites>()
                .Property(e => e.InitialPage)
                .IsUnicode(false);

            modelBuilder.Entity<PageKeyWords>()
                .Property(e => e.Keyword)
                .IsUnicode(false);
        }
    }
}
