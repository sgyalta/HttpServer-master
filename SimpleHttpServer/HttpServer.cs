﻿// Copyright (C) 2016 by David Jeske, Barend Erasmus and donated to the public domain

using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using logger;
using Server.Models;

namespace Server
{

    public class HttpServer
    {
        #region Fields

        private readonly int _port;
        private TcpListener _listener;
        private readonly HttpProcessor _processor;
        private readonly bool _isActive = true;

        #endregion

        

        #region Public Methods
        public HttpServer(int port, IEnumerable<Route> routes)
        {
            _port = port;
            _processor = new HttpProcessor();

            foreach (var route in routes)
            {
                
                _processor.AddRoute(route);
            }
        }

        public void Listen()
        {
            _listener = new TcpListener(IPAddress.Any, _port);
            _listener.Start();
            while (_isActive)
            {
                var s = _listener.AcceptTcpClient();
                var thread = new Thread(() =>
                {
                    _processor.HandleClient(s);
                });
                thread.Start();
                Thread.Sleep(1);
            }
        }

        #endregion

    }
}



