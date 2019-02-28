﻿using Data.Contracts;
using Data.Contracts.Models.Entities;
using Search.Implementation;
using System;
using System.Linq.Expressions;

namespace Data.Implementation.Repositories
{
    public class RepositoryOfPostRating : DefaultRepository<PostRatingEntity>
    {
        private readonly ServiceOfSearch serviceOfSearch;
        public RepositoryOfPostRating(ApplicationDbContext context, ServiceOfSearch serviceOfSearch) : base(context)
        {
            this.serviceOfSearch = serviceOfSearch;
        }

        public override PostRatingEntity Create(PostRatingEntity entity)
        {
            IRepository<PostEntity> repositoryOfPost = new RepositoryOfPost(context, serviceOfSearch);

            var Post = entity.Post;
            if (Post == null)
            {
                Post = repositoryOfPost.Read(a => a.Id == entity.PostId);
            }

            Post.SumOfScore += entity.Score;
            Post.CountOfScore++;
            repositoryOfPost.Update(Post);

            return base.Create(entity);
        }

        public override void Delete(PostRatingEntity entity)
        {
            IRepository<PostEntity> repositoryOfPost = new RepositoryOfPost(context, serviceOfSearch);
            
            var Post = entity.Post;
            if (Post == null)
            {
                Post = repositoryOfPost.Read(a => a.Id == entity.PostId);
            }

            Post.SumOfScore -= entity.Score;
            Post.CountOfScore--;
            repositoryOfPost.Update(Post);
            
            base.Delete(entity);
        }

        public override void Update(PostRatingEntity entity, params Expression<Func<PostRatingEntity, object>>[] properties)
        {
            IRepository<PostEntity> repositoryOfPost = new RepositoryOfPost(context, serviceOfSearch);

            var lastPostRating = Read(a => a.Id == entity.Id);
            var Post = entity.Post;
            if (Post == null)
            {
                Post = repositoryOfPost.Read(a => a.Id == entity.PostId);
            }

            Post.SumOfScore -= lastPostRating.Score;
            Post.SumOfScore += entity.Score;
            repositoryOfPost.Update(Post);

            base.Update(entity, properties);
        }
    }
}
