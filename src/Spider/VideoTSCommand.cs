using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using AngleSharp.Common;

namespace Spider
{
    class VideoTSCommand : IXCommand
    {
        public string Description => "post video by .ts";

        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            var url = ConsoleCommand.ReadLine("is file path?");
            if (string.IsNullOrWhiteSpace(url)) { ConsoleCommand.WriteLine("please input a full url."); return; }
            var desc = ConsoleCommand.ReadLine("is desc?");
            var headtext = ConsoleCommand.ReadLine("is request file or header text?");
            if (headtext.EndsWith(".req")) { headtext = System.IO.File.ReadAllText(headtext); }
            var index = 0;
            int.TryParse(ConsoleCommand.ReadLine("is index?"), out index);
            var headers = headtext.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).ToDictionary(c => c.Split(':')[0], c => c.Split(':')[1]);
            headers.Add("path", new Uri(url).PathAndQuery);
            await PostToDB.PostAsync("video", url, desc, index, headers);
        }
    }
}
