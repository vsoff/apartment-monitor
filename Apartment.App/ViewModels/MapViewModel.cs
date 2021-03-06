﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Input;
using Apartment.App.Common;
using Apartment.App.Models;
using Apartment.App.Views;
using Apartment.Common.Models;
using Apartment.Options;
using GMap.NET;

namespace Apartment.App.ViewModels
{
    public class MapViewModel : ViewModelBase
    {
        public PointLatLng StartPosition { get; }
        public bool IsRegionEditingMode { get; set; }

        public ObservableCollection<ApartmentsGroup> Apartments { get; }
        public ObservableCollection<Region> Regions { get; }

        public EventHandler<object> ItemsAdded;
        public EventHandler<object> ItemsRemoved;
        public EventHandler<IEnumerable<PointLatLng>> RegionCreated;
        public EventHandler<PointLatLng> CurrentPositionChanged;

        public ICommand AddNewRegionPointCommand { get; }
        public ICommand CreateRegionCommand { get; }
        public ICommand OpenMarkerInfoCommand { get; }

        public MapViewModel(ApplicationOptions options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));
            AddNewRegionPointCommand = new RelayCommand(x => AddNewRegionPoint((PointLatLng) x), x => IsRegionEditingMode);
            CreateRegionCommand = new RelayCommand(x => FlushNewRegion(), x => IsRegionEditingMode);
            // TODO: Убрать нарушение MVVM.
            OpenMarkerInfoCommand = new RelayCommand(x => new ApartmentsGroupWindow(x as ApartmentsGroup).ShowDialog());
            IsRegionEditingMode = false;

            _newRegionData = null;
            _newRegionPointsData = new List<NewRegionPoint>();

            var apartments = new FixedObservableCollection<ApartmentsGroup>();
            apartments.CollectionChanged += DrawableCollectionChanged;
            apartments.Clearing += CollectionClearing;
            Apartments = apartments;

            var regions = new FixedObservableCollection<Region>();
            regions.CollectionChanged += DrawableCollectionChanged;
            regions.Clearing += CollectionClearing;
            Regions = regions;
            StartPosition = new PointLatLng(options.StartPosition.Latitude, options.StartPosition.Longitude);
        }

        private void CollectionClearing(object sender, EventArgs e) => ItemsRemoved?.Invoke(this, sender);

        private void DrawableCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems?.Count > 0)
            {
                var items = e.NewItems;
                ItemsAdded?.Invoke(this, items);
                return;
            }

            if (e.Action == NotifyCollectionChangedAction.Remove && e.OldItems?.Count > 0)
            {
                var items = e.OldItems;
                ItemsRemoved?.Invoke(this, items);
                return;
            }

            if (e.Action == NotifyCollectionChangedAction.Reset)
                return;

            throw new NotImplementedException("Такой кейс не проверялся");
        }

        #region New region editing

        private readonly List<NewRegionPoint> _newRegionPointsData;
        private NewRegionData _newRegionData;

        private void FlushNewRegion()
        {
            if (_newRegionData == null)
                return;

            var locations = _newRegionData.Locations;

            // Чистим карту
            ItemsRemoved?.Invoke(this, _newRegionData);
            ItemsRemoved?.Invoke(this, _newRegionPointsData);
            _newRegionData = null;
            _newRegionPointsData.Clear();

            // Добавляем новоиспечённый регион.
            if (locations.Count > 2)
                RegionCreated?.Invoke(this, locations);
        }

        private void AddNewRegionPoint(PointLatLng point)
        {
            List<PointLatLng> locations;
            if (_newRegionData == null)
            {
                locations = new List<PointLatLng> {point};
            }
            else
            {
                locations = new List<PointLatLng>(_newRegionData.Locations.Count);
                locations.AddRange(_newRegionData.Locations);
                locations.Add(point);
                ItemsRemoved?.Invoke(this, _newRegionData);
            }

            var newPoint = new NewRegionPoint(point);
            _newRegionPointsData.Add(newPoint);
            ItemsAdded?.Invoke(this, newPoint);

            _newRegionData = new NewRegionData(locations);
            ItemsAdded?.Invoke(this, _newRegionData);
        }

        #endregion

        /// <summary>
        /// Центрирует карту по указанным координатам.
        /// </summary>
        public void SetCurrentPosition(PointLatLng position) => CurrentPositionChanged?.Invoke(this, position);
    }
}