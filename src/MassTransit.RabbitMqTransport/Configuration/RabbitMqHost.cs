﻿// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace MassTransit.RabbitMqTransport.Configuration
{
    using Policies;
    using Transports;


    public class RabbitMqHost :
        IRabbitMqHost
    {
        readonly RabbitMqConnectionCache _connectionCache;
        readonly RabbitMqHostSettings _hostSettings;
        readonly IMessageNameFormatter _messageNameFormatter;
        readonly RabbitMqConnectionCache _sendConnectionCache;

        public RabbitMqHost(RabbitMqHostSettings hostSettings)
        {
            _hostSettings = hostSettings;

            var connector = new RabbitMqConnector(hostSettings, Retry.None);

            _connectionCache = new RabbitMqConnectionCache(connector);
            _sendConnectionCache = new RabbitMqConnectionCache(connector);
            _messageNameFormatter = new RabbitMqMessageNameFormatter();
        }

        public IMessageNameFormatter MessageNameFormatter
        {
            get { return _messageNameFormatter; }
        }

        public IConnectionCache SendConnectionCache
        {
            get { return _sendConnectionCache; }
        }

        public IConnectionCache ConnectionCache
        {
            get { return _connectionCache; }
        }

        public RabbitMqHostSettings Settings
        {
            get { return _hostSettings; }
        }
    }
}