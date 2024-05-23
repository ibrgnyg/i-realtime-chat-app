using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace I.Chat.Core.MDBRepository.Builder
{
    public class UpdateBuilder<TEntity>
    {
        private readonly List<UpdateDefinition<TEntity>> _list = new();

        protected UpdateBuilder() { }

        public static UpdateBuilder<TEntity> Create()
        {
            return new();
        }

        public UpdateBuilder<TEntity> Set<TProperty>(Expression<Func<TEntity, TProperty>> selector, TProperty value)
        {
            _list.Add(Builders<TEntity>.Update.Set(selector, value));
            return this;
        }

        public UpdateBuilder<TEntity> Set<TProperty>(FieldDefinition<TEntity, TProperty> field, TProperty value)
        {
            _list.Add(Builders<TEntity>.Update.Set(field, value));
            return this;
        }

        public UpdateBuilder<TEntity> PullFilter<TProperty>(FieldDefinition<TEntity> field, FilterDefinition<TProperty> value)
        {
            _list.Add(Builders<TEntity>.Update.PullFilter(field, value));
            return this;
        }

        public UpdateBuilder<TEntity> Push<TProperty>(Expression<Func<TEntity, IEnumerable<TProperty>>> field, TProperty value)
        {
            _list.Add(Builders<TEntity>.Update.Push(field, value));
            return this;
        }

        public IEnumerable<UpdateDefinition<TEntity>> Fields
        {
            get { return _list; }
        }
    }
}
