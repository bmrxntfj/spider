using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;

using MongoDB.Driver;

using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace VideoViewer
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private string[] VideoCollectionNames { get; set; }
        private IMongoDatabase MongoDatabase { get; set; }
        public MainPage()
        {
            this.InitializeComponent();
            var client = new MongoDB.Driver.MongoClient("mongodb://127.0.0.1:27017");
            MongoDatabase = client.GetDatabase("spider");
            this.Loaded += MainPage_Loaded;
            this.KeyUp += MainPage_KeyUp;
            btnPlayFile.Click += BtnPlayFile_Click;
            btnPrev.Click += BtnPrev_Click;
            btnNext.Click += BtnNext_Click;
            btnPause.Click += BtnPause_Click;
            btnPlay.Click += BtnPlay_Click;
            btnDelete.Click += BtnDelete_Click;
        }

        private async void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (this.VideoCollectionNames.Length <= CurrentVideoCollectionIndex || CurrentVideoCollectionIndex < 0) { return; }
            await this.MongoDatabase.DropCollectionAsync(this.VideoCollectionNames[CurrentVideoCollectionIndex]);
            await this.LoadVideo();
        }

        private void BtnPlay_Click(object sender, RoutedEventArgs e)
        {
            _mediaPlayerElement.MediaPlayer.Play();
        }

        private void BtnPause_Click(object sender, RoutedEventArgs e)
        {
            _mediaPlayerElement.MediaPlayer.Pause();
        }

        private async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            var collectionnames = await this.MongoDatabase.ListCollectionNames().ToListAsync();
            VideoCollectionNames = collectionnames.Where(c => c.StartsWith("video")).ToArray();
            var mediaPlayer = new Windows.Media.Playback.MediaPlayer();
            mediaPlayer.TimelineController = new Windows.Media.MediaTimelineController();
            _mediaPlayerElement.SetMediaPlayer(mediaPlayer);
            //await LoadVideo();
        }

        private void MainPage_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Left || e.Key == Windows.System.VirtualKey.Up)
            {
                BtnPrev_Click(sender, e);
            }
            else if (e.Key == Windows.System.VirtualKey.Right || e.Key == Windows.System.VirtualKey.Down)
            {
                BtnNext_Click(sender, e);
            }
        }

        private async void BtnPlayFile_Click(object sender, RoutedEventArgs e)
        {
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.ViewMode = PickerViewMode.List;
            openPicker.SuggestedStartLocation = PickerLocationId.VideosLibrary;
            openPicker.FileTypeFilter.Add(".ts");

            var file = await openPicker.PickSingleFileAsync();
            _mediaPlayerElement.MediaPlayer.Source = Windows.Media.Core.MediaSource.CreateFromStorageFile(file);
            _mediaPlayerElement.MediaPlayer.Play();
        }

        private async void BtnPrev_Click(object sender, RoutedEventArgs e)
        {
            CurrentVideoCollectionIndex -= 1;
            CurrentVideoCollectionIndex = Math.Max(0, CurrentVideoCollectionIndex);
            await this.LoadVideo();
        }

        private async void BtnNext_Click(object sender, RoutedEventArgs e)
        {
            CurrentVideoCollectionIndex += 1;
            await this.LoadVideo();
        }
        private int CurrentVideoCollectionIndex = 0;

        //private IMongoCollection<SpiderVideo> CurrentVideoCollection { get; set; }
        //private SpiderVideo[] CurrentSpiderVideos { get; set; }
        private async Task LoadVideo()
        {
            var currentVideoCollection = this.MongoDatabase.GetCollection<SpiderVideo>(this.VideoCollectionNames[CurrentVideoCollectionIndex]);
            var skip = 0;
            var take = 1;
            var spiderVideos = new List<SpiderVideo>();
            do
            {
                var currentSpiderVideos = currentVideoCollection.AsQueryable().Skip(skip).Take(take).ToArray();
                if (currentSpiderVideos.Length == 0) { break; }
                skip += take;
                spiderVideos.AddRange(currentSpiderVideos);
            }
            while (true);
            var videoContent = spiderVideos.OrderBy(c => c.Index).SelectMany(c => c.Content).ToArray();
            var videoContentType = spiderVideos[0].ContentType;
            await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, async () =>
            {
                _mediaPlayerElement.MediaPlayer.Source = Windows.Media.Core.MediaSource.CreateFromStream(await GetRandomAccessStream(videoContent), videoContentType);
                _mediaPlayerElement.MediaPlayer.TimelineController.Start();
            });
        }

        public async Task<IRandomAccessStream> GetRandomAccessStream(byte[] bytes)
        {
            IRandomAccessStream randomStream = new InMemoryRandomAccessStream();
            DataWriter dataWriter = new DataWriter(randomStream);
            dataWriter.WriteBytes(bytes);
            await dataWriter.StoreAsync();
            return randomStream;
        }
    }

    [MongoDB.Bson.Serialization.Attributes.BsonIgnoreExtraElements]
    public class SpiderVideo
    {
        public string Url { get; set; }
        public string Description { get; set; }
        public string ContentType { get; set; }
        public int Index { get; set; }
        public byte[] Content { get; set; }
    }
}
