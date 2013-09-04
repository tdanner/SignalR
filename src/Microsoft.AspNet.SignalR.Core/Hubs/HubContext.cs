// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.md in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.AspNet.SignalR.Infrastructure;

namespace Microsoft.AspNet.SignalR.Hubs
{
    internal class HubContext : IHubContext
    {
        public HubContext(IDuplexConnection connection, IHubPipelineInvoker invoker, string hubName)
        {
            ConnectionId = connection.ConnectionId;
            Clients = new HubConnectionContextBase(connection, invoker, hubName);
            Groups = new GroupManager(connection, PrefixHelper.GetHubGroupName(hubName));
        }

        public IHubConnectionContext Clients { get; private set; }

        public IGroupManager Groups { get; private set; }

        public string ConnectionId { get; private set; }
    }
}
