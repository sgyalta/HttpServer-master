// Copyright (C) 2016 by Barend Erasmus and donated to the public domain

using System.Collections.Generic;
using Server.Models;
using Server.RouteHandlers;

namespace Server.WebApp
{
    internal static class Routes
    {

        public static List<Route> Get => new List<Route>
                {
                    new Route
                    {
                        Callable = request => new FileSystemRouteHandler {GetCnt =+1}.HandleGetResponse(request),
                        UrlRegex = "^\\/*\\/(.*)$",
                        Method = "GET"
                    },
                    new Route
                    {
                        Callable = new FileSystemRouteHandler { PostCnt =+1}.HandlePostResponse,
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
