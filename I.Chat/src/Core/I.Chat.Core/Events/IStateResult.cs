using I.Chat.Configure.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace I.Chat.Core.Events
{
    public interface IStateResult
    {
        string Message { get; set; }
        bool IsError { get; set; }
        StateStatus StateStatus { get; set; }
        IDictionary<string, object> Fields { get; set; }
        void SetExceptionEvent(Exception ex, string localizerMessage = "unknown_error");
        IStateResult SetErrorEvent(string localizerMessage = "error", StateStatus eventStatus = StateStatus.Error, string logMessage = "", bool resetFields = true);
        IStateResult SetSuccessEvent(string localizerMessage = "success", StateStatus eventStatus = StateStatus.Success, bool resetFields = false);
    }
}
