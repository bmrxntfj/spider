using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Spider
{
    class VideoM3u8Command : IXCommand
    {
        public string Description => "post video by .m3u8";

        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            var url = ConsoleCommand.ReadLine("is file path?");
            if (string.IsNullOrWhiteSpace(url)) { ConsoleCommand.WriteLine("please input a full url."); return; }
            if (url.EndsWith(".m3u8") == false) { return; }
            var desc = ConsoleCommand.ReadLine("is desc?");
            var headtext = ConsoleCommand.ReadLine("is request file or header text?");
            if (headtext.EndsWith(".req")) { headtext = System.IO.File.ReadAllText(headtext); }
            var httpClient = new HttpClient();
            var res = await httpClient.GetAsync(url);
            var response = await res.Content.ReadAsStringAsync();
            var uri = new Uri(url);
            var hosturl = $"{uri.Scheme}://{uri.Host}";
            var index = 0;
            var tses = response.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).Where(c => c.EndsWith(".ts")).Select(c => new TsFileInfo { Url = c, Index = index++ }).ToArray();
            var collectionname = Guid.NewGuid().ToString("n");
            Parallel.ForEach(tses, new ParallelOptions { CancellationToken = cancellationToken, MaxDegreeOfParallelism = Environment.ProcessorCount / 2 }, ts =>
            //tses.ForEach(ts=>
            {
                try
                {
                    ConsoleCommand.WriteLine(ts.Url);
                    var headers = headtext.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).ToDictionary(c => c.Split(':')[0], c => c.Split(':')[1]);
                    headers.Add("path", ts.Url);
                    PostToDB.Post($"video{collectionname}", $"{hosturl}{ts.Url}", desc, ts.Index, headers);
                }
                catch (Exception ex) { ConsoleCommand.WriteLine(ex.Message); }
            });
        }
    }

    public class TsFileInfo
    {
        public string Url { get; set; }
        public int Index { get; set; }
    }
}