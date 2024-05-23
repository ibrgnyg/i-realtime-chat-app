using I.Chat.Configure.Models;
using I.Chat.Configure.Models.Base;
using I.Chat.Core.MDBRepository.Builder;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Linq.Expressions;

namespace I.Chat.Core.MDBRepository
{
    public partial class MongoRepository<TEntity> : IMongoRepository<TEntity> where TEntity : BaseEntity
    {
        protected IMongoCollection<TEntity>? _collection;

        public IMongoCollection<TEntity>? Collection { get { return _collection; } }

        protected IMongoDatabase? _database;

        public IMongoDatabase? Database { get { return _database; } }

        private readonly IOptions<DBSettings> _settings;

        public MongoRepository(IOptions<DBSettings> settings)
        {
            _settings = settings;
            var dbName = _settings.Value.DatabaseName;

            if (!string.IsNullOrEmpty(dbName))
            {
                var client = new MongoClient(_settings.Value.ConnectionString);
                _database = client.GetDatabase(_settings.Value.DatabaseName);
                _collection = _database.GetCollection<TEntity>(typeof(TEntity).Name);
            }
        }

        public IQueryable<TEntity> Table => _collection.AsQueryable();

        public bool Exists(Expression<Func<TEntity, bool>> filter)
        {
            return GetCount(filter).GetAwaiter().GetResult() > 0;
        }

        public Task<TEntity> FirstOrDefault(Expression<Func<TEntity, bool>> filter)
        {
            return _collection.Find(filter).FirstOrDefaultAsync();
        }

        public Task<List<TEntity>> GetAll()
        {
            return _collection.AsQueryable().ToListAsync();
        }

        public virtual Task<TEntity> GetById(string id)
        {
            return _collection.Find(e => e.Id == id).FirstOrDefaultAsync();
        }

        public virtual Task<long> GetCount(Expression<Func<TEntity, bool>> filter)
        {
            return _collection.Find(filter).CountDocumentsAsync();
        }

        public virtual async Task<TEntity> Save(TEntity entity)
        {
            await _collection.InsertOneAsync(entity);
            return entity;
        }

        public virtual async Task<ReplaceOneResult> Update(TEntity entity)
        {
            entity.UpdateDate = DateTime.UtcNow;
            return await _collection.ReplaceOneAsync(x => x.Id == entity.Id, entity, new ReplaceOptions { IsUpsert = true });
        }

        public virtual async Task<UpdateResult> UpdateField<U>(string id, Expression<Func<TEntity, U>> expression, U value)
        {
            var builder = Builders<TEntity>.Filter;
            var filter = builder.Eq(x => x.Id, id);
            var update = Builders<TEntity>.Update
                .Set(expression, value)
                .Set(x => x.UpdateDate, DateTime.UtcNow);

            return await _collection.UpdateOneAsync(filter, update);
        }

        public virtual async Task<UpdateResult> UpdateMany(Expression<Func<TEntity, bool>> filterexpression, UpdateBuilder<TEntity> updateBuilder)
        {
            updateBuilder
               .Set(x => x.UpdateDate, DateTime.UtcNow);

            var update = Builders<TEntity>.Update.Combine(updateBuilder.Fields);
            return await _collection.UpdateOneAsync(filterexpression, update);
        }
    }
}
