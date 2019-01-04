﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Data.Implementation.Repositories
{
    public class Repository<T> where T : class
    {
        protected ApplicationDbContext context;
        protected DbSet<T> entities;

        public Repository(ApplicationDbContext context)
        {
            this.GetContext(context);
            entities = context
                .GetType()
                .GetProperties()
                .Where(a => a.PropertyType == typeof(DbSet<T>))
                .Single()
                .GetValue(context) as DbSet<T>;
        }

        public void GetContext(ApplicationDbContext context)
        {
            this.context = context;
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
        public virtual void Update(T entity, params Expression<Func<T, object>>[] properties)
        {
            List<string> includelist = new List<string>();
            foreach (var item in properties)
            {
                MemberExpression body = item.Body as MemberExpression;
                if (body == null)
                    throw new ArgumentException("The body must be a member expression");
                includelist.Add(body.Member.Name);
            }
            entities.Attach(entity);
            var entry = context.Entry(entity);
            foreach (var item in includelist)
            {
                entry.Property(item).IsModified = true;
            }
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
