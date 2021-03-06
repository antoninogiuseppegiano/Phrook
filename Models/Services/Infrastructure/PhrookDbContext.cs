
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Phrook.Models.Entities;

#nullable disable

namespace Phrook.Models.Services.Infrastructure
{
    public partial class PhrookDbContext : IdentityDbContext<ApplicationUser>
    {
        public PhrookDbContext(DbContextOptions<PhrookDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Book> Books { get; set; }
        public virtual DbSet<LibraryBook> LibraryBooks { get; set; }
        public virtual DbSet<Wishlist> Wishlist { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
			base.OnModelCreating(modelBuilder);
			
            modelBuilder.Entity<Book>(entity =>
            {
				entity.ToTable("Books");
				entity.HasKey(book => book.Id); //unnecessary
				// entity.Property(book => book.RowVersion).IsRowVersion();

				entity.Property(e => e.BookId).IsRequired();

				#region mapping autogenerated by reverse engineering tool
				/*
                entity.Property(e => e.Author)
                    .IsRequired()
                    .HasColumnType("TEXT (100)");

                entity.Property(e => e.Description).HasColumnType("TEXT (10000)");

                entity.Property(e => e.ImagePath).HasColumnType("TEXT (100)");

                entity.Property(e => e.Isbn)
                    .IsRequired()
                    .HasColumnType("TEXT (100)")
                    .HasColumnName("ISBN");

                entity.Property(e => e.ReadingState).HasColumnType("TEXT (100)");

                entity.Property(e => e.Tag).HasColumnType("TEXT (100)");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasColumnType("TEXT (100)");
				*/
				#endregion
            });
			
			modelBuilder.Entity<LibraryBook>(entity =>
            {
				entity.ToTable("LibraryBooks");
				entity.HasKey(libraryBook => libraryBook.Id); //unnecessary
				entity.Property(libraryBook => libraryBook.RowVersion).IsRowVersion();
				entity.HasIndex(libraryBook => new { libraryBook.UserId, libraryBook.BookId }).IsUnique();

                entity.Property(e => e.Rating).HasColumnType("NUMERIC").HasDefaultValueSql("0");;
                entity.Property(e => e.ReadingState).HasColumnType("TEXT (100)");
                entity.Property(e => e.Tag).HasColumnType("TEXT (100)");

                entity.Property(e => e.InitialTime)
					.HasColumnType("TEXT");
                entity.Property(e => e.FinalTime)
					.HasColumnType("TEXT");
					
				entity.HasOne(libraryBook => libraryBook.User)
					.WithMany(user => user.Library)
					.HasForeignKey(libraryBook => libraryBook.UserId);

				entity.HasOne(libraryBook => libraryBook.Book)
					.WithMany(book => book.InLibrary)
					.HasPrincipalKey(book => book.BookId)
					.HasForeignKey(libraryBook => libraryBook.BookId);
            });

			modelBuilder.Entity<Wishlist>(entity =>
            {
				entity.ToTable("Wishlists");
				entity.HasKey(wishlist => wishlist.Id); //unnecessary
				entity.HasIndex(wishlist => new { wishlist.UserId, wishlist.BookId }).IsUnique();

                entity.Property(e => e.UserId).HasColumnType("TEXT");
                entity.Property(e => e.BookId).HasColumnType("TEXT");
                entity.Property(e => e.Isbn).HasColumnType("TEXT");
                entity.Property(e => e.Title).HasColumnType("TEXT");
                entity.Property(e => e.NormalizedTitle).HasColumnType("TEXT");
                entity.Property(e => e.ImagePath).HasColumnType("TEXT");
                entity.Property(e => e.Author).HasColumnType("TEXT");

				entity.HasOne(wishlist => wishlist.User)
					.WithMany(user => user.Whishes)
					.HasForeignKey(wishlist => wishlist.UserId);

            });


            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
