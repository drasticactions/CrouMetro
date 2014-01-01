﻿using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using CrouMetro.Core.Entity;
using Croumetro.Resources;

namespace CrouMetro.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private string _sampleProperty = "Sample Runtime Property Value";

        public MainViewModel()
        {
            Items = new ObservableCollection<ItemViewModel>();
        }

        /// <summary>
        ///     A collection for ItemViewModel objects.
        /// </summary>
        public ObservableCollection<ItemViewModel> Items { get; private set; }

        public PostEntity SelectedPost { get; set; }

        public UserEntity SelectedUser { get; set; }

        /// <summary>
        ///     Sample ViewModel property; this property is used in the view to display its value using a Binding
        /// </summary>
        /// <returns></returns>
        public string SampleProperty
        {
            get { return _sampleProperty; }
            set
            {
                if (value != _sampleProperty)
                {
                    _sampleProperty = value;
                    NotifyPropertyChanged("SampleProperty");
                }
            }
        }

        /// <summary>
        ///     Sample property that returns a localized string
        /// </summary>
        public string LocalizedSampleProperty
        {
            get { return AppResources.SampleProperty; }
        }

        public bool IsDataLoaded { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        ///     Creates and adds a few ItemViewModel objects into the Items collection.
        /// </summary>
        public void LoadData()
        {
            // Sample data; replace with real data
            Items.Add(new ItemViewModel
            {
                LineOne = "runtime one",
                LineTwo = "Maecenas praesent accumsan bibendum",
                LineThree =
                    "Facilisi faucibus habitant inceptos interdum lobortis nascetur pharetra placerat pulvinar sagittis senectus sociosqu"
            });
            Items.Add(new ItemViewModel
            {
                LineOne = "runtime two",
                LineTwo = "Dictumst eleifend facilisi faucibus",
                LineThree =
                    "Suscipit torquent ultrices vehicula volutpat maecenas praesent accumsan bibendum dictumst eleifend facilisi faucibus"
            });
            Items.Add(new ItemViewModel
            {
                LineOne = "runtime three",
                LineTwo = "Habitant inceptos interdum lobortis",
                LineThree =
                    "Habitant inceptos interdum lobortis nascetur pharetra placerat pulvinar sagittis senectus sociosqu suscipit torquent"
            });
            Items.Add(new ItemViewModel
            {
                LineOne = "runtime four",
                LineTwo = "Nascetur pharetra placerat pulvinar",
                LineThree =
                    "Ultrices vehicula volutpat maecenas praesent accumsan bibendum dictumst eleifend facilisi faucibus habitant inceptos"
            });
            Items.Add(new ItemViewModel
            {
                LineOne = "runtime five",
                LineTwo = "Maecenas praesent accumsan bibendum",
                LineThree =
                    "Maecenas praesent accumsan bibendum dictumst eleifend facilisi faucibus habitant inceptos interdum lobortis nascetur"
            });
            Items.Add(new ItemViewModel
            {
                LineOne = "runtime six",
                LineTwo = "Dictumst eleifend facilisi faucibus",
                LineThree =
                    "Pharetra placerat pulvinar sagittis senectus sociosqu suscipit torquent ultrices vehicula volutpat maecenas praesent"
            });
            Items.Add(new ItemViewModel
            {
                LineOne = "runtime seven",
                LineTwo = "Habitant inceptos interdum lobortis",
                LineThree =
                    "Accumsan bibendum dictumst eleifend facilisi faucibus habitant inceptos interdum lobortis nascetur pharetra placerat"
            });
            Items.Add(new ItemViewModel
            {
                LineOne = "runtime eight",
                LineTwo = "Nascetur pharetra placerat pulvinar",
                LineThree =
                    "Pulvinar sagittis senectus sociosqu suscipit torquent ultrices vehicula volutpat maecenas praesent accumsan bibendum"
            });
            Items.Add(new ItemViewModel
            {
                LineOne = "runtime nine",
                LineTwo = "Maecenas praesent accumsan bibendum",
                LineThree =
                    "Facilisi faucibus habitant inceptos interdum lobortis nascetur pharetra placerat pulvinar sagittis senectus sociosqu"
            });
            Items.Add(new ItemViewModel
            {
                LineOne = "runtime ten",
                LineTwo = "Dictumst eleifend facilisi faucibus",
                LineThree =
                    "Suscipit torquent ultrices vehicula volutpat maecenas praesent accumsan bibendum dictumst eleifend facilisi faucibus"
            });
            Items.Add(new ItemViewModel
            {
                LineOne = "runtime eleven",
                LineTwo = "Habitant inceptos interdum lobortis",
                LineThree =
                    "Habitant inceptos interdum lobortis nascetur pharetra placerat pulvinar sagittis senectus sociosqu suscipit torquent"
            });
            Items.Add(new ItemViewModel
            {
                LineOne = "runtime twelve",
                LineTwo = "Nascetur pharetra placerat pulvinar",
                LineThree =
                    "Ultrices vehicula volutpat maecenas praesent accumsan bibendum dictumst eleifend facilisi faucibus habitant inceptos"
            });
            Items.Add(new ItemViewModel
            {
                LineOne = "runtime thirteen",
                LineTwo = "Maecenas praesent accumsan bibendum",
                LineThree =
                    "Maecenas praesent accumsan bibendum dictumst eleifend facilisi faucibus habitant inceptos interdum lobortis nascetur"
            });
            Items.Add(new ItemViewModel
            {
                LineOne = "runtime fourteen",
                LineTwo = "Dictumst eleifend facilisi faucibus",
                LineThree =
                    "Pharetra placerat pulvinar sagittis senectus sociosqu suscipit torquent ultrices vehicula volutpat maecenas praesent"
            });
            Items.Add(new ItemViewModel
            {
                LineOne = "runtime fifteen",
                LineTwo = "Habitant inceptos interdum lobortis",
                LineThree =
                    "Accumsan bibendum dictumst eleifend facilisi faucibus habitant inceptos interdum lobortis nascetur pharetra placerat"
            });
            Items.Add(new ItemViewModel
            {
                LineOne = "runtime sixteen",
                LineTwo = "Nascetur pharetra placerat pulvinar",
                LineThree =
                    "Pulvinar sagittis senectus sociosqu suscipit torquent ultrices vehicula volutpat maecenas praesent accumsan bibendum"
            });

            IsDataLoaded = true;
        }

        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}