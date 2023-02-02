using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ShitheadApi.Attributes;
using ShitheadApi.Models.Entities;

namespace ShitheadApi
{
    public class DatabaseContext : IdentityDbContext<User, Role, Guid>
    {
        /// <summary>
        /// Database context
        /// Via this method the database can be accessed
        /// </summary>
        /// <param name="options"></param>
        public DatabaseContext(DbContextOptions options) : base(options) { }

        public DbSet<Card> Card { get; set; }

        public DbSet<UserFriend> Friend { get; set; }

        public DbSet<Game> Game { get; set; }

        public DbSet<Player> Player { get; set; }

        public DbSet<User> User { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Card>()
                .HasOne(c => c.Game)
                .WithMany(g => g.Cards)
                .HasForeignKey(c => c.GameId);

            modelBuilder.Entity<UserFriend>()
                .HasOne(uf => uf.User)
                .WithMany(u => u.Friends)
                .HasForeignKey(uf => uf.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserFriend>()
                .HasOne(uf => uf.Friend)
                .WithMany()
                .HasForeignKey(uf => uf.FriendId);

            modelBuilder.Entity<Player>()
                .HasOne(p => p.User);

            modelBuilder.Entity<Player>()
                .HasOne(p => p.Game)
                .WithMany(g => g.Players)
                .HasForeignKey(p => p.GameId);

            // Renames the AspNetEntities to beautiful table names  
            modelBuilder.Entity<User>().ToTable("User");
            modelBuilder.Entity<Role>().ToTable("Role");
            modelBuilder.Entity<IdentityUserRole<Guid>>().ToTable("UserRole");
            modelBuilder.Entity<IdentityUserClaim<Guid>>().ToTable("UserClaim");
            modelBuilder.Entity<IdentityUserLogin<Guid>>().ToTable("UserLogin");
            modelBuilder.Entity<IdentityRoleClaim<Guid>>().ToTable("RoleClaim");
            modelBuilder.Entity<IdentityUserToken<Guid>>().ToTable("UserToken");

            // Seeds context with data
            Guid theShitHeadGameAdminId = Guid.NewGuid();
            Guid theShitHeadGameUserId = Guid.NewGuid();

            // Seeds and creates our default user roles
            modelBuilder.Entity<Role>().HasData(
                new Role
                {
                    Id = theShitHeadGameAdminId,
                    Name = AuthorizationRole.ShitheadAdmin,
                    NormalizedName = AuthorizationRole.ShitheadAdmin.ToUpper(),
                    ConcurrencyStamp = Guid.NewGuid().ToString()
                },
                new Role
                {
                    Id = theShitHeadGameUserId,
                    Name = AuthorizationRole.ShitheadUser,
                    NormalizedName = AuthorizationRole.ShitheadUser.ToUpper(),
                    ConcurrencyStamp = Guid.NewGuid().ToString()
                });
        }
    }
}
