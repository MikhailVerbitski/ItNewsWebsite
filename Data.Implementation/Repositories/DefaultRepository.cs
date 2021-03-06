﻿using Data.Contracts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Data.Implementation.Repositories
{
    public class DefaultRepository<T> : IRepository<T> where T : class
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
            var result = (keySelector != null) ? dbQuery.FirstOrDefault(keySelector) : null;
            return result;
        }
        public virtual T Read(string keySelector, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> dbQuery = GetEntitiesWithIncludes(entities, includes);
            var result = (keySelector != null) ? dbQuery.FirstOrDefault(keySelector) : null;
            return result;
        }
        public virtual IEnumerable<T> ReadMany(
            Expression<Func<T, bool>>[] whereProperties, 
            Expression<Func<T, bool>>[] orderProperties, 
            Expression<Func<T, object>>[] includes
            )
        {
            IQueryable<T> dbQuery = GetEntitiesWithIncludes(entities, includes);
            if (whereProperties != null)
            {
                foreach (var item in whereProperties)
                {
                    dbQuery = dbQuery.Where(item);
                }
            }
            if (orderProperties != null)
            {
                foreach (var item in orderProperties)
                {
                    dbQuery = dbQuery.OrderBy(item);
                }
            }
            return dbQuery;
        }
        public virtual IEnumerable<T> ReadMany(
            Expression<Func<T, bool>>[] whereProperties,
            string orderKey,
            Expression<Func<T, object>>[] includes
            )
        {
            IQueryable<T> dbQuery = GetEntitiesWithIncludes(entities, includes);
            if (whereProperties != null)
            {
                foreach (var item in whereProperties)
                {
                    dbQuery = dbQuery.Where(item);
                }
            }
            dbQuery = (orderKey != null) ? dbQuery.OrderBy(orderKey) : dbQuery;
            return dbQuery;
        }
        public virtual IEnumerable<T> ReadMany(string where, string orderBy, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> dbQuery = GetEntitiesWithIncludes(entities, includes);
            dbQuery = (orderBy != null && orderBy != string.Empty) ? dbQuery.OrderBy(orderBy) : dbQuery;
            dbQuery = (where != null && where != string.Empty) ? dbQuery.Where(where) : dbQuery;
            return dbQuery;
        }
        public virtual IEnumerable<T> ReadMany()
        {
            return ReadMany(null, null);
        }
        public virtual T Update(T entity, params Expression<Func<T, object>>[] properties)
        {
            var property = typeof(T).GetProperties().SingleOrDefault(a => a.Name == "Id");
            if (property == null)
            {
                property = typeof(T).GetProperties().Where(a => a.Name.Contains("id") || a.Name.Contains("Id")).First();
            }
            var id = property.GetValue(entity);

            if (properties.Length == 0)
            {
                var localEntity = entities.Local.SingleOrDefault(a => property.GetValue(a).Equals(id));
                if (localEntity != null)
                {
                    var localEntityEntry = context.Entry(localEntity);
                    localEntityEntry.State = EntityState.Detached;
                    context.SaveChanges();
                }

                entities.Attach(entity);
                var entry = context.Entry(entity);
                entry.State = EntityState.Modified;
            }
            else
            {
                var lastEntity = entities.Find(id);
                foreach (var item in properties)
                {
                    var body = ((item.Body is UnaryExpression)
                        ? (item.Body as UnaryExpression).Operand
                        : item.Body)
                        as MemberExpression;
                    if (body == null)
                    {
                        throw new ArgumentException("The body must be a member expression");
                    }
                    var propertyInfo = typeof(T).GetProperty(body.Member.Name);
                    propertyInfo.SetValue(lastEntity, propertyInfo.GetValue(entity));
                }
            }
            context.SaveChanges();
            return entity;
        }
        public virtual async Task Delete(T entity)
        {
            entities.Attach(entity);
            entities.Remove(entity);
            await context.SaveChangesAsync();
        }
        private IQueryable<T> GetEntitiesWithIncludes(DbSet<T> entities, Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> dbQuery = entities;
            foreach (var item in includes)
            {
                MemberExpression body = item.Body as MemberExpression;
                if (body == null)
                {
                    throw new ArgumentException("The body must be a member expression");
                }
                dbQuery = dbQuery.Include(body.Member.Name);
            }
            return dbQuery;
        }
    }
}
