using System;
using System.ComponentModel;
using System.Threading;
using loader;
using appobj;
namespace SimpleHttpServer.WebApp
{
    static class Program
    {
       
        public static Thread ConsoleThread;
        public static Thread ControlThread;
        public static Thread LoggerThread;
        public static event EventHandler<HandledEventArgs> Exit = delegate { };


        private static void Main(string[] args)
        {
            Init.PerformFileCheck();
            
            var _cfg = Init._cfg;
            loader.Config.Save(_cfg);
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
            LoggerThread = new Thread(start: logger.Logger.LogWorker);
            LoggerThread.Start();
            var thread = new Thread(start: httpServer.Listen);
            thread.Start();

            ConsoleThread = new Thread(CmdConsole.Start);
            ConsoleThread.Start();
            ControlThread = new Thread(Control);
            
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