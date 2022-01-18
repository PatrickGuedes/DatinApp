using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options) : base(options)
        {

        }
        public DbSet<AppUser> Users { get; set; }
        public DbSet<UserLike> Likes { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<UserLike>()
                .HasKey(key => new {key.SourceUserId, key.LikedUserId});

            builder.Entity<UserLike>()
                .HasOne(userLike => userLike.SourceUser)
                .WithMany(user => user.LikedUsers)
                .HasForeignKey(userLiked => userLiked.SourceUserId)
                .OnDelete(DeleteBehavior.Cascade);  // That might cause an error on SQL Server (use: NoAction)


            builder.Entity<UserLike>()
                .HasOne(userLiked => userLiked.LikedUser)
                .WithMany(user => user.LikedByUsers)
                .HasForeignKey(userLiked => userLiked.LikedUserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}