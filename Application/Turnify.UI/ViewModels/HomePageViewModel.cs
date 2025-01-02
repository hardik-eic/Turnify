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

        private  string _selectedDropOffLocation = String.Empty;
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

        public string? Distance { get; private set; }
        public string? TimeToReach { get; private set; }

        private async Task GetSuggestionsAsync(string input)
        {
            var suggestions = await _placesService.GetPlaceSuggestionsAsync(input);
            Suggestions!.Clear();
            foreach (var suggestion in suggestions)
                Suggestions.Add(suggestion);
        }

        private async Task ShowRouteAsync()
        {
            // Use Google Maps API to calculate route, distance, and time
            var route = await _placesService.GetRouteAsync(PickupLocation, DropOffLocation);
            Distance = route.DistanceText;
            TimeToReach = route.DurationText;

            OnPropertyChanged(nameof(Distance));
            OnPropertyChanged(nameof(TimeToReach));

            // Show the route on the map (notify view through events or data bindings)
        }
    }
}