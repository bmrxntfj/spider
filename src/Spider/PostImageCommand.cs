using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Spider
{
    class PostImageCommand : IXCommand
    {
        public string Description => "post image by url";

        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            var url = ConsoleCommand.ReadLine("is url?");
            if (string.IsNullOrWhiteSpace(url)) { ConsoleCommand.WriteLine("please input a full url."); return; }
            var desc = ConsoleCommand.ReadLine("is desc?");
            await PostToDB.PostAsync("image", url, desc, 0);
        }
    }
}
