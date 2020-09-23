using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;

using MongoDB.Driver;

using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Imaging;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace ImageViewer
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        IMongoCollection<SpiderImage> ImageCollection { get; set; }
        public MainPage()
        {
            this.InitializeComponent();
            this.Loaded += MainPage_Loaded;
            var client = new MongoDB.Driver.MongoClient("mongodb://127.0.0.1:27017");
            var db = client.GetDatabase("spider");
            ImageCollection = db.GetCollection<SpiderImage>("image");
            btnPrev.Click += BtnPrev_Click;
            btnPrev5.Click += BtnPrev5_Click;
            btnNext.Click += BtnNext_Click;
            btnNext5.Click += BtnNext5_Click;
            btnDelete.Click += BtnDelete_Click;
            this.KeyUp += MainPage_KeyUp;

            sliderOpacity.ValueChanged += SliderOpacity_ValueChanged;
            btnRotate.Click += BtnRotate_Click;
            //btnZoomin.Click += BtnZoomin_Click;
            //btnZoomout.Click += BtnZoomout_Click;
            btnFlip.Click += BtnFlip_Click;

            imgScrollViewer.PointerWheelChanged += ImgScrollViewer_PointerWheelChanged;
        }

        private Storyboard flipboard = new Storyboard();
        private bool flipping = false;
        private void BtnFlip_Click(object sender, RoutedEventArgs e)
        {
            if (flipping)
            {
                flipboard.Stop();
                flipping = false;
            }
            else
            {
                var animation = new DoubleAnimation();
                animation.From = 0.0;
                animation.To = 360.0;
                animation.Duration = TimeSpan.FromSeconds(4);
                animation.RepeatBehavior = RepeatBehavior.Forever;
                Storyboard.SetTarget(animation, img);
                Storyboard.SetTargetProperty(animation, "(UIElement.Projection).(PlaneProjection.Rotation" + "Y" + ")");
                flipboard.Children.Clear();
                flipboard.Children.Add(animation);
                flipboard.Begin();
                flipping = true;
            }
        }

        private async void ImgScrollViewer_PointerWheelChanged(object sender, PointerRoutedEventArgs e)
        {
            if (e.KeyModifiers != Windows.System.VirtualKeyModifiers.Control) { return; }
            ScaleTransform transform = null;
            if (img.RenderTransform is ScaleTransform) { transform = img.RenderTransform as ScaleTransform; }
            else { transform = new ScaleTransform(); img.RenderTransform = transform; }
            if (transform.ScaleX != 1 && transform.ScaleY != 1 && imgScrollViewer.VerticalScrollBarVisibility != ScrollBarVisibility.Hidden) { imgScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden; }
            var pos = e.GetCurrentPoint(img);
            transform.CenterX = pos.Position.X;// img.ActualWidth / 2;
            transform.CenterY = pos.Position.Y;// img.ActualHeight / 2;

            double deltaValue = -1 * e.GetCurrentPoint(img).Properties.MouseWheelDelta;
            var scaleValue = (deltaValue > 0) ? 0.8 : 1.2;
            transform.ScaleX = transform.ScaleX * scaleValue;
            transform.ScaleY = transform.ScaleY * scaleValue;
            await Task.CompletedTask;
        }

        //private void BtnZoomin_Click(object sender, RoutedEventArgs e)
        //{
        //    ScaleTransform transform = null;
        //    if (img.RenderTransform is ScaleTransform) { transform = img.RenderTransform as ScaleTransform; }
        //    else { transform = new ScaleTransform(); img.RenderTransform = transform; }
        //    transform.CenterX = img.ActualWidth / 2;
        //    transform.CenterY = img.ActualHeight / 2;
        //    transform.ScaleY -= 0.1;
        //    transform.ScaleX -= 0.1;
        //}

        //private void BtnZoomout_Click(object sender, RoutedEventArgs e)
        //{
        //    ScaleTransform transform = null;
        //    if (img.RenderTransform is ScaleTransform) { transform = img.RenderTransform as ScaleTransform; }
        //    else { transform = new ScaleTransform(); img.RenderTransform = transform; }
        //    transform.CenterX = img.ActualWidth / 2;
        //    transform.CenterY = img.ActualHeight / 2;
        //    transform.ScaleY += 0.1;
        //    transform.ScaleX += 0.1;
        //}

        private async void BtnRotate_Click(object sender, RoutedEventArgs e)
        {
            RotateTransform transform = null;
            if (img.RenderTransform is RotateTransform) { transform = img.RenderTransform as RotateTransform; }
            else { transform = new RotateTransform(); img.RenderTransform = transform; }
            transform.Angle += 90;
            transform.CenterX = img.ActualWidth / 2;
            transform.CenterY = img.ActualHeight / 2;
            if (transform.Angle >= 360) { transform.Angle = 0; }
            await Task.CompletedTask;
        }

        private void SliderOpacity_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            img.Opacity = e.NewValue;
        }

        private async void BtnNext5_Click(object sender, RoutedEventArgs e)
        {
            skip += 5;
            await this.LoadImage();
        }

        private async void BtnPrev5_Click(object sender, RoutedEventArgs e)
        {
            skip -= 5;
            skip = Math.Max(0, skip);
            await this.LoadImage();
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

        private async void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentImage == null) { return; }
            await ImageCollection.DeleteOneAsync(c => c.Url == CurrentImage.Url);
            await this.LoadImage();
        }

        private async void BtnPrev_Click(object sender, RoutedEventArgs e)
        {
            skip -= 1;
            skip = Math.Max(0, skip);
            await this.LoadImage();
        }

        private async void BtnNext_Click(object sender, RoutedEventArgs e)
        {
            skip += 1;
            await this.LoadImage();
        }

        private int skip = 0;
        private int take = 1;
        private async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadImage();
        }

        private SpiderImage CurrentImage { get; set; }
        private async Task LoadImage()
        {
            if (imgScrollViewer.VerticalScrollBarVisibility != ScrollBarVisibility.Disabled) { imgScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled; }
            if (img.RenderTransform is ScaleTransform) { var transform = img.RenderTransform as ScaleTransform; transform.ScaleX = 1; transform.ScaleY = 1; }

            CurrentImage = ImageCollection.AsQueryable().Skip(skip).Take(take).FirstOrDefault();
            if (CurrentImage == null) { skip -= 1; return; }
            await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, async () =>
            {
                img.Source = await SaveToImageSource(CurrentImage);
            });
        }

        public async Task<ImageSource> SaveToImageSource(SpiderImage spiderImage)
        {
            ImageSource imageSource = null;
            using (MemoryStream stream = new MemoryStream(spiderImage.Content))
            {
                var ras = stream.AsRandomAccessStream();
                var decoderGuid = BitmapDecoder.JpegDecoderId;
                switch (spiderImage.ImageType.ToLower())
                {
                    case "image/png": { decoderGuid = BitmapDecoder.PngDecoderId; break; }
                    case "image/webp": { decoderGuid = BitmapDecoder.WebpDecoderId; break; }
                    case "image/gif": { decoderGuid = BitmapDecoder.GifDecoderId; break; }
                }
                BitmapDecoder decoder = await BitmapDecoder.CreateAsync(decoderGuid, ras);
                var provider = await decoder.GetPixelDataAsync();
                byte[] buffer = provider.DetachPixelData();
                WriteableBitmap bitmap = new WriteableBitmap((int)decoder.PixelWidth, (int)decoder.PixelHeight);
                await bitmap.PixelBuffer.AsStream().WriteAsync(buffer, 0, buffer.Length);
                imageSource = bitmap;
            }
            return imageSource;
        }
    }

    [MongoDB.Bson.Serialization.Attributes.BsonIgnoreExtraElements]
    public class SpiderImage
    {
        public string Url { get; set; }
        public string Description { get; set; }
        public string ImageType { get; set; }
        public byte[] Content { get; set; }
    }
}
