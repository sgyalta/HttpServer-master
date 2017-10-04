// Copyright (C) 2016 by Barend Erasmus and donated to the public domain

using System;
using System.ComponentModel;
using System.IO;
using System.Threading;
using SimpleHttpServer.WebApp.Models;
using SimpleHttpServer.WebApp.Models.Models.Configs;
using Console = SimpleHttpServer.WebApp.Models.Models.Configs.Cmd;

namespace SimpleHttpServer.WebApp
{
    static class Program
    {
        public static bool Verbose;
        public static Default _cfg = new Default();
        public static Cmd ConsoleCfg = new Cmd(true);
        public static volatile bool PendingLogSave = false;
        public static event EventHandler<HandledEventArgs> Exit = delegate { };
        public static Thread ConsoleThread;
        public static Thread ControlThread;

        public static Thread LoggerThread;

        private static void Main(string[] args)
        {
            PerformFileCheck();
            _cfg = new Default();
            HttpServer httpServer;
            System.Console.Title = _cfg.Cheme + _cfg.URI + ":" + 80;
            if (args.Length > 0)
            {
                System.Console.Title = _cfg.Cheme + _cfg.URI + ":" + args[0];
                httpServer = new HttpServer(Convert.ToInt16(args[0]), Routes.Get);
            }
            else
            {
                System.Console.Title = _cfg.Cheme + _cfg.URI + ":" + _cfg.Defaultport;
                httpServer = new HttpServer(Convert.ToInt16(_cfg.Defaultport), Routes.Get);
            }
            LoggerThread = new Thread(Logger.LogWorker);
            LoggerThread.Start();
            var thread = new Thread(httpServer.Listen);
            thread.Start();

            ConsoleThread = new Thread(CmdConsole.Start);
            ConsoleThread.Start();
            ControlThread = new Thread(Control);
        }



        public static void PerformFileCheck()
        {
            if (!Directory.Exists(Path.Combine(Environment.CurrentDirectory, "Configs")))
                Directory.CreateDirectory(Path.Combine(Environment.CurrentDirectory, "Configs"));
            if (!File.Exists(Path.Combine(Environment.CurrentDirectory, "Configs/Default.cfg")))
            {
                Config.Save(_cfg);
                _cfg.Deserialize();
            }
            else
            {
                _cfg.Deserialize();
                Config.Save(_cfg);
            }

            Verbose = _cfg.Verbose;
            if (!Directory.Exists(Path.Combine(Environment.CurrentDirectory, _cfg.Codepath)))
                Directory.CreateDirectory(Path.Combine(Environment.CurrentDirectory, _cfg.Codepath));
        }

        private static void Control()
        {
            while (true)
            {
                if (ConsoleThread.IsAlive || LoggerThread.IsAlive) continue;
                OnExit();
                Thread.CurrentThread.Abort();
                return;
            }
        }

        public static void OnExit()
        {
            Exit(null, new HandledEventArgs());
        }
    }
}