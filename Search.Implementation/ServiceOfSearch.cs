using AutoMapper;
using Search.Contracts.Models;
using Nest;
using System.Linq;
using System.Collections.Generic;
using System;
using System.Linq.Expressions;
using Data.Contracts.Models.Entities;

namespace Search.Implementation
{
    public class ServiceOfSearch
    {
        private readonly IMapper mapper;
        private Dictionary<Type, Func<ElasticClient>> clients;
        private Dictionary<Type, Type> config;
        public ServiceOfSearch(IMapper mapper)
        {
            this.mapper = mapper;
            clients = new Dictionary<Type, Func<ElasticClient>>(new[] {
                new KeyValuePair<Type, Func<ElasticClient>>(typeof(PostSearch), () => GetClient<PostSearch>(b => b.Id, "post")),
                new KeyValuePair<Type, Func<ElasticClient>>(typeof(CommentSearch), () => GetClient<CommentSearch>(b => b.Id, "comment")),
                new KeyValuePair<Type, Func<ElasticClient>>(typeof(ApplicationUserSearch), () => GetClient<ApplicationUserSearch>(b => b.Id, "user"))
            });
            config = new Dictionary<Type, Type>(new[]
            {
                new KeyValuePair<Type, Type>(typeof(PostEntity), typeof(PostSearch)),
                new KeyValuePair<Type, Type>(typeof(CommentEntity), typeof(CommentSearch)),
                new KeyValuePair<Type, Type>(typeof(ApplicationUserEntity), typeof(ApplicationUserSearch))
            });
        }
        private ElasticClient GetClient<T>(Expression<Func<T, object>> idProperty, string IndexName) where T:class
        {
            var connectionSettings = new ConnectionSettings(new Uri("http://localhost:9200/"))
                .DefaultMappingFor<T>(i => i
                    .IdProperty(idProperty)
                    .IndexName(IndexName)
                    .TypeName("dafault"))
                .DefaultIndex(IndexName);
            var client = new ElasticClient(connectionSettings);
            return client;
        }
        //public IIndexResponse Create<TResult>(object entity) where TResult : class
        //{
        //    var searchEntity = mapper.Map <TResult>(entity);
        //    var client = clients.GetValueOrDefault(typeof(TResult))();
        //    var indexResponse = client.IndexDocument(searchEntity);
        //    return indexResponse;
        //}

        public IIndexResponse Create<T>(T entity) where T: class
        {
            var searchEntityType = config.GetValueOrDefault(typeof(T));
            var searchEntity = mapper.Map(entity, typeof(T), searchEntityType);
            var client = clients.GetValueOrDefault(searchEntityType)();
            var indexResponse = client.IndexDocument(searchEntity);
            return indexResponse;
        }
        public void Update<T>(T entity) where T : class => Create<T>(entity);
        public IDeleteResponse DeletePost(PostEntity postEntity)
        {
            var post = mapper.Map<PostEntity, PostSearch>(postEntity);
            var client = clients.GetValueOrDefault(typeof(PostSearch))();
            var response = client.Delete<PostSearch>(post.Id, a => a.Index("post").Type("object"));
            return response;
        }
        public void DeleteComment(CommentEntity commentEntity)
        {
            var comment = mapper.Map<CommentEntity, CommentSearch>(commentEntity);
            var client = clients.GetValueOrDefault(typeof(CommentSearch))();
            client.Delete<CommentSearch>(comment.Id, a => a.Index("comment").Type("object"));
        }
        public void DeleteUser(ApplicationUserEntity userEntity)
        {
            var user = mapper.Map<ApplicationUserEntity, ApplicationUserSearch>(userEntity);
            var client = clients.GetValueOrDefault(typeof(ApplicationUserSearch))();
            client.Delete<ApplicationUserSearch>(user.Id, a => a.Index("user").Type("object"));
        }
        public IEnumerable<int> SearchPosts(string key, int skip, int take)
        {
            var client = GetClient<PostSearch>(a => a.Id, "post");
            var response = client.Search<PostSearch>(a => a
                .AllTypes()
                .From(skip)
                .Size(take)
                .Query(q => q.MultiMatch(c => c.Fields(f => f.Field(p => p.Header).Field(p => p.Content)).Query(key))));
            var Ids = response.Documents.Select(a => a.Id).ToList();
            return Ids;
        }
        public IEnumerable<int> SearchComments(string key, int skip, int take)
        {
            var client = GetClient<CommentSearch>(a => a.Id, "comment");
            var response = client.Search<CommentSearch>(a => a
                .AllTypes()
                .From(skip)
                .Size(take)
                .Query(q => q
                     .Match(m => m
                        .Field(f => f.Content)
                        .Query(key)
                     )
                ));
            var Ids = response.Documents.Select(a => a.Id).ToList();
            return Ids;
        }
        public IEnumerable<string> SearchUsers(string key, int skip, int take)
        {
            var client = GetClient<ApplicationUserSearch>(a => a.Id, "user");
            var response = client.Search<ApplicationUserSearch>(a => a
                .AllTypes()
                .From(skip)
                .Size(take)
                .Query(q => q.MultiMatch(c => c.Fields(f => f.Field(p => p.FirstName).Field(p => p.LastName)).Query(key))));
            var Ids = response.Documents.Select(a => a.Id).ToList();
            return Ids;
        }
    }
}
