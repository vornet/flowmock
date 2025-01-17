﻿using FlowMock.Engine.Models;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace FlowMock.Engine
{
    public class MockContext
    {
        public Request Request { get; internal set; }
        public HttpContext HttpContext { get; internal set; }
        public Dictionary<string, string> MockState { get; internal set; }
        public HttpRequestMapper RequestMapper { get; internal set; }
    }
}
