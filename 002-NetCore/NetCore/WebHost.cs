﻿using System.Threading.Tasks;

namespace NetCore
{
    public interface IWebHost
    {
        Task StartAsync();
    }

    public class WebHost : IWebHost
    {
        private readonly IServer _server;
        private readonly RequestDelegate _handler;
        public WebHost(IServer server, RequestDelegate handler)
        {
            _server = server;
            _handler = handler;
        }
        public Task StartAsync() => _server.StartAsync(_handler);
    }
}
