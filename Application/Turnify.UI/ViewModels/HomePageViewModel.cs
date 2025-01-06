using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls.Maps;
namespace Turnify.UI.ViewModels
{
    public class HomePageViewModel : BaseViewModel
    {
        private readonly GooglePlacesService _placesService;

        public ObservableCollection<string> PickupSuggestions { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<string> DropOffSuggestions { get; set; } = new ObservableCollection<string>();


        private string _pickupLocation = String.Empty;
        public string PickupLocation
        {
            get => _pickupLocation;
            set => SetProperty(ref _pickupLocation, value);
        }

        private string _dropOffLocation = String.Empty;
        public string DropOffLocation
        {
            get => _dropOffLocation;
            set => SetProperty(ref _dropOffLocation, value);
        }

        private string _selectedPickupLocation = String.Empty;
        public string SelectedPickupLocation
        {
            get => _selectedPickupLocation;
            set
            {
                if (SetProperty(ref _selectedPickupLocation, value))
                {
                    PickupLocation = value; // Update text field
                }
            }
        }

        private string _selectedDropOffLocation = String.Empty;
        public string SelectedDropOffLocation
        {
            get => _selectedDropOffLocation;
            set
            {
                if (SetProperty(ref _selectedDropOffLocation, value))
                {
                    DropOffLocation = value; // Update text field
                }
            }
        }

        public ICommand ShowRouteCommand { get; }
        public ICommand NavigateCommand { get; }

        public HomePageViewModel()
        {
            _placesService = new GooglePlacesService(apiKey: "AIzaSyAY4IHCAWSUYwNx-igDkMzcfFqaZ2Bofok");
            ShowRouteCommand = new Command(async () => await ShowRouteAsync());
            NavigateCommand = new Command(async () => await NavigateAsync());
            Distance = "--";
            TimeToReach = "--";
        }

        private async Task NavigateAsync()
        {
            // throw new NotImplementedException();
        }

        public async Task GetPickupSuggestionsAsync(string query)
        {
            PickupSuggestions.Clear();
            var suggestions = await _placesService.GetPlaceSuggestionsAsync(query);
            foreach (var suggestion in suggestions)
            {
                PickupSuggestions.Add(suggestion);
            }
        }

        public async Task GetDropOffSuggestionsAsync(string query)
        {
            DropOffSuggestions.Clear();
            var suggestions = await _placesService.GetPlaceSuggestionsAsync(query);
            foreach (var suggestion in suggestions)
            {
                DropOffSuggestions.Add(suggestion);
            }
        }

        public ObservableCollection<string>? Suggestions { get; }

        private Location? _deviceLocation;
        public Location? DeviceLocation
        {
            get => _deviceLocation;
            set => SetProperty(ref _deviceLocation, value);
        }

        private string _distance;
        private string _timeToReach;
        private bool _isDistanceAndTimeAvailable;

        public string Distance
        {
            get => _distance;
            set
            {
                SetProperty(ref _distance, value); // Using SetProperty from BaseViewModel
                UpdateIsDistanceAndTimeAvailable();
            }
        }

        public string TimeToReach
        {
            get => _timeToReach;
            set
            {
                SetProperty(ref _timeToReach, value); // Using SetProperty from BaseViewModel
                UpdateIsDistanceAndTimeAvailable();
            }
        }

        public bool IsDistanceAndTimeAvailable
        {
            get => _isDistanceAndTimeAvailable;
            set => SetProperty(ref _isDistanceAndTimeAvailable, value); // Using SetProperty from BaseViewModel
        }

        private void UpdateIsDistanceAndTimeAvailable()
        {
            IsDistanceAndTimeAvailable = !string.IsNullOrEmpty(Distance) && !string.IsNullOrEmpty(TimeToReach);
        }

        public ObservableCollection<Location> RoutePoints { get; private set; }

        private async Task GetSuggestionsAsync(string input)
        {
            var suggestions = await _placesService.GetPlaceSuggestionsAsync(input);
            Suggestions!.Clear();
            foreach (var suggestion in suggestions)
                Suggestions.Add(suggestion);
        }

        public async Task ShowRouteAsync()
        {
            if (string.IsNullOrEmpty(PickupLocation) || string.IsNullOrEmpty(DropOffLocation))
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Please enter both pickup and drop-off locations.", "OK");
                return;
            }

            // Get the route details from Google Places Service
            var route = await _placesService.GetRouteAsync(PickupLocation, DropOffLocation);
            if (route == null || route.RoutePoints == null || !route.RoutePoints.Any())
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Unable to calculate route. Please try again.", "OK");
                return;
            }

            // Update Distance and Duration
            Distance = route.DistanceText;
            TimeToReach = route.DurationText;
            OnPropertyChanged(nameof(Distance));
            OnPropertyChanged(nameof(TimeToReach));

            // Notify view with route points (optional: trigger event or binding for Map)
            RoutePoints = new ObservableCollection<Location>(
                route.RoutePoints.Select(point => new Location(point.Latitude, point.Longitude))
            );
            OnPropertyChanged(nameof(RoutePoints));
        }
    }
}