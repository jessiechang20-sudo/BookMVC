using Microsoft.EntityFrameworkCore;
using BookMvc.Models.Entities;

namespace BookMvc.Data
{
    public sealed class BookDbContext :DbContext
    {
        public BookDbContext(DbContextOptions<BookDbContext> options) : base(options)
        {
        }
        public DbSet<Book> Books => Set<Book>(); 
        protected override void OnModelCreating(ModelBuilder modelBuilder) 
        {
            modelBuilder.Entity<Book>(b =>
            { 
                b.ToTable("books");  
                b.HasKey(x => x.Id);   //PK
                b.Property(x => x.Isbn).HasMaxLength(20).IsRequired(); 
                b.Property(x => x.Title).HasMaxLength(200).IsRequired();
                b.Property(x => x.Author).HasMaxLength(100);
                b.Property(x => x.CreatedAt).IsRequired();
                b.Property(x => x.BCoverFileName).HasMaxLength(200);
                b.HasIndex(x => x.Isbn).IsUnique();//建立唯一索引，確保 ISBN 不重複（Isbn 已 Required，所以不會有 NULL）

            } );
        }

    }
}
