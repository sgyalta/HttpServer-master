using logger;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace appobj
{
    public delegate void Command(string[] args);
    public static class CmdConsole
    {
        public static List<ValuePair<string, Command>> CmdList = new List<ValuePair<string, Command>>();
        public static List<string> HelpStrings = new List<string>();
        public static char CommandDelimiter = loader.Init.ConsoleCfg.CommandDelimiter;
        public static ObservableCollection<string> CommandQuery = new ObservableCollection<string>();

        public static void Start()
        {
            CommonCommands.IsInitialized = true;
            while (true)
            {
                Console.Write(loader.Init.ConsoleCfg.ConsoleReadyString);
                CommandSelector(Console.ReadLine());
            }
        }

        public static void CommandSelector(string line)
        {
            Logger.LogNoTrace(loader.Init.ConsoleCfg.ConsoleReadyString + line);
            var temp = "";
            var index = 0;
            var isString = false;
            var found = false;
            var precedingchar = '\n';
            var commands = new List<string>();
            foreach (var ch in line)
            {
                if (ch == '\"')
                {
                    isString = !isString;
                    temp += ch;
                    continue;
                }
                if (ch == CommandDelimiter && index != 0 && precedingchar != '\\' && !isString && precedingchar != CommandDelimiter)
                {
                    commands.Add(temp);
                    temp = "";
                }
                else if (precedingchar + ch != CommandDelimiter + ' ')
                    temp += ch;

                if (index == line.Length - 1 && ch != CommandDelimiter)
                {
                    commands.Add(temp);
                    temp = "";
                }
                index++;
                precedingchar = ch;
            }
            foreach (var command in commands)
            {
                CommandQuery.Add(command);
                foreach (var cmd in CmdList)
                {
                    if (!command.StartsWith(cmd.LeftValue)) continue;
                    cmd.RightValue.Invoke(
                        command.Substring(cmd.LeftValue.Length).TrimStart().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                    );
                    found = true;
                    break;
                }

                if (found) continue;
                if (!command.StartsWith(" "))
                    Logger.TalkyLog("No commands with the specified name found: " + (command.Contains(" ") ? command.Substring(0, command.IndexOf(' ')) : command));
                else
                    Logger.TalkyLog("Command mustn't start with a space");
            }
            Logger.Save();
        }
    }
    public static class CommonCommands
    {
        public static bool IsInitialized;
        public static Hashtable Variables = new Hashtable();
        public static ObservableCollection<string> Labels = new ObservableCollection<string>();
        static CommonCommands()
        {
            CmdConsole.CmdList.Add(new ValuePair<string, Command>("help", Help));
            CmdConsole.CmdList.Add(new ValuePair<string, Command>("test", Test));
            CmdConsole.CmdList.Add(new ValuePair<string, Command>("quit", Quit));
            CmdConsole.CmdList.Add(new ValuePair<string, Command>("exit", Quit));
            CmdConsole.CmdList.Add(new ValuePair<string, Command>("set", Set));
            CmdConsole.CmdList.Add(new ValuePair<string, Command>("get", Get));
            CmdConsole.CmdList.Add(new ValuePair<string, Command>("label", Label));
            CmdConsole.CmdList.Add(new ValuePair<string, Command>("goto", Goto));

        }
        public static void Goto(string[] args)
        {
            Console.WriteLine(args[0]);
            if (!Labels.Contains(args[0])) return;

            Console.WriteLine("yes");
            if (CmdConsole.CommandQuery.Contains("end: " + args[0]))
                for (var i = CmdConsole.CommandQuery.IndexOf("lbl: " + args[0]) + 1; i < CmdConsole.CommandQuery.IndexOf("end: " + args[0]); i++)
                {
                    if (CmdConsole.CommandQuery[i].StartsWith("lbl:") || CmdConsole.CommandQuery[i].StartsWith("end:"))
                        continue;
                    CmdConsole.CommandSelector(CmdConsole.CommandQuery[i]);
                }
        }

        public static void Label(string[] args)
        {
            switch (args.Length)
            {
                case 0:
                    Logger.Log("Name must be specified");
                    break;
                case 1:
                    if (!Labels.Contains(args[0]))
                    {
                        CmdConsole.CommandQuery.Add("lbl: " + args[0]);
                        Labels.Add(args[0]);
                    }
                    else
                        Logger.Log("Label already exists");
                    break;
                case 2:
                    if (args[1] == "end")
                        CmdConsole.CommandQuery.Add("end: " + args[0]);
                    else
                        Logger.Log("Unknown sub-command");
                    break;
            }
        }

        public static void Get(string[] args)
        {
            switch (args.Length)
            {
                case 0:
                    Logger.Log(
                            "Usage:\n",
                            "get varname; - returns value of a variable\n",
                            "get varname push command {}; - passes value of a variable to a command\n",
                            "Description: Allows the user to retrieve values of the variables they stored\n"
                        );
                    return;
                default:
                    Logger.Log(Variables[args[0]]);
                    return;
            }
        }

        public static void Set(string[] args)
        {
            if (args.Length == 0)
            {
                Logger.Log(
                        "Usage:\n",
                        "set varname; - create an empty variable\n",
                        "set varname = anything; - create a variable with the value of anything\n",
                        "Description: The set provides the user with ability to create runtime script-like variables,\n whose value can be later fetched and used with get\n"
                    );
                return;
            }
            if (Variables.ContainsKey(args[0]))
            {
                if (args.Length == 1)
                {
                    Logger.Log(args[0], " already exists");
                }
                else
                {
                    if (args[1] != "=")
                        Logger.Log("Syntax error: ", string.Join(" ", args));
                    else
                    {
                        if (args.Length < 3)
                            Logger.Log("Syntax error: ", string.Join(" ", args));
                        else
                        {
                            Variables.Remove(args[0]);
                            Variables.Add(args[0], string.Join(" ", args).Replace(args[0] + " = ", ""));
                            Logger.Log($"{args[0]} set to {Variables[args[0]]}");
                        }
                    }
                }
            }
            else
            {
                if (args.Length == 1)
                {
                    Variables.Add(args[0], null);
                    Logger.Log(args[0], " added");
                }
                else
                {
                    if (args[1] != "=" || args.Length < 3)
                        Logger.Log("Syntax error: ", string.Join(" ", args));
                    else
                    {
                        Variables.Add(args[0], string.Join(" ", args).Replace(args[0] + " = ", ""));
                        Logger.Log($"{args[0]} set to {Variables[args[0]]}");
                    }
                }
            }
        }

        public static void Help(string[] args)
        {
            Logger.Log("Available commands:");
            Logger.Log("===================");
            foreach (var cmd in CmdConsole.CmdList)
            {
                Console.Write(cmd.LeftValue + ", ");
            }

            Logger.Log();
        }

        public static void Test(string[] args)
        {
            foreach (var arg in args)
                Logger.Log(arg);
        }

        public static void Quit(string[] args)
        {
            Logger.Log("Shuting down....");
           
            Logger.Log("Exited control-thread.");
            Logger.Save();
            Logger.Log("Listener thread exited");
            loader.Init.OnExit();
            Environment.Exit(0);
        }
    }
}
