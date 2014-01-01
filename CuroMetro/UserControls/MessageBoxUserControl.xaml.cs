using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Windows.Storage;
using CrouMetro.Core.Managers;
using CrouMetro.Core.Tools;
using CrouMetro.Views;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;

namespace CrouMetro.UserControls
{
    public partial class MessageBoxUserControl : UserControl
    {
        private readonly CameraCaptureTask _cameraCaptureTask;

        public MessageBoxUserControl()
        {
            InitializeComponent();
            _cameraCaptureTask = new CameraCaptureTask();
            _cameraCaptureTask.Completed += cameraCaptureTask_Completed;
            StatusTextCount.Text = string.Format("{0}/{1}", StatusUpdateBox.Text.Length, Constants.STATUS_LIMIT);
        }

        public StorageFile File { get; private set; }

        private void StatusUpdateBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            StatusTextCount.Text = string.Format("{0}/{1}", StatusUpdateBox.Text.Length, Constants.STATUS_LIMIT);
            if (StatusUpdateBox.Text.Length > Constants.STATUS_LIMIT)
            {
                StatusTextCount.Foreground = new SolidColorBrush(Colors.Red);
                StatusPostButton.IsEnabled = false;
                ImagePicker.IsEnabled = false;
            }
            else
            {
                StatusTextCount.Foreground = new SolidColorBrush(Colors.White);
                StatusPostButton.IsEnabled = true;
                ImagePicker.IsEnabled = true;
            }
        }

        private async void StatusPostButton_Click(object sender, RoutedEventArgs e)
        {
            progressBar.Visibility = Visibility.Visible;
            StatusPostButton.IsEnabled = false;
            ImagePicker.IsEnabled = false;
            CameraAccess.IsEnabled = false;
            //ImagePicker.IsEnabled = false;

            bool result = false;
            if (MessagePost.IsSecretMail)
            {
                result =
                    await
                        SecretMailManager.CreateMail(StatusUpdateBox.Text, MessagePost.UserId, string.Empty,
                            App.userAccountEntity);
            }
            else if (ImagePickerImage.Source != null)
            {
                var bmp = new WriteableBitmap((BitmapSource) ImagePickerImage.Source);
                byte[] byteArray;
                using (var stream = new MemoryStream())
                {
                    bmp.SaveJpeg(stream, bmp.PixelWidth, bmp.PixelHeight, 0, 100);

                    byteArray = stream.ToArray();
                }
                result =
                    await
                        StatusManager.UpdateStatusWithMedia(StatusUpdateBox.Text, ".jpg", byteArray,
                            MessagePost.ReplyStatusId, MessagePost.ReplyStatusId.HasValue, null, App.userAccountEntity);
            }
            else
            {
                result =
                    await
                        StatusManager.UpdateStatus(StatusUpdateBox.Text, MessagePost.ReplyStatusId,
                            MessagePost.ReplyStatusId.HasValue, null, App.userAccountEntity);
            }
            var rootFrame = Application.Current.RootVisual as PhoneApplicationFrame;
            if (rootFrame != null)
                rootFrame.Navigate(new Uri("/MainTimelinePivot.xaml", UriKind.Relative));
        }

        private void ImagePicker_Click(object sender, RoutedEventArgs e)
        {
            PhotoChooserTask photoChooserTask;
            photoChooserTask = new PhotoChooserTask();
            photoChooserTask.Completed += photoChooserTask_Completed;
            photoChooserTask.Show();
        }

        private void photoChooserTask_Completed(object sender, PhotoResult e)
        {
            if (e.TaskResult == TaskResult.OK)
            {
                //MessageBox.Show(e.ChosenPhoto.Length.ToString());

                //Code to display the photo on the page in an image control named myImage.
                var bmp = new BitmapImage();
                bmp.SetSource(e.ChosenPhoto);
                ImagePickerImage.Source = bmp;
            }
        }

        private void cameraCaptureTask_Completed(object sender, PhotoResult e)
        {
            if (e.TaskResult == TaskResult.OK)
            {
                var bmp = new BitmapImage();
                bmp.SetSource(e.ChosenPhoto);
                ImagePickerImage.Source = bmp;
            }
        }

        private void CameraAccess_Click(object sender, RoutedEventArgs e)
        {
            _cameraCaptureTask.Show();
        }
    }
}