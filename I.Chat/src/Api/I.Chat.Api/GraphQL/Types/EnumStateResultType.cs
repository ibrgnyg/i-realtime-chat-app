using HotChocolate.Types;
using I.Chat.Configure.Models.Enums;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace I.Chat.Api.GraphQL.Types
{
    public class EnumStateResultType : EnumType<StateStatus>
    {
        protected override void Configure(IEnumTypeDescriptor<StateStatus> descriptor)
        {
            var enumValues = Enum.GetValues(typeof(StateStatus)).Cast<StateStatus>().ToList();

            foreach (var item in enumValues)
            {
                try
                {
                    descriptor
                        .Value(item)
                        .Name(item.ToString().ToLower());
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    continue;
                }
            }
        }
    }
}
