using LoyalCode.Dmitriev.Ivan.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoyalCode.Dmitriev.Ivan.Infrustructure
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users => Set<User>();
        public DbSet<Currency> Currencies => Set<Currency>();
        public DbSet<UserFavoriteCurrency> Favorites => Set<UserFavoriteCurrency>();

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<User>(x =>
            {
                x.ToTable("users");
                x.HasKey(u => u.Id);
                x.Property(u => u.Name).IsRequired();
                x.Property(u => u.Password).IsRequired();
            });

            builder.Entity<Currency>(x =>
            {
                x.ToTable("currency");
                x.HasKey(c => c.Id);
                x.Property(c => c.Name).IsRequired();
                x.Property(c => c.Rate).HasColumnType("numeric");
            });

            builder.Entity<UserFavoriteCurrency>(x =>
            {
                x.ToTable("user_favorites");

                x.HasKey(f => new { f.UserId, f.CurrencyId });

                x.HasOne(f => f.User)
                    .WithMany(u => u.Favorites)
                    .HasForeignKey(f => f.UserId);

                x.HasOne(f => f.Currency)
                    .WithMany()
                    .HasForeignKey(f => f.CurrencyId);
            });
        }
    }
}
