using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using CrouMetro.Core.Entity;
using CrouMetro.Core.Managers;

namespace CrouMetro.Core.Tools
{
    public class InfiniteScrollingSecretMailCollection : INotifyPropertyChanged
    {
        public long? MaxStatusId;
        private bool _isLoading;
        private bool _isLoadingData = false;
        private int _page;

        public InfiniteScrollingSecretMailCollection()
        {
            HasMoreItems = true;
            IsLoading = false;
            _page = 0;
        }

        public bool HasMoreItems { get; protected set; }

        public string timeline { get; set; }
        public string userName { get; set; }
        public long UserId { get; set; }
        public UserAccountEntity userAccountEntity { get; set; }

        public ObservableCollection<SecretMailEntity> PostCollection { get; set; }

        public bool IsLoading
        {
            get { return _isLoading; }

            private set
            {
                _isLoading = value;
                NotifyPropertyChanged("IsLoading");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public async void LoadPosts(string timeline)
        {
            IsLoading = true;
            var items = new List<SecretMailEntity>();
            items = await SecretMailManager.GetSecretMails(null, MaxStatusId, null, userAccountEntity);

            foreach (SecretMailEntity item in items)
            {
                PostCollection.Add(item);
            }
            if (items.Any())
            {
                HasMoreItems = true;
                MaxStatusId = items.Last().ID;
            }
            else
            {
                HasMoreItems = false;
            }
            IsLoading = false;
        }
    }
}