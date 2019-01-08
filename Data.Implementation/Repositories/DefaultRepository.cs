﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Data.Implementation.Repositories
{
    public class DefaultRepository<T> where T : class
    {
        protected ApplicationDbContext context;
        protected DbSet<T> entities;

        public DefaultRepository(ApplicationDbContext context)
        {
            this.context = context;
            entities = context.Set<T>();
        }

        public virtual T Create(T entity)
        {
            entities.Add(entity);
            context.SaveChanges();
            return entity;
        }
        public virtual T Read(Expression<Func<T, bool>> keySelector, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> dbQuery = GetEntitiesWithIncludes(entities, includes);
            var result = (keySelector != null) ? dbQuery.Where(keySelector).SingleOrDefault() : entities.SingleOrDefault();
            return result;
        }
        public virtual IEnumerable<T> ReadMany(Expression<Func<T, bool>> keySelector, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> dbQuery = GetEntitiesWithIncludes(entities, includes);
            return (keySelector != null) ? dbQuery.Where(keySelector) : dbQuery;
        }
        public virtual void Update(T entity)
        {
            var property = typeof(T).GetProperties().SingleOrDefault(a => a.Name == "Id");
            if (property == null)
            {
                property = typeof(T).GetProperties().Where(a => a.Name.Contains("id") || a.Name.Contains("Id")).First();
            }
            var id = property.GetValue(entity);

            var oldEntity = entities.Find(id);

            var entry = context.Entry(oldEntity);
            entry.State = EntityState.Detached;
            entry.CurrentValues.SetValues(entity);

            context.SaveChanges();
        }
        public virtual void Delete(T entity)
        {
            entities.Attach(entity);
            entities.Remove(entity);
            context.SaveChanges();
        }

        private IQueryable<T> GetEntitiesWithIncludes(DbSet<T> entities, Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> dbQuery = entities;
            foreach (var item in includes)
            {
                MemberExpression body = item.Body as MemberExpression;
                if (body == null)
                    throw new ArgumentException("The body must be a member expression");
                dbQuery = dbQuery.Include(body.Member.Name);
            }
            return dbQuery;
        }
    }
}
