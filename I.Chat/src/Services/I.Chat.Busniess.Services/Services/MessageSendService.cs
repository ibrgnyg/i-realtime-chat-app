using I.Chat.Busniess.Services.Interfaces;
using I.Chat.Configure.Models.Base;
using I.Chat.Configure.Models.DTOs;
using I.Chat.Configure.Models.Enums;
using I.Chat.Core.Events;
using I.Chat.Core.MDBRepository;
using I.Chat.Core.MDBRepository.Builder;
using I.Chat.Domain.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace I.Chat.Busniess.Services.Services
{
    public class MessageSendService : IMessageSendService
    {
        private readonly IStateResult _stateResult;
        private readonly IMongoRepository<Message> _collectionRepository;
        private readonly IUserManagerService _userManagerService;
        public MessageSendService(
            IStateResult stateResult,
            IMongoRepository<Message> collectionRepository,
            IUserManagerService userManagerService)
        {
            _stateResult = stateResult;
            _collectionRepository = collectionRepository;
            _userManagerService = userManagerService;
        }

        public IQueryable<Message> Table => _collectionRepository.Table;

        public DTOMessageCheck DTOMessageCheck(string startUserId, string toUserId)
        {
            var getMessage = GetMessageFilter(x => x.StartUserId == startUserId && x.ToUserId == toUserId
                || x.StartUserId == toUserId && x.ToUserId == startUserId);
            if (getMessage == null)
            {
                return new DTOMessageCheck();
            }
            var dTOMessageCheck = new DTOMessageCheck();
            dTOMessageCheck.AnyMessage = false;
            dTOMessageCheck.Id = getMessage.Id;
            return dTOMessageCheck;
        }


        public bool ExistMessage(Expression<Func<Message, bool>> filter)
        {
            return _collectionRepository.Exists(filter);
        }

        public virtual Message GetMessageFilter(Expression<Func<Message, bool>> filter)
        {
            var Message = _collectionRepository.FirstOrDefault(filter).ConfigureAwait(false).GetAwaiter().GetResult();
            return Message;
        }


        public virtual Message GetMessage(string id)
        {
            if (string.IsNullOrEmpty(id))
                return null;

            var Message = _collectionRepository.GetById(id).ConfigureAwait(false).GetAwaiter().GetResult();
            return Message;
        }

        public virtual long GetCountMessage(Expression<Func<Message, bool>> filter)
        {
            return (int)_collectionRepository.GetCount(filter).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public virtual IStateResult RemoveMessage(string id)
        {
            try
            {
                var model = GetMessage(id);
                if (model == null)
                    return _stateResult.SetErrorEvent("", StateStatus.EmptyParameter);

                /*var updateResult = _collectionRepository.re(id).ConfigureAwait(false).GetAwaiter().GetResult();
                if (!updateResult.IsAcknowledged)
                {
                    return _stateResult.SetErrorEvent("");
                }
                _stateResult.SetSuccessEvent("");*/
            }
            catch (Exception ex)
            {
                _stateResult.SetExceptionEvent(ex);
            }
            return _stateResult;
        }

        public virtual IStateResult SaveMessage(DTOMessage model)
        {
            try
            {
                if (model == null)
                    return _stateResult.SetErrorEvent("", StateStatus.EmptyParameter);

                var Message = new Message()
                {
                    StartUserId = model.StartUserId,
                    ToUserId = model.ToUserId,
                    MessageContents = new List<MessageContent>()
                    {
                        new()
                        {
                            UserId = model.StartUserId,
                            Message = model.MessageContent,
                            CreateDate= DateTime.UtcNow,
                        }
                    }
                };

                var result = _collectionRepository.Save(Message).ConfigureAwait(false).GetAwaiter().GetResult();

                if (result == null)
                {
                    return _stateResult.SetErrorEvent("");
                }

                _stateResult.Fields = new Dictionary<string, object>()
                {
                    {"Id",Message.Id},
                    {"Model",model}
                };

                _stateResult.SetSuccessEvent("");
            }
            catch (Exception ex)
            {
                _stateResult.SetExceptionEvent(ex);
            }
            return _stateResult;
        }

        public virtual IStateResult SaveMessage(Message model)
        {
            try
            {
                if (model == null)
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

        public virtual IStateResult UpdateMessage(DTOMessage model)
        {
            try
            {
                if (model == null)
                    return _stateResult.SetErrorEvent("", StateStatus.EmptyParameter);

                if (!ExistMessage(x => x.StartUserId == model.StartUserId && x.ToUserId == model.ToUserId
                || x.StartUserId == model.ToUserId && x.ToUserId == model.StartUserId))
                    return SaveMessage(model);

                var updateBuilder = UpdateBuilder<Message>.Create();
                updateBuilder.Push(x => x.MessageContents, new MessageContent()
                {
                    Message = model.MessageContent,
                    UserId = model.StartUserId,
                    IsRead = model.IsRead,
                    CreateDate = DateTime.UtcNow,
                });

                var updateResult = _collectionRepository.UpdateMany(x => x.Id == model.Id, updateBuilder).ConfigureAwait(false).GetAwaiter().GetResult();
                if (!updateResult.IsAcknowledged)
                {
                    return _stateResult.SetErrorEvent("error", resetFields: true);
                }

                _stateResult.SetSuccessEvent("", resetFields: true);

                if (!_stateResult.IsError && _stateResult.Fields == null || !_stateResult.Fields.Any())
                {
                    _stateResult.Fields = new Dictionary<string, object>()
                    {
                        {"Model",model}
                    };
                }
            }
            catch (Exception ex)
            {
                _stateResult.SetExceptionEvent(ex);
            }
            return _stateResult;
        }

        public virtual IStateResult UpdateOneFieldMessage<U>(string id, Expression<Func<Message, U>> expression, U value)
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

        public DTOMessagePagination GetDTOMessagePagination(string id, string userId, int activePage, int pageSize)
        {
            if (string.IsNullOrEmpty(id))
                return new DTOMessagePagination();

            var message = _collectionRepository.FirstOrDefault(x => x.Id == id).ConfigureAwait(false).GetAwaiter().GetResult();

            if (message == null)
                return new DTOMessagePagination();

            var pagedMessages = message.MessageContents
                .OrderByDescending(x => x.CreateDate)
                .Select(item => new DTOMessageDetailContent
                {
                    Id = item.Id,
                    CreateDate = item.CreateDate,
                    UserId = item.UserId,
                    IsRead = item.IsRead,
                    Message = item.Message,
                    BlockUserId = message.BlockUserId,
                    IsBloked = message.IsBloked,
                });

            var totalMessageCounts = pagedMessages.Count();
            var totalPages = (int)Math.Ceiling((double)totalMessageCounts / pageSize);

            pagedMessages = pagedMessages
                .Skip((activePage - 1) * pageSize)
                .Take(pageSize)
                .OrderBy(x => x.CreateDate)
                .ToList();

            return new DTOMessagePagination
            {
                Id = message.Id,
                StartUserId = message.StartUserId,
                ToUserId = message.ToUserId,
                Messages = (List<DTOMessageDetailContent>)pagedMessages,
                TotalCount = totalMessageCounts,
                TotalPageCount = totalPages,
            };
        }

        public async Task<List<DTOMessages>> GetMessages(string userId)
        {
            var DTOMessages = new List<DTOMessages>();

            var queryable = Table.Where(x => x.StartUserId == userId || x.ToUserId == userId)

            .Select(x => new Message()
            {
                Id = x.Id,
                StartUserId = x.StartUserId,
                ToUserId = x.ToUserId,
                BlockUserId = x.BlockUserId,
                IsBloked = x.IsBloked,
                MessageContents = x.MessageContents,
            });

            var messages = queryable.ToList();

            foreach (var item in messages)
            {
                var getUserId = item.StartUserId != userId ? item.StartUserId : item.ToUserId;

                var user = _userManagerService.GetUser(getUserId);

                if (user == null)
                    continue;

                var lastMessage = item.MessageContents.Where(x => x.UserId != userId)
                    .OrderByDescending(x => x.CreateDate)
                    .FirstOrDefault()?.Message ?? item.MessageContents.Where(x => x.UserId == userId)
                    .OrderByDescending(x => x.CreateDate)
                    .FirstOrDefault()?.Message;

                DTOMessages.Add(new DTOMessages()
                {
                    Id = item.Id,
                    Name = user.UserName,
                    Avatar = user.Avatar,
                    UserId = user.Id,
                    LastMessage = lastMessage ?? string.Empty,
                    UnreadMessageCount = item.MessageContents
                    .Where(x => !x.IsRead && x.UserId != userId)
                    .Count()

                });
            }

            return DTOMessages;
        }

        public IStateResult BlockMessage(DTOBlockMessage model)
        {
            try
            {
                if (model == null)
                    return _stateResult.SetErrorEvent("", StateStatus.EmptyParameter);

                var updateBuilder = UpdateBuilder<Message>.Create();

                var blockedUserId = model.Type ? model.StartUserId : string.Empty;

                updateBuilder.Set(x => x.BlockUserId, blockedUserId);
                updateBuilder.Set(x => x.IsBloked, model.Type);

                var updateResult = _collectionRepository.UpdateMany(x => x.Id == model.Id, updateBuilder).ConfigureAwait(false).GetAwaiter().GetResult();
                if (!updateResult.IsAcknowledged)
                {
                    return _stateResult.SetErrorEvent("error", resetFields: true);
                }

                _stateResult.SetSuccessEvent("", resetFields: true);

                if (!_stateResult.IsError && _stateResult.Fields == null || !_stateResult.Fields.Any())
                {
                    _stateResult.Fields = new Dictionary<string, object>()
                    {
                        {"IsBlocked",model.Type}
                    };
                }
            }
            catch (Exception ex)
            {
                _stateResult.SetExceptionEvent(ex);
            }
            return _stateResult;
        }

        public Task UpdateIsRead(string id, string userId)
        {
            try
            {
                if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(userId))
                    return Task.CompletedTask;

                var getMessage = GetMessage(id);

                if (getMessage == null)
                    return Task.CompletedTask;

                var unreadMessages = getMessage.MessageContents
                                              .Where(x => x.UserId != userId && !x.IsRead)
                                              .ToList();

                if (unreadMessages.Any())
                {
                    foreach (var message in unreadMessages)
                    {
                        message.IsRead = true;
                    }

                    _collectionRepository.Update(getMessage);
                }
            }
            catch (Exception ex)
            {
                _stateResult.SetExceptionEvent(ex);
            }
            return Task.CompletedTask;
        }


        public Task<int> GetUnreadMessagesCount(string userId)
        {
            try
            {
                var messageCount = Table
                .Where(x => x.StartUserId == userId || x.ToUserId == userId)
                .SelectMany(x => x.MessageContents)
                .Where(x => !x.IsRead && x.UserId != userId)
                .GroupBy(x => x.UserId)
                .Select(g => new { UserId = g.Key, MessageCount = g.Count() }).Count();

                return Task.FromResult(messageCount);
            }
            catch (Exception ex)
            {
                _stateResult.SetExceptionEvent(ex);
                return Task.FromResult(0);
            }
        }
    }
}
