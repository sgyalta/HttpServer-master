using SimpleHttpServer.Models;
using System.IO;

namespace SimpleHttpServer
{
    internal static class HttpBuilder
    {
        public static HttpResponse InternalServerError()
        {
            var content = File.ReadAllText("Resources/Pages/500.html"); 

            return new HttpResponse()
            {
                ReasonPhrase = "InternalServerError",
                StatusCode = "500",
                ContentAsUtf8 = content
            };
        }

        public static HttpResponse NotFound()
        {
            var content = File.ReadAllText("Resources/Pages/404.html");

            return new HttpResponse()
            {
                ReasonPhrase = "NotFound",
                StatusCode = "404",
                ContentAsUtf8 = content
            };
        }
    }
}
