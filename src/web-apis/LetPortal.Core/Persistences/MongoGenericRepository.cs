﻿using LetPortal.Core.Exceptions;
using LetPortal.Core.Persistences.Attributes;
using LetPortal.Core.Utils;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace LetPortal.Core.Persistences
{
    public class MongoGenericRepository<T> : IGenericRepository<T> where T : Entity
    {
        protected MongoConnection Connection;

        private EntityCollectionAttribute entityCollectionAttribute = typeof(T).GetEntityCollectionAttribute();

        protected string CollectionName => entityCollectionAttribute.Name;

        protected IMongoCollection<T> Collection => Connection.GetDatabaseConnection().GetCollection<T>(CollectionName);

        public async Task AddAsync(T entity)
        {
            entity.Check();
            if(string.IsNullOrEmpty(entity.Id))
            {
                entity.Id = DataUtil.GenerateUniqueId();
            }
            if(entityCollectionAttribute.IsUniqueBackup)
            {
                await CheckIsExist(entity);
            }
            await Collection.InsertOneAsync(entity);
        }

        public async Task AddBulkAsync(IEnumerable<T> entities)
        {
            foreach(var entity in entities)
            {
                entity.Check();
                if(entityCollectionAttribute.IsUniqueBackup)
                {
                    await CheckIsExist(entity);
                }
            }
            var insertModels = entities.Select(a => new InsertOneModel<T>(a));
            await Collection.BulkWriteAsync(insertModels);
        }

        public async Task DeleteAsync(string id)
        {
            await Collection.DeleteOneAsync(a => a.Id == id);
        }

        public async Task DeleteBulkAsync(IEnumerable<string> ids)
        {
            var deleteModels = ids.Select(a => new DeleteOneModel<T>(Builders<T>.Filter.Eq(b => b.Id, a)));
            await Collection.BulkWriteAsync(deleteModels);
        }

        public IQueryable<T> GetAsQueryable()
        {
            return Collection.AsQueryable();
        }

        public Task<IEnumerable<T>> GetAllByIdsAsync(List<string> ids)
        {
            var filter = Builders<T>.Filter.In(a => a.Id, ids);
            return Task.FromResult(Collection.Find(filter).ToCursor().Current);
        }

        public async Task<T> GetOneAsync(string id)
        {
            return await Collection.AsQueryable().FirstAsync(a => a.Id == id);
        }

        public async Task UpdateAsync(string id, T entity)
        {
            entity.Check();
            if(entityCollectionAttribute.IsUniqueBackup)
            {
                await CheckIsExist(entity);
            }
            await Collection.FindOneAndReplaceAsync(a => a.Id == id, entity);
        }

        public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> expression = null)
        {
            if(expression != null)
            {
                return await Collection.OfType<T>().AsQueryable().Where(expression).ToListAsync();
            }

            return await Collection.OfType<T>().AsQueryable().ToListAsync();
        }

        private async Task CheckIsExist(T entity)
        {
            if(entity is BackupableEntity backupableEntity)
            {
                var backupableCollection = Connection.GetDatabaseConnection().GetCollection<BackupableEntity>(CollectionName);
                var isExist = await backupableCollection.AsQueryable().AnyAsync(a => a.Name == backupableEntity.Name && a.Id != backupableEntity.Id);
                if(isExist)
                {
                    throw new CoreException(ErrorCodes.NameAlreadyExistException);
                }
            }
        }

        public async Task<bool> IsExistAsync(string compareValue, string key = "name")
        {
            var filter = Builders<T>.Filter.Eq(key, compareValue);
            var result = await Collection.Find(filter).AnyAsync();
            return result;
        }
    }
}
