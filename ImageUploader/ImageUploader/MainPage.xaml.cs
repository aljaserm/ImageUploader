using Microsoft.WindowsAzure.Storage;
using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ImageUploader
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

         async void SelecetImageButton_Clicked(object sender, EventArgs e)
        {
            await CrossMedia.Current.Initialize();
            if (!CrossMedia.Current.IsPickPhotoSupported)
            {
                await DisplayAlert("error", "Not Supported","OK");
                return;
            }
            var mediaOption = new PickMediaOptions()
            {
                PhotoSize = PhotoSize.Medium
            };
            var selectedimageFile = await CrossMedia.Current.PickPhotoAsync(mediaOption);
            if(selectedimageFile == null)
            {
                await DisplayAlert("error", "No Image", "OK");
                return;
            }
            imgSelected.Source = ImageSource.FromStream(() => selectedimageFile.GetStream());
            ImageUploader(selectedimageFile.GetStream());
        }

        private async void ImageUploader(Stream stream)
        {
            var acct = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=imagestoragemj;AccountKey=z8vLQ1JSYPLPeBgIggTEcksgYcE+1NzMFLgnS1QPUBrMkEk+++rESwul/CrUAzNJrg+13bHDhtXqEp009pPcmw==;EndpointSuffix=core.windows.net");
            var blobclient = acct.CreateCloudBlobClient();
            var container = blobclient.GetContainerReference("imagecontainer");
            await container.CreateIfNotExistsAsync();
            var name = Guid.NewGuid().ToString();
            var blobclock = container.GetBlockBlobReference($"{name}.jpg");
            await blobclock.UploadFromStreamAsync(stream);
            var url=blobclock.Uri.OriginalString;
        }
    }
}
