﻿// Copyright 2007-2014 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Transports.RabbitMq
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Net.Mime;
    using System.Text;
    using RabbitMQ.Client;


    public class RabbitMQReceiveContext :
        ReceiveContext,
        BasicConsumeContext
    {
        static readonly ContentType DefaultContentType = new ContentType("application/vnd.masstransit+json");

        readonly byte[] _body;
        readonly string _consumerTag;
        readonly ulong _deliveryTag;
        readonly string _exchange;
        readonly Uri _inputAddress;
        readonly IBasicProperties _properties;
        readonly Stopwatch _receiveTimer;
        readonly bool _redelivered;
        readonly string _routingKey;
        ContentType _contentType;
        Encoding _encoding;
        RabbitMQReceiveContextHeaders _headers;

        public RabbitMQReceiveContext(string exchange, string routingKey, string consumerTag, Uri inputAddress, ulong deliveryTag,
            byte[] body, bool redelivered, IBasicProperties properties)
        {
            _receiveTimer = Stopwatch.StartNew();

            _exchange = exchange;
            _routingKey = routingKey;
            _body = body;
            _redelivered = redelivered;
            _deliveryTag = deliveryTag;
            _properties = properties;
            _inputAddress = inputAddress;
            _consumerTag = consumerTag;
        }

        public string ConsumerTag
        {
            get { return _consumerTag; }
        }

        public ulong DeliveryTag
        {
            get { return _deliveryTag; }
        }

        public string Exchange
        {
            get { return _exchange; }
        }

        public string RoutingKey
        {
            get { return _routingKey; }
        }

        public IBasicProperties Properties
        {
            get { return _properties; }
        }

        public Encoding ContentEncoding
        {
            get
            {
                return _encoding ?? (_encoding = string.IsNullOrWhiteSpace(ContentType.CharSet)
                    ? Encoding.UTF8
                    : Encoding.GetEncoding(ContentType.CharSet));
            }
        }

        public Stream Body
        {
            get { return new MemoryStream(_body, 0, _body.Length, false); }
        }

        public TimeSpan ElapsedTime
        {
            get { return _receiveTimer.Elapsed; }
        }

        public Uri InputAddress
        {
            get { return _inputAddress; }
        }

        public ContentType ContentType
        {
            get { return _contentType ?? (_contentType = GetContentType()); }
        }

        public bool Redelivered
        {
            get { return _redelivered; }
        }

        public Headers Headers
        {
            get { return _headers ?? (_headers = new RabbitMQReceiveContextHeaders(this)); }
        }

        ContentType GetContentType()
        {
            object contentTypeHeader;
            if (Headers.TryGetHeader("Content-Type", out contentTypeHeader))
            {
                var contentType = contentTypeHeader as ContentType;
                if (contentType != null)
                    return contentType;
                var s = contentTypeHeader as string;
                if (s != null)
                    return new ContentType(s);
            }

            return DefaultContentType;
        }
    }
}