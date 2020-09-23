using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Spider
{
    class PostToDB
    {
        private static HttpClient HttpClient { get; set; }
        private static MongoDB.Driver.MongoClient MongoClient { get; set; }
        private static MongoDB.Driver.IMongoDatabase MongoDatabase { get; set; }
        static PostToDB()
        {
            HttpClient = new HttpClient();
            MongoClient = new MongoDB.Driver.MongoClient("mongodb://127.0.0.1:27017");
            MongoDatabase = MongoClient.GetDatabase("spider");
        }
        public static async Task PostAsync(string collectionname, string url, string desc, int index, Dictionary<string, string> headers = null)
        {
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri(url),
                Method = HttpMethod.Get
            };
            if (headers != null && headers.Count > 0)
            {
                headers.ForEach(c =>
                {
                    request.Headers.Add(c.Key, c.Value);
                });
            }
            var res = await HttpClient.SendAsync(request);
            var contentType = res.Content.Headers.GetValues("Content-Type").Join(";");
            var data = await res.Content.ReadAsByteArrayAsync();
            var spiderData = new SpiderData { Url = url, Content = data, ContentType = contentType, Description = desc, Index = index };
            await UploadAsync(collectionname, spiderData);
        }
        public static async Task UploadAsync<T>(string collectionname, T t)
        {
            var collection = MongoDatabase.GetCollection<T>(collectionname);
            await collection.InsertOneAsync(t);
        }

        public static void Post(string collectionname, string url, string desc, int index, Dictionary<string, string> headers = null)
        {
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri(url),
                Method = HttpMethod.Get
            };
            if (headers != null && headers.Count > 0)
            {
                headers.ForEach(c =>
                {
                    request.Headers.Add(c.Key, c.Value);
                });
            }
            var res = HttpClient.SendAsync(request).GetAwaiter().GetResult();
            var contentType = res.Content.Headers.GetValues("Content-Type").Join(";");
            var data = res.Content.ReadAsByteArrayAsync().GetAwaiter().GetResult();
            var spiderData = new SpiderData { Url = url, Content = data, ContentType = contentType, Description = desc, Index = index };
            Upload(collectionname, spiderData);
        }
        public static void Upload<T>(string collectionname, T t)
        {
            var collection = MongoDatabase.GetCollection<T>(collectionname);
            collection.InsertOneAsync(t);
        }
    }

    public class SpiderData
    {
        public string Url { get; set; }
        public string Description { get; set; }
        public string ContentType { get; set; }
        public int Index { get; set; }
        public byte[] Content { get; set; }
    }
}
