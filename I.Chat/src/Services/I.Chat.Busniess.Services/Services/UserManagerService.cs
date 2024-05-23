using I.Chat.Busniess.Services.Interfaces;
using I.Chat.Busniess.Services.ServiceHelper;
using I.Chat.Configure.Models.DTOs;
using I.Chat.Configure.Models.Enums;
using I.Chat.Core.Events;
using I.Chat.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace I.Chat.Busniess.Services.Services
{
    public class UserManagerService : IUserManagerService
    {
        private readonly IStateResult _stateResult;
        private readonly IServiceProvider _serviceProvider;
        private readonly IJWTTokenService _jWTTokenService;
        private readonly Lazy<IMessageSendService> _messageSendService;
        private UserManager<User> _userManager;
        private SignInManager<User> _signInManager;

        public UserManagerService(IStateResult stateResult,
            IServiceProvider serviceProvider,
            IJWTTokenService jWTTokenService,
            Lazy<IMessageSendService> messageSendService)
        {
            _stateResult = stateResult;
            _serviceProvider = serviceProvider;
            _jWTTokenService = jWTTokenService;
            _messageSendService = messageSendService;
        }

        public IStateResult LoginUser(DTOUserLogin dtoModel)
        {
            try
            {
                if (dtoModel == null)
                    return _stateResult.SetErrorEvent("empty_parameter", StateStatus.EmptyParameter);

                using (var scope = _serviceProvider.CreateScope())
                {
                    _userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
                    _signInManager = scope.ServiceProvider.GetRequiredService<SignInManager<User>>();

                    var user = _userManager.FindByEmailAsync(dtoModel.Email).ConfigureAwait(false).GetAwaiter().GetResult();

                    if (user == null)
                        return _stateResult.SetErrorEvent("error", StateStatus.NotFound);

                    var result = _signInManager.PasswordSignInAsync(user, dtoModel.Password, false, false).ConfigureAwait(false).GetAwaiter().GetResult();
                    if (!result.Succeeded)
                        return _stateResult.SetErrorEvent("incorrect_email_or_password");

                    var token = _jWTTokenService.CreateToken(user.Id);

                    if (string.IsNullOrEmpty(token))
                        return _stateResult.SetErrorEvent("not_found_token", StateStatus.NotFound);

                    _stateResult.Fields = new Dictionary<string, object>()
                    {
                        {"userId",user.Id},
                        {"token",token}
                    };

                    _stateResult.SetSuccessEvent("");
                }
            }
            catch (Exception ex)
            {
                _stateResult.SetExceptionEvent(ex);
            }
            return _stateResult;
        }

        public IStateResult RegisterUser(DTOUserRegister dtoModel)
        {
            try
            {
                if (dtoModel == null)
                    return _stateResult.SetErrorEvent("empty_parameter", StateStatus.EmptyParameter);

                using (var scope = _serviceProvider.CreateScope())
                {
                    _userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

                    var existUser = _userManager.FindByEmailAsync(dtoModel.Email).ConfigureAwait(false).GetAwaiter().GetResult();
                    if (existUser != null)
                        return _stateResult.SetErrorEvent("dublicate_user", StateStatus.Duplicate);

                    var user = new User
                    {
                        Id = Guid.NewGuid().ToString(),
                        Email = dtoModel.Email,
                        UserName = dtoModel.UserName,
                        EmailConfirmed = false,
                    };

                    var result = _userManager.CreateAsync(user, dtoModel.Password).ConfigureAwait(false).GetAwaiter().GetResult();
                    if (!result.Succeeded)
                    {
                        if (result.Errors.Any(x => x.Code == "DuplicateUserName"))
                        {
                            return _stateResult.SetErrorEvent("duplicate_userName", StateStatus.Duplicate);
                        }
                        return _stateResult.SetErrorEvent("error");
                    }

                    _stateResult.SetSuccessEvent("");
                }
            }
            catch (Exception ex)
            {
                _stateResult.SetExceptionEvent(ex);
            }
            return _stateResult;
        }

        public User GetUser(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    return null;

                using (var scope = _serviceProvider.CreateScope())
                {
                    _userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

                    return _userManager.FindByIdAsync(id).ConfigureAwait(false).GetAwaiter().GetResult();
                }
            }
            catch (Exception ex)
            {
                _stateResult.SetExceptionEvent(ex);
                return null;
            }
        }

        public IStateResult UpdateUserName(DTOUserChangeName dtoModel)
        {
            try
            {
                if (dtoModel == null || string.IsNullOrEmpty(dtoModel.UserName) || string.IsNullOrEmpty(dtoModel.Id))
                    return _stateResult.SetErrorEvent("empty_parameter", StateStatus.EmptyParameter);

                using (var scope = _serviceProvider.CreateScope())
                {
                    _userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

                    var getUser = _userManager.FindByIdAsync(dtoModel.Id).ConfigureAwait(false).GetAwaiter().GetResult();

                    if (getUser == null)
                        return _stateResult.SetErrorEvent("notfound_user", StateStatus.NotFound);

                    var result = _userManager.SetUserNameAsync(getUser, dtoModel.UserName).ConfigureAwait(false).GetAwaiter().GetResult();
                    if (!result.Succeeded)
                    {
                        if (result.Errors.Any(x => x.Code == "DuplicateUserName"))
                        {
                            return _stateResult.SetErrorEvent("duplicate_userName", StateStatus.Duplicate);
                        }
                        return _stateResult.SetErrorEvent("error");
                    }
                }
                _stateResult.SetSuccessEvent("");
            }
            catch (Exception ex)
            {
                _stateResult.SetExceptionEvent(ex);
            }
            return _stateResult;
        }

        public IStateResult UpdatePassword(DTOUserChangePassword dtoModel)
        {
            try
            {
                if (string.IsNullOrEmpty(dtoModel.Id) || dtoModel == null)
                    return _stateResult.SetErrorEvent("", StateStatus.EmptyParameter);

                using (var scope = _serviceProvider.CreateScope())
                {
                    _userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
                    var user = _userManager.FindByIdAsync(dtoModel.Id).ConfigureAwait(false).GetAwaiter().GetResult();

                    if (user == null)
                        return _stateResult.SetErrorEvent("", StateStatus.NotFound);

                    var result = _userManager.ChangePasswordAsync(user, dtoModel.OldPassword, dtoModel.NewPassword).ConfigureAwait(false).GetAwaiter().GetResult();
                    if (!result.Succeeded)
                    {
                        if (result.Errors.Any(x => x.Code == "PasswordMismatch"))
                        {
                            return _stateResult.SetErrorEvent("password_mismatch");
                        }

                        return _stateResult.SetErrorEvent("change_password_error");
                    }
                    _stateResult.SetSuccessEvent("change_password_success");
                }
            }
            catch (Exception ex)
            {
                _stateResult.SetExceptionEvent(ex);
            }
            return _stateResult;
        }

        public List<DTOSearchUser> GetSearchUsers(string username, string userId)
        {
            var dtoSearchUsers = new List<DTOSearchUser>();
            try
            {
                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(userId))
                    return dtoSearchUsers;

                using (var scope = _serviceProvider.CreateScope())
                {
                    _userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

                    var queryable = _userManager.Users.Where(x => !x.Id.Contains(userId) && x.UserName.ToLower().Contains(username.ToLower()));

                    if (queryable == null)
                        return dtoSearchUsers;

                    foreach (var item in queryable)
                    {
                        var existMessage = _messageSendService.Value.DTOMessageCheck(userId, item.Id);

                        dtoSearchUsers.Add(new DTOSearchUser()
                        {
                            Id = existMessage.Id ?? string.Empty,
                            UserId = item.Id,
                            Name = item.UserName,
                            Avatar = item.Avatar,
                        });
                    }
                    return dtoSearchUsers;
                }
            }
            catch (Exception ex)
            {
                _stateResult.SetExceptionEvent(ex);
                return dtoSearchUsers;
            }
        }
    }
}
