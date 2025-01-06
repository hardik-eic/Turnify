using Microsoft.Maui.Maps;
using Microsoft.Maui.Controls.Maps;
using Turnify.UI.ViewModels;
using System.ComponentModel;

namespace Turnify.UI.Views;

public partial class HomePage : ContentPage
{
    private readonly HomePageViewModel _viewModel;

    public HomePage()
    {
        InitializeComponent();
        _viewModel = new HomePageViewModel();
        BindingContext = _viewModel;

        _viewModel.PropertyChanged += ViewModel_RoutePointsChanged;
    }

    private void ViewModel_RoutePointsChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(HomePageViewModel.RoutePoints))
        {
            UpdateMap();
        }
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        var geolocationRequest = new GeolocationRequest(GeolocationAccuracy.High, TimeSpan.FromSeconds(10));
        var location = await Geolocation.GetLocationAsync(geolocationRequest);

        MapView.MoveToRegion(MapSpan.FromCenterAndRadius(location, Distance.FromKilometers(2)));
    }

    private async void OnPickupLocationTextChanged(object sender, TextChangedEventArgs e)
    {
        if (e.NewTextValue.Length > 3)
        {
            var viewModel = BindingContext as HomePageViewModel;
            if (viewModel != null)
            {
                await viewModel.GetPickupSuggestionsAsync(e.NewTextValue);
            }
        }
    }

    private async void OnDropOffLocationTextChanged(object sender, TextChangedEventArgs e)
    {
        if (e.NewTextValue.Length > 3)
        {
            var viewModel = BindingContext as HomePageViewModel;
            if (viewModel != null)
            {
                await viewModel.GetDropOffSuggestionsAsync(e.NewTextValue);
            }
        }
    }

    private void UpdateMap()
    {
        if (_viewModel.RoutePoints != null && _viewModel.RoutePoints.Any())
        {
            // Clear existing map elements & pins
            MapView.MapElements.Clear();
            MapView.Pins.Clear();

            // Add polyline for the route
            var polyline = new Polyline
            {
                StrokeColor = Colors.Blue,
                StrokeWidth = 15
            };

            foreach (var point in _viewModel.RoutePoints)
            {
                polyline.Geopath.Add(point);
            }

            MapView.MapElements.Add(polyline);

            // Add pins for pickup and drop-off locations
            var pickupPin = new Pin
            {
                Label = "Pickup Location",
                Location = _viewModel.RoutePoints.First(),
                Type = PinType.Place
            };

            var dropOffPin = new Pin
            {
                Label = "Drop-Off Location",
                Location = _viewModel.RoutePoints.Last(),
                Type = PinType.Place
            };

            MapView.Pins.Add(pickupPin);
            MapView.Pins.Add(dropOffPin);

            // Adjust map to show the entire route
            var firstPoint = _viewModel.RoutePoints.First();
            var lastPoint = _viewModel.RoutePoints.Last();

            var mapSpan = MapSpan.FromCenterAndRadius(
                new Location(
                    (firstPoint.Latitude + lastPoint.Latitude) / 2,
                    (firstPoint.Longitude + lastPoint.Longitude) / 2),
                Distance.FromKilometers(5)
            );

            MapView.MoveToRegion(mapSpan);
        }
        else
        {
            DisplayAlert("Error", "Unable to display the route.", "OK");
        }
    }

    // Event handler for when the user clicks the "Start Navigate" button
    private async void OnStartNavigateClicked(object sender, EventArgs e)
    {
        // Navigate to the next page (for example, a "NavigationPage" or "RoutePage")
        // await Shell.Current.GoToAsync("RoutePage");
    }
}
