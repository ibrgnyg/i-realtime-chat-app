using HotChocolate.Types;
using I.Chat.Configure.Models.Enums;
using I.Chat.Core.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace I.Chat.Api.GraphQL.Types
{
    public class StateResultType : ObjectType<StateResult>
    {
        protected override void Configure(IObjectTypeDescriptor<StateResult> descriptor)
        {
            descriptor.Ignore(t => t.SetSuccessEvent(string.Empty, StateStatus.Success,false));
            descriptor.Ignore(t => t.SetErrorEvent(string.Empty, StateStatus.Error, string.Empty,true));
            descriptor.Field(t => t.Fields).Type<AnyType>();
        }
    }
}
