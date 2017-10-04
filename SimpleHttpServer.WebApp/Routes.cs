// Copyright (C) 2016 by Barend Erasmus and donated to the public domain

using System.Collections.Generic;
using SimpleHttpServer.Models;
using SimpleHttpServer.RouteHandlers;

namespace SimpleHttpServer.WebApp
{
    internal static class Routes
    {

        public static List<Route> Get => new List<Route>
                {
                    new Route
                    {
                        Callable = request => new FileSystemRouteHandler {BaseUri = "/www"}.Handle(request),
                        UrlRegex = "^\\/*\\/(.*)$",
                        Method = "GET"
                    },
                    new Route
                    {
                        Callable = new FileSystemRouteHandler { BaseUri = @"C:\Users\Barend.Erasmus\Desktop\Test"}.Handle,
                        UrlRegex = "^\\/*\\/(.*)$",
                        Method = "POST"
                    }
                };

        private static HttpResponse HomeIndex(HttpRequest request)
        {
            return new HttpResponse
            {
                ContentAsUtf8 = "Hello",
                ReasonPhrase = "OK",
                StatusCode = "200"
            };

        }
    }
}
