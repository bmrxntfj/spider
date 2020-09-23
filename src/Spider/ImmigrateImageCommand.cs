//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;

//using MongoDB.Driver;
//using MongoDB.Driver.Linq;

//namespace Spider
//{
//    class ImmigrateImageCommand : IXCommand
//    {
//        public string Description => "immigrate image";

//        private MongoDB.Driver.MongoClient MongoClient { get; set; }
//        private MongoDB.Driver.IMongoDatabase MongoDatabase { get; set; }
//        private MongoDB.Driver.IMongoCollection<SpiderImage> MongoCollection { get; set; }
//        public ImmigrateImageCommand()
//        {
//            MongoClient = new MongoDB.Driver.MongoClient("mongodb://127.0.0.1:27017");
//            MongoDatabase = MongoClient.GetDatabase("spider");
//            MongoCollection = MongoDatabase.GetCollection<SpiderImage>("image");
//        }

//        public async Task ExecuteAsync(CancellationToken cancellationToken)
//        {
//            var skip = 0;
//            var take = 1000;
//            do
//            {
//                var images = await MongoCollection.AsQueryable().Skip(skip).Take(take).ToListAsync();
//                if (images.Count == 0) { break; }
//                skip += take;
//                var updateOps= images.Select(image =>  new UpdateOneModel<SpiderImage>(Builders<SpiderImage>.Filter.Eq(c => c._id,image._id), Builders<SpiderImage>.Update.Set(c => c.ContentType, image.ImageType))).ToArray();
//                await MongoCollection.BulkWriteAsync(updateOps);
//            }
//            while (true);
//        }
//    }

//    public class SpiderImage
//    {
//        public MongoDB.Bson.ObjectId _id { get; set; }
//        public string Url { get; set; }
//        public string Description { get; set; }
//        public string ImageType { get; set; }
//        public string ContentType { get; set; }
//        public int Index { get; set; }
//        public byte[] Content { get; set; }
//    }
//}
