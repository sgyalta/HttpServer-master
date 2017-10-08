﻿// Copyright (C) 2016 by David Jeske, Barend Erasmus and donated to the public domain

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using logger;
using Server.Models;

namespace Server
{
    public class HttpProcessor
    {

        #region Fields

        private static int _maxPostSize = 10 * 1024 * 1024; // 10MB

        private readonly List<Route> _routes = new List<Route>();


        #endregion

        #region Constructors
        public HttpProcessor()
        {
        }

        #endregion

        #region Public Methods
        public void HandleClient(TcpClient tcpClient)
        {
                var inputStream = GetInputStream(tcpClient);
                var outputStream = GetOutputStream(tcpClient);
                var request = GetRequest(inputStream);

                // route and handle the request...
                var response = RouteRequest(inputStream, outputStream, request);      
          
                Console.WriteLine("{0} {1}",response.StatusCode,request.Url);
                // build a default response for errors
                if (response.Content == null) {
                    if (response.StatusCode != "200") {
                        response.ContentAsUtf8 = ($"{response.StatusCode} {request.Url} <p> {response.ReasonPhrase}");
                        Logger.Log($"{response.StatusCode} {request.Method} {request.Url} <p> {response.ReasonPhrase}");
                }
                }

                WriteResponse(outputStream, response);

                outputStream.Flush();
                outputStream.Close();
/*
                outputStream = null;
*/

                inputStream.Close();
/*
                inputStream = null;
*/

        }

        // this formats the HTTP response...
        private static void WriteResponse(Stream stream, HttpResponse response) {            
            if (response.Content == null) {           
                response.Content = new byte[]{};
            }
            
            // default to text/html content type
            if (!response.Headers.ContainsKey("Content-Type")) {
                response.Headers["Content-Type"] = "text/html";
            }

            response.Headers["Content-Length"] = response.Content.Length.ToString();

            Write(stream, $"HTTP/1.0 {response.StatusCode} {response.ReasonPhrase}\r\n");
            Write(stream, string.Join("\r\n", response.Headers.Select(x => $"{x.Key}: {x.Value}")));
            Write(stream, "\r\n\r\n");

            stream.Write(response.Content, 0, response.Content.Length);       
        }

        public void AddRoute(Route route)
        {
            _routes.Add(route);
        }

        #endregion

        #region Private Methods

        private static string Readline(Stream stream)
        {
            var data = "";
            while (true)
            {
                var nextChar = stream.ReadByte();
                if (nextChar == '\n') { break; }
                if (nextChar == '\r') { continue; }
                if (nextChar == -1) { Thread.Sleep(1); continue; };
                data += Convert.ToChar(nextChar);
            }
            return data;
        }

        private static void Write(Stream stream, string text)
        {
            var bytes = Encoding.UTF8.GetBytes(text);
            stream.Write(bytes, 0, bytes.Length);
        }

        protected virtual Stream GetOutputStream(TcpClient tcpClient)
        {
            return tcpClient.GetStream();
        }

        protected virtual Stream GetInputStream(TcpClient tcpClient)
        {
            return tcpClient.GetStream();
        }

        protected virtual HttpResponse RouteRequest(Stream inputStream, Stream outputStream, HttpRequest request)
        {

            var routes = _routes.Where(x => Regex.Match(request.Url, x.UrlRegex).Success).ToList();

            if (!routes.Any())
                return HttpBuilder.NotFound();

            var route = routes.SingleOrDefault(x => x.Method == request.Method);

            if (route == null)
            {
                return new HttpResponse()
                {
                    ReasonPhrase = "Method Not Allowed",
                    StatusCode = "405",

                };
            }

            // extract the path if there is one
            var match = Regex.Match(request.Url,route.UrlRegex);
            request.Path = match.Groups.Count > 1 ? match.Groups[1].Value : request.Url;

            // trigger the route handler...
            request.Route = route;
            try {
                return route.Callable(request);
            } catch(Exception ex) {
                Logger.TalkyLog(ex);
                return HttpBuilder.InternalServerError();
            }

        }

        private static HttpRequest GetRequest(Stream inputStream)
        {
            //Read Request Line
            var request = Readline(inputStream);

            var tokens = request.Split(' ');
            if (tokens.Length != 3)
            {
                throw new Exception("invalid http request line");
            }
            var method = tokens[0].ToUpper();
            var url = tokens[1];
            var protocolVersion = tokens[2];

            //Read Headers
            var headers = new Dictionary<string, string>();
            string line;
            while ((line = Readline(inputStream)) != null)
            {
                if (line.Equals(""))
                {
                    break;
                }

                var separator = line.IndexOf(':');
                if (separator == -1)
                {
                    throw new Exception("invalid http header line: " + line);
                }
                var name = line.Substring(0, separator);
                var pos = separator + 1;
                while ((pos < line.Length) && (line[pos] == ' '))
                {
                    pos++;
                }

                var value = line.Substring(pos, line.Length - pos);
                headers.Add(name, value);
            }

            string content = null;
            if (!headers.ContainsKey("Content-Length"))
            {
                return new HttpRequest()
                {
                    Method = method,
                    Url = url,
                    Headers = headers,
                    Content = content
                };
            }

            var totalBytes = Convert.ToInt32(headers["Content-Length"]);
            var bytesLeft = totalBytes;
            var bytes = new byte[totalBytes];
               
            while(bytesLeft > 0)
            {
                var buffer = new byte[bytesLeft > 1024? 1024 : bytesLeft];
                var n = inputStream.Read(buffer, 0, buffer.Length);
                buffer.CopyTo(bytes, totalBytes - bytesLeft);

                bytesLeft -= n;
            }

            content = Encoding.ASCII.GetString(bytes);


            return new HttpRequest()
            {
                Method = method,
                Url = url,
                Headers = headers,
                Content = content
            };
        }

        #endregion


    }
}
