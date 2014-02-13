using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.AspNet.SignalR.Messaging
{
    interface IBinaryMessages
    {
        IEnumerable<Message> BinaryMessages { get; } 
    }
}
