using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using CrouMetro.Core.Managers;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;

namespace CrouMetro.Views
{
    public partial class UserConfigurationPage : PhoneApplicationPage
    {
        public bool ImageChanged = false;

        public UserConfigurationPage()
        {
            InitializeComponent();
            ContentPanel.DataContext = App.userAccountEntity.GetUserEntity();
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            SaveButton.IsEnabled = false;
            //UserProgressBar.Visibility = System.Windows.Visibility.Visible;
            bool result =
                await
                    UserManager.ChangeUserProfile(NameBox.Text, URLBox.Text, DescriptionBox.Text, LocationBox.Text,
                        App.userAccountEntity);
            if (ImageChanged)
            {
                var bmp = new WriteableBitmap((BitmapSource) userProfileImage.Source);
                byte[] byteArray;
                using (var stream = new MemoryStream())
                {
                    bmp.SaveJpeg(stream, bmp.PixelWidth, bmp.PixelHeight, 0, 100);

                    byteArray = stream.ToArray();
                }
                bool result2 = await UserManager.ChangeUserProfileImage("test.jpg", byteArray, App.userAccountEntity);
            }
            await Auth.VerifyAccount(App.userAccountEntity);
            NavigationService.Navigate(new Uri("/MainTimelinePivot.xaml", UriKind.Relative));
        }

        private void ChangeProfilePictureButton_Click(object sender, RoutedEventArgs e)
        {
            PhotoChooserTask photoChooserTask;
            photoChooserTask = new PhotoChooserTask();
            var bitmapimage = new BitmapImage();
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
                userProfileImage.Source = bmp;
                ImageChanged = true;
            }
        }
    }
}