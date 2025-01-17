﻿using FlowMock.Engine.Data;
using FlowMock.Engine.Models;
using LazyCache;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Threading.Tasks;

namespace FlowMock.Engine
{
    public class FlowMockMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IHttpProxier _httpProxier;
        private readonly IHttpMocker _httpMocker;
        private readonly IDataAccess _dataAccess;
        private readonly IAppCache _appCache;

        public FlowMockMiddleware(RequestDelegate next, IHttpProxier httpProxier, IHttpMocker httpMocker, IDataAccess dataAccess, IAppCache appCache)
        {
            _next = next;
            _httpProxier = httpProxier;
            _httpMocker = httpMocker;
            _dataAccess = dataAccess;
            _appCache = appCache;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var settings = await _appCache.GetOrAddAsync("settings", async () =>
            {
                return (await _dataAccess.GetAllSettingsAsync()).ToList();
            });

            var proxyBasePath = settings.FirstOrDefault(setting => setting.Key == "Proxy Base Path").Value;

            if (!context.Request.Path.StartsWithSegments(proxyBasePath))
            {
                await _next(context);
                return;
            }

            var request = new Request();

            var requestStopwatch = System.Diagnostics.Stopwatch.StartNew();
            (var mock, var mockContext) = await _httpMocker.ShouldHandleAsync(context);
            if (mock != null)
            {
                mockContext.Request = request;
                await _httpMocker.HandleAsync(mock, mockContext);
            }
            else
            {
                var proxyContext = new ProxyContext { Request = request, HttpContext = context };
                await _httpProxier.HandleAsync(proxyContext);
            }
            requestStopwatch.Stop();
            request.ResponseTime = requestStopwatch.ElapsedMilliseconds;
            await _dataAccess.AddRequestAsync(request);
        }
    }
}
