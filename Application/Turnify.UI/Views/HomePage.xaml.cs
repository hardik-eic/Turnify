using Microsoft.Maui.Maps;
using Microsoft.Maui.Controls.Maps;
using Turnify.UI.ViewModels;

namespace Turnify.UI.Views;

public partial class HomePage : ContentPage
{
    private readonly HomePageViewModel _viewModel;

    public HomePage()
    {
        InitializeComponent();
        _viewModel = new HomePageViewModel();
        BindingContext = _viewModel;
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

    // Event handler for when the user clicks the "Show Route" button
    private async void OnShowRouteClicked(object sender, EventArgs e)
    {
        var viewModel = BindingContext as HomePageViewModel;
        if (viewModel == null)
            return;

        await viewModel.ShowRouteAsync();

        if (viewModel.RoutePoints != null && viewModel.RoutePoints.Any())
        {
            try
            {
                // Clear existing map elements and pins
                MapView.MapElements.Clear();
                MapView.Pins.Clear();

                // Add a pin for the pickup location
                var pickupLocation = viewModel.RoutePoints.First();
                var pickupPin = new Pin
                {
                    Label = "Pickup Location",
                    Address = viewModel.PickupLocation,
                    Location = pickupLocation,
                    Type = PinType.Place
                };
                MapView.Pins.Add(pickupPin);

                // Add a pin for the drop-off location
                var dropOffLocation = viewModel.RoutePoints.Last();
                var dropOffPin = new Pin
                {
                    Label = "Drop-off Location",
                    Address = viewModel.DropOffLocation,
                    Location = dropOffLocation,
                    Type = PinType.Place
                };
                MapView.Pins.Add(dropOffPin);

                // Create and add polyline
                var polyline = new Polyline
                {
                    StrokeColor = Colors.Blue,
                    StrokeWidth = 5
                };

                foreach (var point in viewModel.RoutePoints)
                {
                    polyline.Geopath.Add(point);
                }

                MapView.MapElements.Add(polyline);

                // Adjust map to show the route
                var mapSpan = MapSpan.FromCenterAndRadius(
                    new Location(
                        (pickupLocation.Latitude + dropOffLocation.Latitude) / 2,
                        (pickupLocation.Longitude + dropOffLocation.Longitude) / 2),
                    Distance.FromKilometers(5)
                );

                MapView.MoveToRegion(mapSpan);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Unable to display the route: {ex.Message}", "OK");
            }
        }
        else
        {
            await DisplayAlert("Error", "Unable to display the route. No points found.", "OK");
        }
    }

    // Event handler for when the user clicks the "Start Navigate" button
    private async void OnStartNavigateClicked(object sender, EventArgs e)
    {
        // Navigate to the next page (for example, a "NavigationPage" or "RoutePage")
        // await Shell.Current.GoToAsync("RoutePage");
    }
}
