using I.Chat.Configure.Models.Enums;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace I.Chat.Core.Events
{
    public class StateResult : IStateResult
    {
        private readonly ILogger<StateResult> _logger;
        //private readonly IStringLocalizer _localizer;

        public StateResult() { }

        public StateResult(ILogger<StateResult> logger)
        {
            _logger = logger;
        }

        public string Message { get; set; } = string.Empty;

        private StateStatus _stateStatus;

        public StateStatus StateStatus
        {
            get => _stateStatus;
            set => _stateStatus = value;
        }

        private Dictionary<string, object> _fields = new Dictionary<string, object>();

        public IDictionary<string, object> Fields
        {
            get => _fields ?? new Dictionary<string, object>();
            set => _fields = (Dictionary<string, object>)value;
        }

        public bool IsError { get; set; } = false;

        public void SetExceptionEvent(Exception ex, string localizerMessage = "unknown_error")
        {
            IsError = true;
            Message = localizerMessage; //_localizer[localizerMessage];
            StateStatus = StateStatus.Error;
            //_logger.LogError(ex.ToString());
        }

        public IStateResult SetErrorEvent(string localizerMessage, StateStatus stateStatus = StateStatus.Error, string logMessage = "", bool resetFields = true)
        {
            if (resetFields)
            {
                Fields = new Dictionary<string, object>();
            }
            IsError = true;
            Message = localizerMessage; //_localizer[localizerMessage];
            StateStatus = stateStatus;
            //_logger.LogError(logMessage ?? string.Empty);
            return this;
        }

        public IStateResult SetSuccessEvent(string localizerMessage, StateStatus stateStatus = StateStatus.Success, bool resetFields = false)
        {
            if(resetFields)
            {
                Fields = new Dictionary<string, object>();
            }
            IsError = false;
            Message = localizerMessage;//_localizer[localizerMessage];
            StateStatus = stateStatus;
            return this;
        }
    }
}
