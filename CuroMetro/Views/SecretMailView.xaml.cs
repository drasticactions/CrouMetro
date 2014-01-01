using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Navigation;
using CrouMetro.Core.Entity;
using CrouMetro.Core.Managers;
using CrouMetro.Core.Tools;
using Microsoft.Phone.Controls;

namespace CrouMetro.Views
{
    public partial class SecretMailView : PhoneApplicationPage
    {
        private int _offsetKnob = 15;

        public SecretMailView()
        {
            InitializeComponent();
        }

        public static InfiniteScrollingSecretMailCollection secretMailCollection { get; set; }

        public async Task<bool> BindToSecretMails()
        {
            secretMailCollection = new InfiniteScrollingSecretMailCollection();
            secretMailCollection.timeline = EndPoints.MESSAGE_MAILS;
            secretMailCollection.userAccountEntity = App.userAccountEntity;
            secretMailCollection.PostCollection = new ObservableCollection<SecretMailEntity>();
            List<SecretMailEntity> items =
                await SecretMailManager.GetSecretMails(null, null, null, App.userAccountEntity);
            foreach (SecretMailEntity item in items)
            {
                secretMailCollection.PostCollection.Add(item);
            }
            secretMailCollection.MaxStatusId = items.Last().ID;
            secretEmailTimeLine.DataContext = secretMailCollection;
            secretEmailTimeLine.ItemRealized += secretMailTimeLine_ItemRealized;
            return true;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            BindToSecretMails();
        }

        private void secretMailTimeLine_ItemRealized(object sender, ItemRealizationEventArgs e)
        {
            if (!secretMailCollection.IsLoading && secretEmailTimeLine.ItemsSource != null &&
                secretEmailTimeLine.ItemsSource.Count >= _offsetKnob)
            {
                if (e.ItemKind == LongListSelectorItemKind.Item)
                {
                    if (
                        (e.Container.Content as PostEntity).Equals(
                            secretEmailTimeLine.ItemsSource[secretEmailTimeLine.ItemsSource.Count - _offsetKnob]))
                    {
                        secretMailCollection.LoadPosts(EndPoints.MENTIONS_TIMELINE);
                    }
                }
            }
        }
    }
}