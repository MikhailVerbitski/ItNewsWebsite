﻿using Data.Contracts.Models.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Data.Implementation
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUserEntity>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }
        
        public DbSet<CommentEntity> Comments { get; set; }
        public DbSet<CommentLikeEntity> CommentLikes { get; set; }
        public DbSet<ImageEntity> ImageEntities { get; set; }
        public DbSet<PostEntity> Posts { get; set; }
        public DbSet<PostRatingEntity> PostRatings { get; set; }
        public DbSet<PostTagEntity> PostTags { get; set; }
        public DbSet<RoleEntity> MyRoles { get; set; }
        public DbSet<SectionEntity> Sections { get; set; }
        public DbSet<TagEntity> Tags { get; set; }
        public DbSet<UserProfileEntity> UserProfiles { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            RoleEntity adminRole = new RoleEntity { Id = 1, Name = "admin" };
            RoleEntity userRole = new RoleEntity { Id = 2, Name = "user" };
            builder.Entity<RoleEntity>().HasData(new RoleEntity[] { adminRole, userRole });


            var sectionsName = new string[] { "Java", "C#", "C++", "Algorithms", "Machine Learning" };
            var sections = Enumerable
                .Range(0, sectionsName.Length)
                .Select(a => new SectionEntity()
                {
                    Id = a + 1,
                    Name = sectionsName[a]
                })
            .ToArray();
            builder.Entity<SectionEntity>().HasData(sections);


            var ApplicationUser = builder.Entity<ApplicationUserEntity>();
            ApplicationUser
                .HasOne(a => a.UserProfile)
                .WithOne(a => a.ApplicationUser)
                .OnDelete(DeleteBehavior.Cascade)
                .HasForeignKey<ApplicationUserEntity>(a => a.UserProfileId);
            ApplicationUser
                .HasOne(a => a.Role)
                .WithMany(a => a.ApplicationUsers)
                .HasForeignKey(a => a.RoleId);


            var UserProfile = builder.Entity<UserProfileEntity>();
            UserProfile
                .HasOne(a => a.ApplicationUser)
                .WithOne(a => a.UserProfile)
                .OnDelete(DeleteBehavior.Cascade)
                .HasForeignKey<ApplicationUserEntity>(a => a.UserProfileId);
            UserProfile
                .HasMany(a => a.Posts)
                .WithOne(a => a.UserProfile)
                .HasForeignKey(a => a.UserProfileId);
            UserProfile
                .HasMany(a => a.Comments)
                .WithOne(a => a.UserProfile)
                .HasForeignKey(a => a.UserProfileId);
            UserProfile
                .HasMany(a => a.PostRatings)
                .WithOne(a => a.UserProfile)
                .HasForeignKey(a => a.UserProfileId);


            var Comment = builder.Entity<CommentEntity>();
            Comment
                .HasOne(a => a.UserProfile)
                .WithMany(a => a.Comments)
                .HasForeignKey(a => a.UserProfileId);
            Comment
                .HasMany(a => a.Likes)
                .WithOne(a => a.Comment)
                .HasForeignKey(a => a.CommentId)
                .OnDelete(DeleteBehavior.Cascade);
            Comment
                .HasOne(a => a.Post)
                .WithMany(a => a.Comments)
                .HasForeignKey(a => a.PostId);


            var CommentLike = builder.Entity<CommentLikeEntity>();
            CommentLike
                .HasOne(a => a.Comment)
                .WithMany(a => a.Likes)
                .HasForeignKey(a => a.CommentId);
            CommentLike
                .HasKey(a => new { a.CommentId, a.UserProfileId });


            var Post = builder.Entity<PostEntity>();
            Post
                .HasOne(a => a.Section)
                .WithMany(a => a.Posts)
                .HasForeignKey(a => a.SectionId);
            Post
                .HasMany(a => a.Comments)
                .WithOne(a => a.Post)
                .HasForeignKey(a => a.PostId)
                .OnDelete(DeleteBehavior.Cascade);
            Post
                .HasOne(a => a.UserProfile)
                .WithMany(a => a.Posts)
                .HasForeignKey(a => a.UserProfileId);
            Post
                .HasMany(a => a.Tags)
                .WithOne(a => a.Post)
                .HasForeignKey(a => a.PostId);
            Post
                .HasMany(a => a.Images)
                .WithOne(a => a.Post)
                .HasForeignKey(a => a.PostId);
            Post
                .HasMany(a => a.PostRatings)
                .WithOne(a => a.Post)
                .HasForeignKey(a => a.PostId)
                .OnDelete(DeleteBehavior.Cascade);


            var Role = builder.Entity<RoleEntity>();
            Role
                .HasMany(a => a.ApplicationUsers)
                .WithOne(a => a.Role)
                .HasForeignKey(a => a.RoleId);


            var Section = builder.Entity<SectionEntity>();
            Section
                .HasMany(a => a.Posts)
                .WithOne(a => a.Section)
                .HasForeignKey(a => a.SectionId);


            var Tag = builder.Entity<TagEntity>();
            Tag
                .HasMany(a => a.Posts)
                .WithOne(a => a.Tag)
                .HasForeignKey(a => a.TagId);


            var Image = builder.Entity<ImageEntity>();
            Image
                .HasOne(a => a.Post)
                .WithMany(a => a.Images)
                .HasForeignKey(a => a.PostId);


            var PostTag = builder.Entity<PostTagEntity>();
            PostTag
                .HasOne(a => a.Post)
                .WithMany(a => a.Tags)
                .HasForeignKey(a => a.PostId)
                .OnDelete(DeleteBehavior.Cascade);
            PostTag
                .HasOne(a => a.Tag)
                .WithMany(a => a.Posts)
                .HasForeignKey(a => a.TagId);


            var PostRating = builder.Entity<PostRatingEntity>();
            PostRating
                .HasOne(a => a.Post)
                .WithMany(a => a.PostRatings);
            PostRating
                .HasOne(a => a.UserProfile)
                .WithMany(a => a.PostRatings);

            base.OnModelCreating(builder);
        }
    }
}
