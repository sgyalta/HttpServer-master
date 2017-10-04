// Copyright (C) 2016 by Barend Erasmus, David Jeske and donated to the public domain

using System.Collections.Generic;
using System.Threading;
using SimpleHttpServer.Models;

namespace SimpleHttpServer.App
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();

            var routeConfig = new List<Route>() {
                new Route {
                    Name = "Hello Handler",
                    UrlRegex = @"^/$",
                    Method = "GET",
                    Callable = (HttpRequest request) => new HttpResponse()
                    {
                        ContentAsUtf8 = "Hello from SimpleHttpServer",
                        ReasonPhrase = "OK",
                        StatusCode = "200"
                    }
                }, 
                //new Route {   
                //    Name = "FileSystem Static Handler",
                //    UrlRegex = @"^/Static/(.*)$",
                //    Method = "GET",
                //    Callable = new FileSystemRouteHandler() { BasePath = @"C:\Tmp", ShowDirectories=true }.Handle,
                //},
            };

            var httpServer = new HttpServer(8080, routeConfig);
            
            var thread = new Thread(httpServer.Listen);
            thread.Start();
        }
    }
}
