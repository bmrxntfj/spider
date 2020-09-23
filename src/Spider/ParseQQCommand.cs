using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using AngleSharp;

namespace Spider
{
    class ParseQQCommand : IXCommand
    {
        public string Description => "parse qq web's url";

        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            var url = ConsoleCommand.ReadLine("is url?");
            if (string.IsNullOrWhiteSpace(url)) { ConsoleCommand.WriteLine("please input a full url."); return; }
            var doc = await AngleSharp.BrowsingContext.New(Configuration.Default.WithDefaultLoader()).OpenAsync(url);
            var elements = doc.QuerySelectorAll("img");
            foreach (var element in elements)
            {
                if (element.HasAttribute("data-width") == false || element.HasAttribute("data-height") == false) { continue; }
                var src = element.GetAttribute("data-src");
                if (src.StartsWith("//")) { src = $"{doc.BaseUrl.Scheme}:{src}"; }
                ConsoleCommand.WriteLine(src);
                await PostToDB.PostAsync("image", src, string.Empty, 0);
            }
        }
    }
}
