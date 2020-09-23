using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Spider
{
    class FileImageCommand : IXCommand
    {
        public string Description => "post local image";

        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            var url = ConsoleCommand.ReadLine("is file path?");
            if (string.IsNullOrWhiteSpace(url)) { ConsoleCommand.WriteLine("please input a full url."); return; }
            var desc = ConsoleCommand.ReadLine("is desc?");
            var imageType = ConsoleCommand.ReadLine("is image type?");

            await PostToDB.UploadAsync("image", new SpiderData { Url = string.Empty, Description = desc, ContentType = imageType, Content = File.ReadAllBytes(url) });
        }
    }
}
