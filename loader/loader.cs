using System;
using loader.Models.Configs;
using System.ComponentModel;
using System.IO;

namespace loader
{
    
    public static class Init
    {

        public static bool Verbose;
        public static Default _cfg = new Default();
        public static Cmd ConsoleCfg = new Cmd(true);
        public static volatile bool PendingLogSave = false;
        public static event EventHandler<HandledEventArgs> Exit = delegate { };



        public static void PerformFileCheck()
        {
            if (!Directory.Exists(Path.Combine(Environment.CurrentDirectory, "Configs")))
            {
                Directory.CreateDirectory(Path.Combine(Environment.CurrentDirectory, "Configs"));
            }

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

            if (!Directory.Exists(Path.Combine(Environment.CurrentDirectory, _cfg.Codepath)))
            {
                Directory.CreateDirectory(Path.Combine(Environment.CurrentDirectory, _cfg.Codepath));
            }

        }
        public static void OnExit()
        {
            Exit(null, new HandledEventArgs());
        }
    }
}

