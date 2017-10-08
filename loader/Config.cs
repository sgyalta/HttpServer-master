using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace loader
{
    public class Config
    {
        public static string Serialize(object what)
        {
            var sb = new StringBuilder();
            if (what.GetType().BaseType != typeof(Config)) return sb.ToString();
            foreach (var fld in what.GetType().GetFields())
            {
                if (fld.IsPublic && !fld.Name.StartsWith("_") && fld.IsStatic == false)
                {
                    if (fld.GetValue(what) is char)
                    {
                        sb.AppendLine($"{fld.Name} = {"\'" + fld.GetValue(what) + "\'"}");
                    }
                    else if (fld.GetValue(what) is string)
                    {
                        sb.AppendLine($"{fld.Name} = {"\"" + fld.GetValue(what) + "\""}");
                    }
                    else if (fld.GetValue(what).GetType() == typeof(string[]))
                    {
                        sb.Append(fld.Name).Append(" = ").AppendLine("[ " + string.Join(", ", fld.GetValue(what) as string[] ?? throw new InvalidOperationException()) + " ]");
                    }
                    else
                    {
                        sb.AppendLine($"{fld.Name} = {fld.GetValue(what)}");
                    }
                }
                else
                    throw new Exception("A serialized object must be an instance of a class extending Config");
            }

            return sb.ToString();
        }

        public static void Save(object what)
        {
            if (!Directory.Exists(Path.Combine(Environment.CurrentDirectory, $"Configs")))
                Directory.CreateDirectory(Path.Combine(Environment.CurrentDirectory, $"Configs"));
            var str = File.CreateText(Path.Combine(Environment.CurrentDirectory, $"Configs", what.GetType().Name + ".cfg"));
            str.WriteLine(Serialize(what));
            str.Flush();
            str.Close();
        }

        public void Deserialize()
        {
            var strs = File.ReadAllLines(Path.Combine(Environment.CurrentDirectory, "Configs", GetType().Name + ".cfg"));
            var fldlist = new Dictionary<string, object>();
            foreach (var str in strs)
            {
                var name = str.Replace($" = ", "=").Split('=')[0];
                var strvalue = string.Join($"=", str.Replace(" = ", "=").Split('=').Skip(1).ToArray());
                var value = new object();
                switch (strvalue)
                {
                    case "True":
                        value = true;
                        break;
                    case "False":
                        value = false;
                        break;
                    default:
                        if (!int.TryParse(strvalue, out var num))
                        {
                            if (strvalue.EndsWith("\"") && strvalue.StartsWith("\""))
                                value = strvalue.Substring(1, strvalue.Length - 2);
                            if (strvalue.EndsWith("\'") && strvalue.StartsWith("\'"))
                            {
                                if (strvalue.Length > 4)
                                    throw new Exception("Char value can only contain one character");
                                value = Convert.ToChar(strvalue.Substring(1, strvalue.Length - 2));
                            }
                        }
                        else
                            value = num;
                        break;
                }
                fldlist.Add(name, value);
            }
            try
            {
                foreach (var pair in fldlist)
                    GetType().GetField(pair.Key).SetValue(this, pair.Value);
            }
            catch
            {
                // ignored
            }
        }

        public Config() { }

        public Config(bool autoload)
        {
            if (!autoload) return;
            if (!File.Exists(Path.Combine(Environment.CurrentDirectory, $"Configs/{GetType().Name}.cfg")))
            {
                Save(this);
                Deserialize();
            }
            else
            {
                Deserialize();
                Save(this);
            }
        }
    }

    namespace Models.Configs
    {
        public class Default : Config
        {
            public string Codepath = "Server";
            public string Componentspath = "Components";
            public string Cheme = "https://";
            public string URI = "mytechdoc.toyota-europe.com";
            public int Defaultport = 22425;
            public bool Verbose = false;
            public int MaxPosTmb = 10;
            public bool Asynchlog = true;
            public string Charset = "utf-8";
            public string Logname = "Events.log";

            public Default()
            {
            }
        }

            

        public class Cmd : Config
        {
            private readonly bool _autoload;
            public string ConsoleReadyString = "->";
            public char CommandDelimiter = ';';
            public string[] Arr = { "meow", "test" };

            public Cmd(bool autoload) : base(true) => _autoload = autoload;
            public Cmd() { }
        }
    }
}
