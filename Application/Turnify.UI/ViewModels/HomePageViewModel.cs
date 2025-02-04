using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls.Maps;
using Turnify.UI.Models;
using Turnify.UI.Views;
namespace Turnify.UI.ViewModels
{
    public class HomePageViewModel : BaseViewModel
    {
        private readonly GooglePlacesService _placesService;
        public ObservableCollection<string> PickupSuggestions { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<string> DropOffSuggestions { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<string>? Suggestions { get; }
        public ObservableCollection<Location> RoutePoints { get; private set; }
        public ICommand ShowRouteCommand { get; }
        public ICommand NavigateCommand { get; }
        private readonly INavigation _navigation;
        private CancellationTokenSource _simulationCancellationTokenSource;
        public ObservableCollection<Segment> Segments { get; set; }

        public ObservableCollection<string> VehicleModes { get; } = new ObservableCollection<string>
        {
            "transit",
            "driving",
            "two_wheeler",
            "bicycling",
            "walking",
        };

        private string _pickupLocation = String.Empty;
        public string PickupLocation
        {
            get => _pickupLocation;
            set
            {
                if (_pickupLocation != value)
                {
                    _pickupLocation = value;
                    OnPropertyChanged();
                    CheckButtonState();
                }
            }
        }

        private string _dropOffLocation = String.Empty;
        public string DropOffLocation
        {
            get => _dropOffLocation;
            set
            {
                if (_dropOffLocation != value)
                {
                    _dropOffLocation = value;
                    OnPropertyChanged();
                    CheckButtonState();
                }
            }
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

        private bool _shouldNavigate = false;
        public bool ShouldNavigate
        {
            get => _shouldNavigate;
            private set
            {
                if (_shouldNavigate != value)
                {
                    _shouldNavigate = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _distance;
        public string Distance
        {
            get => _distance;
            set
            {
                SetProperty(ref _distance, value); // Using SetProperty from BaseViewModel
                UpdateIsDistanceAndTimeAvailable();
            }
        }

        private string _timeToReach;
        public string TimeToReach
        {
            get => _timeToReach;
            set
            {
                SetProperty(ref _timeToReach, value); // Using SetProperty from BaseViewModel
                UpdateIsDistanceAndTimeAvailable();
            }
        }

        private bool _isDistanceAndTimeAvailable;
        public bool IsDistanceAndTimeAvailable
        {
            get => _isDistanceAndTimeAvailable;
            set => SetProperty(ref _isDistanceAndTimeAvailable, value); // Using SetProperty from BaseViewModel
        }

        private Location? _deviceLocation;
        public Location? DeviceLocation
        {
            get => _deviceLocation;
            set => SetProperty(ref _deviceLocation, value);
        }

        private Location? _simulatedUserLocation;
        public Location? SimulatedUserLocation
        {
            get => _simulatedUserLocation;
            set => SetProperty(ref _simulatedUserLocation, value);
        }
        private bool _isSimulating;
        public bool IsSimulating
        {
            get => _isSimulating;
            set
            {
                if (SetProperty(ref _isSimulating, value))
                {
                    OnPropertyChanged(nameof(SimulationButtonText));
                }
            }
        }
        private int _selectedSegmentIndex;
        public int SelectedSegmentIndex
        {
            get => _selectedSegmentIndex;
            set
            {
                _selectedSegmentIndex = value;
                SelectedVehicleMode = VehicleModes[_selectedSegmentIndex];
                OnPropertyChanged(nameof(SelectedVehicleMode));
                OnPropertyChanged(nameof(SelectedSegmentIndex));
            }
        }

        private string _selectedVehicleMode;
        public string SelectedVehicleMode
        {
            get => _selectedVehicleMode;
            set
            {
                if (SetProperty(ref _selectedVehicleMode, value))
                {
                    UpdateDistanceAndTimeAsync();
                }
            }
        }
        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public string SimulationButtonText => IsSimulating ? "Stop" : "Start";

        private void CheckButtonState()
        {
            ShouldNavigate = !string.IsNullOrEmpty(PickupLocation) && !string.IsNullOrEmpty(DropOffLocation);
        }

        private void UpdateIsDistanceAndTimeAvailable()
        {
            IsDistanceAndTimeAvailable = !string.IsNullOrEmpty(Distance) && !string.IsNullOrEmpty(TimeToReach);
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

            // Calculate the route details based on selected vehicle type
            await UpdateDistanceAndTimeAsync();

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

        public async Task SimulateUserMovementAsync()
        {
            if (IsSimulating)
            {
                // Stop simulation
                _simulationCancellationTokenSource?.Cancel();
                IsSimulating = false;
                return;
            }

            // Start simulation
            _simulationCancellationTokenSource = new CancellationTokenSource();
            IsSimulating = true;

            try
            {
                if (RoutePoints == null || RoutePoints.Count == 0)
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "No route available to simulate.", "OK");
                    return;
                }

                foreach (var point in RoutePoints)
                {
                    if (_simulationCancellationTokenSource.Token.IsCancellationRequested)
                        break;

                    SimulatedUserLocation = point; // Update the user's simulated location
                    OnPropertyChanged(nameof(SimulatedUserLocation));
                    await Task.Delay(1000, _simulationCancellationTokenSource.Token); // Wait for 1 second before moving to the next point
                }

                if (!_simulationCancellationTokenSource.Token.IsCancellationRequested)
                {
                    await Application.Current.MainPage.DisplayAlert("Simulation", "User has reached the destination!", "OK");
                }
            }
            catch (TaskCanceledException)
            {
                // Simulation was canceled
                IsSimulating = false;
            }
            finally
            {
                IsSimulating = false;
            }
        }

        public async Task UpdateDistanceAndTimeAsync()
        {
            IsLoading = true;
            if (!string.IsNullOrEmpty(PickupLocation) && !string.IsNullOrEmpty(DropOffLocation))
            {
                var result = await _placesService.GetDistanceAndTimeAsync(PickupLocation, DropOffLocation, SelectedVehicleMode);
                Distance = result.DistanceText;
                TimeToReach = result.DurationText;
                OnPropertyChanged(nameof(Distance));
                OnPropertyChanged(nameof(TimeToReach));
                IsLoading = false;
            }
        }

        public HomePageViewModel(INavigation navigation)
        {
            _navigation = navigation;
            _placesService = new GooglePlacesService(apiKey: "${MAPS_API_KEY}");
            ShowRouteCommand = new Command(async () => await ShowRouteAsync());
            NavigateCommand = new Command(async () => await SimulateUserMovementAsync());
            Distance = "--";
            TimeToReach = "--";


            Segments = new ObservableCollection<Segment>
            {
                new Segment { Text = "Transit", ImageSource = "navigation.png" },
                new Segment { Text = "Driving", ImageSource = "drive.png" },
                new Segment { Text = "Bicycling", ImageSource = "bycycle.png" },
                new Segment { Text = "Walking", ImageSource = "walk.png" },
                new Segment { Text = "Cycling", ImageSource = "cycle.png" }
            };
        }

    }
}