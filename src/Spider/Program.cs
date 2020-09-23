using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using System.Threading;
using System.Net.Http;
using System.Threading.Tasks;
using System.IO;

namespace Spider
{
    class Program
    {
        static async Task Main(string[] args)
        {
            typeof(Program).Assembly.GetTypes().Where(c => c.IsInherit(typeof(IXCommand))).ForEach(c =>
            {
                var cmd = Activator.CreateInstance(c) as IXCommand;
                ConsoleCommand.Add(cmd.ExecuteAsync, cmd.Description);
            });

            await ConsoleCommand.Run();
        }
    }
    interface IXCommand
    {
        string Description { get; }
        Task ExecuteAsync(CancellationToken cancellationToken);
    }
}
