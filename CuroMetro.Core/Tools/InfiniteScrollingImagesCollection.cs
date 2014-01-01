using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using CrouMetro.Core.Entity;
using CrouMetro.Core.Managers;

namespace CrouMetro.Core.Tools
{
    public class InfiniteScrollingHtmlParseImageCollection : INotifyPropertyChanged
    {
        public long? MaxStatusId;
        public int Offset;
        private bool _isLoading;
        private int _page;

        public InfiniteScrollingHtmlParseImageCollection()
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
        public ObservableCollection<MediaEntity> MediaCollection { get; set; }

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

        public async void LoadAlbum()
        {
            IsLoading = true;
            var items = new List<MediaEntity>();
            items = await AlbumManager.GetAlbumList(Offset, userName, userAccountEntity);

            foreach (MediaEntity item in items)
            {
                MediaCollection.Add(item);
            }
            if (items.Any())
            {
                HasMoreItems = true;
                Offset += 20;
            }
            else
            {
                HasMoreItems = false;
            }
            IsLoading = false;
        }
    }
}