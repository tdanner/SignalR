// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.md in the project root for license information.

using Microsoft.AspNet.SignalR.Infrastructure;

namespace Microsoft.AspNet.SignalR.Hubs
{
    internal class HubContext : IHubContext
    {
        public HubContext(IDuplexConnection connection, IHubPipelineInvoker invoker, string hubName)
        {
            Connection = connection;
            Clients = new HubConnectionContextBase(connection, invoker, hubName);
            Groups = new GroupManager(connection, PrefixHelper.GetHubGroupName(hubName));
        }

        public IHubConnectionContext Clients { get; private set; }

        public IGroupManager Groups { get; private set; }

        public IDuplexConnection Connection { get; private set; }
    }
}
