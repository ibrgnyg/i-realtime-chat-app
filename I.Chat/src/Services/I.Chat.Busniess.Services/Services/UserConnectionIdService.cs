using I.Chat.Configure.Models.DTOs;
using I.Chat.Configure.Models.Enums;
using I.Chat.Core.Events;
using I.Chat.Core.MDBRepository;
using I.Chat.Core.MDBRepository.Builder;
using I.Chat.Domain.Models;
using System.Linq.Expressions;

namespace I.Chat.Busniess.Services.Services
{
    public class UserConnectionIdService : IUserConnectionIdService
    {
        private readonly IStateResult _stateResult;
        private readonly IMongoRepository<UserConnectionId> _collectionRepository;
        public UserConnectionIdService(
            IStateResult stateResult,
            IMongoRepository<UserConnectionId> collectionRepository)
        {
            _stateResult = stateResult;
            _collectionRepository = collectionRepository;
        }

        public IQueryable<UserConnectionId> Table => _collectionRepository.Table;

        public bool ExistMessageConnectionId(Expression<Func<UserConnectionId, bool>> filter)
        {
            return _collectionRepository.Exists(filter);
        }

        public virtual UserConnectionId GetMessageConnectionIdFilter(Expression<Func<UserConnectionId, bool>> filter)
        {
            var MessageConnectionId = _collectionRepository.FirstOrDefault(filter).ConfigureAwait(false).GetAwaiter().GetResult();
            return MessageConnectionId;
        }

        public virtual UserConnectionId GetMessageConnectionId(string id)
        {
            if (string.IsNullOrEmpty(id))
                return null;

            var MessageConnectionId = _collectionRepository.GetById(id).ConfigureAwait(false).GetAwaiter().GetResult();
            return MessageConnectionId;
        }

        public virtual int GetCountMessageConnectionId(Expression<Func<UserConnectionId, bool>> filter)
        {
            return (int)_collectionRepository.GetCount(filter).ConfigureAwait(false).GetAwaiter().GetResult();
        }


        public virtual IStateResult SaveMessageConnectionId(DTOUserConnectionId model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.UserId) || string.IsNullOrEmpty(model.ConnectionId) || model == null)
                    return _stateResult.SetErrorEvent("", StateStatus.EmptyParameter);

                var messageConnectionId = new UserConnectionId()
                {
                    ConnectionId = model.ConnectionId,
                    UserId = model.UserId
                };

                var result = _collectionRepository.Save(messageConnectionId).ConfigureAwait(false).GetAwaiter().GetResult();

                if (result == null)
                {
                    return _stateResult.SetErrorEvent("");
                }
                _stateResult.SetSuccessEvent("");
            }
            catch (Exception ex)
            {
                _stateResult.SetExceptionEvent(ex);
            }
            return _stateResult;
        }

        public virtual IStateResult SaveMessageConnectionId(UserConnectionId model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.UserId) || string.IsNullOrEmpty(model.ConnectionId) || model == null)
                    return _stateResult.SetErrorEvent("", StateStatus.EmptyParameter);

                var result = _collectionRepository.Save(model).ConfigureAwait(false).GetAwaiter().GetResult();

                if (result == null)
                {
                    return _stateResult.SetErrorEvent("");
                }
                _stateResult.SetSuccessEvent("");
            }
            catch (Exception ex)
            {
                _stateResult.SetExceptionEvent(ex);
            }
            return _stateResult;
        }


        public virtual IStateResult UpdateMessageConnectionId(DTOUserConnectionId model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.UserId) || string.IsNullOrEmpty(model.ConnectionId) || model == null)
                    return _stateResult.SetErrorEvent("", StateStatus.EmptyParameter);

                var getUpdatedValue = ExistMessageConnectionId(x => x.UserId == model.UserId);

                if (!getUpdatedValue)
                {
                    return SaveMessageConnectionId(model);
                }
                var updateBuilder = UpdateBuilder<UserConnectionId>.Create();
                updateBuilder.Set(x => x.ConnectionId, model.ConnectionId);

                var updateResult = _collectionRepository.UpdateMany(x => x.UserId == model.UserId, updateBuilder).ConfigureAwait(false).GetAwaiter().GetResult();
                if (!updateResult.IsAcknowledged)
                {
                    return _stateResult.SetErrorEvent("");
                }
                _stateResult.SetSuccessEvent("");
            }
            catch (Exception ex)
            {
                _stateResult.SetExceptionEvent(ex);
            }
            return _stateResult;
        }

        public virtual IStateResult UpdateOneFieldMessageConnectionId<U>(string id, Expression<Func<UserConnectionId, U>> expression, U value)
        {
            try
            {
                var updateResult = _collectionRepository.UpdateField(id, expression, value).ConfigureAwait(false).GetAwaiter().GetResult();
                if (!updateResult.IsAcknowledged)
                {
                    return _stateResult.SetErrorEvent("");
                }
                _stateResult.SetSuccessEvent("");
            }
            catch (Exception ex)
            {
                _stateResult.SetExceptionEvent(ex);
            }
            return _stateResult;
        }

        public IStateResult UpdateLastMessageId(string userId, string selectedMessageId)
        {
            try
            {
                var updateBuilder = UpdateBuilder<UserConnectionId>.Create();
                updateBuilder.Set(x => x.SelectedMessageId, selectedMessageId);

                var updateResult = _collectionRepository.UpdateMany(x => x.UserId == userId, updateBuilder).ConfigureAwait(false).GetAwaiter().GetResult();
                if (!updateResult.IsAcknowledged)
                {
                    return _stateResult.SetErrorEvent("");
                }
                _stateResult.SetSuccessEvent("");
            }
            catch (Exception ex)
            {
                _stateResult.SetExceptionEvent(ex);
            }
            return _stateResult;
        }
    }
}
