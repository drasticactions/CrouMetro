using System;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;

namespace CrouMetro.Views
{
    public partial class MessagePost : PhoneApplicationPage
    {
        public MessagePost()
        {
            InitializeComponent();
        }

        public static long? ReplyStatusId { get; set; }

        public static long? UserId { get; set; }

        public static bool IsSecretMail { get; set; }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            string msg = string.Empty;
            IsSecretMail = false;
            if (NavigationContext.QueryString.TryGetValue("screenName", out msg))
                MessageBoxUserControl.StatusUpdateBox.Text = msg;

            if (NavigationContext.QueryString.TryGetValue("statusId", out msg))
                ReplyStatusId = Convert.ToInt64(Convert.ToDecimal(msg));

            if (NavigationContext.QueryString.TryGetValue("userId", out msg))
                UserId = Convert.ToInt64(Convert.ToDecimal(msg));

            if (NavigationContext.QueryString.TryGetValue("IsSecretMail", out msg))
                IsSecretMail = msg.ToLower().Equals("true");
        }

        private void NowPlayingButton_Click(object sender, EventArgs e)
        {
            if (MediaPlayer.Queue.ActiveSong != null)
            {
                string name = MediaPlayer.Queue.ActiveSong.Name;
                string artist = MediaPlayer.Queue.ActiveSong.Artist.Name;
                string album = MediaPlayer.Queue.ActiveSong.Album.Name;
                if (MediaPlayer.Queue.ActiveSong.Album.HasArt)
                {
                    var bmp = new BitmapImage();
                    bmp.SetSource(MediaPlayer.Queue.ActiveSong.Album.GetAlbumArt());
                    MessageBoxUserControl.ImagePickerImage.Source = bmp;
                }
                MessageBoxUserControl.StatusUpdateBox.Text = string.Format("#nowplaying {0} - {1} ({2})", artist, name,
                    album);
                FrameworkDispatcher.Update();
            }
        }
    }
}