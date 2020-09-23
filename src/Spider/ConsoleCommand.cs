using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spider
{
    public class CommandStatement
    {
        public int Id { get; internal set; }
        public Func<System.Threading.CancellationToken, Task> Action { get; set; }
        public string Description { get; set; }
        public bool ShowTip { get; internal set; }
        public bool AutoRun { get; set; }
    }

    public static class ConsoleCommand
    {
        static ConsoleCommand()
        {
            ConsoleCommand.Add((token) => Task.Factory.StartNew(() =>
            {
                var builder = new StringBuilder();
                for (var i = 0; i < Commands.Count; i++)
                {
                    var a = Commands.Skip(i).Take(1).First();
                    var tip = a.Description;
                    if (tip == string.Empty) { tip = a.Action.Method.Name; }
                    builder.AppendLine(string.Format("{0}:{1}", i, tip));
                }
                Info(builder.ToString());
            }), "show menu", true, false);
            ConsoleCommand.Add((token) => Task.CompletedTask, "exit", false, false);
        }
        //static ILog log = LogManager.GetLog("ConsoleCommand");
        static void Info(string message)
        {
            Console.WriteLine(message);
        }
        static List<CommandStatement> Commands = new List<CommandStatement>();

        static async Task ExecuteCommand(int i, bool tip)
        {
            if (i < 0 || i > Commands.Count - 1) { Info(string.Format("Can't find command index of {0}.", i)); return; }
            var a = Commands.Skip(i).Take(1).FirstOrDefault();
            await ExecuteCommand(a);
        }
        static async Task ExecuteCommand(CommandStatement command)
        {
            if (command.ShowTip) { Info(string.Format("command {0} found,execute immediately...", command.Id)); }
            var startTime = DateTime.Now;
            try
            {
                await command.Action(CancellationTokenSource.Token);
                if (command.ShowTip)
                {
                    Info(string.Format("command {0} execute end, all spent {1}.", command.Id, (DateTime.Now - startTime).TotalSeconds));
                }
                Info("type a command's id");
            }
            catch (Exception ex)
            {
                Info(ex.ToString());
            }
        }

        public static void Add(Func<System.Threading.CancellationToken, Task> handler, string tip, bool autoRun = false, bool showTip = true)
        {
            Commands.Add(new CommandStatement { Id = Commands.Count, Action = handler, Description = tip, ShowTip = showTip, AutoRun = autoRun });
        }
        static bool OutputSwitch = true;

        public static string ReadLine(string tip = null)
        {
            if (tip != null) { Console.WriteLine(tip); }
            return Console.ReadLine();
        }

        public static void Write(string value)
        {
            if (OutputSwitch)
            {
                Info(value);
            }
        }

        public static void WriteLine(string value)
        {
            if (OutputSwitch)
            {
                Info(value);
            }
        }

        public static System.Threading.CancellationTokenSource CancellationTokenSource = new System.Threading.CancellationTokenSource();
        public async static Task Run()
        {
            foreach (var c in Commands.Where(c => c.AutoRun == true))
            {
                await ExecuteCommand(c);
            }
            var consoleColorRandom = new Random(1);
            while (!CancellationTokenSource.IsCancellationRequested)
            {
                var key = Console.ReadLine();
                if (key == "1")
                {
                    Info("quit...");
                    CancellationTokenSource.Cancel();
                    System.Threading.Thread.Sleep(3 * 1000);
                    break;
                }
                else if (string.IsNullOrWhiteSpace(key))
                {
                    OutputSwitch = !OutputSwitch;
                    Info(string.Format("Log is {0}.", OutputSwitch ? "ON" : "OFF"));
                    continue;
                }
                if (!int.TryParse(key, out int messageId)) { messageId = -1; }
                await ExecuteCommand(messageId, true);
            }
        }
    }
}
