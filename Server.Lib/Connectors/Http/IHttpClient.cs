﻿using System;
using Server.Lib.Models.Resources.Api;

namespace Server.Lib.Connectors.Http
{
    public interface IHttpClient
    {
        IHttpRequest Head(Uri target);
        IHttpRequest Get(Uri target);
        IHttpRequest<T> Get<T>(Uri target) where T : ApiResource;
    }
}