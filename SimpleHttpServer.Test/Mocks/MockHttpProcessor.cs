using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using SimpleHttpServer.Models;

namespace SimpleHttpServer.Test.Mocks
{
    class MockHttpProcessor : HttpProcessor
    {
        public HttpResponse Response { get; set; }
        public HttpRequest Request { get; set; }

        public MockHttpProcessor(HttpRequest request, List<Route> routes)
        {
            Request = request;

            foreach (var route in routes)
            {
                AddRoute(route);
            }
        }

        public MockHttpProcessor(HttpRequest request, Route route)
        {
            Request = request;

            AddRoute(route);

        }

        protected override Stream GetInputStream(TcpClient tcpClient)
        {
            return GenerateStreamFromString(Request.ToString());
        }

        protected override Stream GetOutputStream(TcpClient tcpClient)
        {
            return new MemoryStream();
        }

        protected override HttpResponse RouteRequest(Stream inputStream, Stream outputStream, HttpRequest request)
        {
            Response = base.RouteRequest(inputStream, outputStream, request);

            return Response;

        }

        private Stream GenerateStreamFromString(string s)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
    }
}
