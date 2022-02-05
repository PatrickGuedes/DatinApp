using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class DataContext : IdentityDbContext<AppUser, AppRole, int, 
                                                IdentityUserClaim<int>, AppUserRole,
                                                IdentityUserLogin<int>, 
                                                IdentityRoleClaim<int>,  
                                                IdentityUserToken<int>>
    {
        public DataContext(DbContextOptions options) : base(options)
        {

        }
        public DbSet<UserLike> Likes { get; set; }
        public DbSet<Message> Messages { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<AppUser>()
                .HasMany(user => user.UserRoles)
                .WithOne(user => user.User)
                .HasForeignKey(user => user.UserId)
                .IsRequired();


            builder.Entity<AppRole>()
                .HasMany(user => user.UserRoles)
                .WithOne(user => user.Role)
                .HasForeignKey(user => user.RoleId)
                .IsRequired();

            builder.Entity<UserLike>()
                .HasKey(key => new {key.SourceUserId, key.LikedUserId});

            builder.Entity<UserLike>()
                .HasOne(like => like.SourceUser)
                .WithMany(user => user.LikedUsers)
                .HasForeignKey(liked => liked.SourceUserId)
                .OnDelete(DeleteBehavior.Cascade);  // That might cause an error on SQL Server (use: NoAction)

            builder.Entity<UserLike>()
                .HasOne(like => like.LikedUser)
                .WithMany(user => user.LikedByUsers)
                .HasForeignKey(like => like.LikedUserId)
                .OnDelete(DeleteBehavior.Cascade);


            builder.Entity<Message>()
                .HasOne(message => message.UserRecipient)
                .WithMany(user => user.MessagesReceived)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Message>()
                .HasOne(message => message.UserSender)
                .WithMany(user => user.MessagesSent)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}