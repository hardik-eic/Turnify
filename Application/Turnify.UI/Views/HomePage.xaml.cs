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
		// await _viewModel.ShowRouteCommand.ExecuteAsync(null);
	}

	// Event handler for when the user clicks the "Start Navigate" button
	private async void OnStartNavigateClicked(object sender, EventArgs e)
	{
		// Navigate to the next page (for example, a "NavigationPage" or "RoutePage")
		// await Shell.Current.GoToAsync("RoutePage");
	}
}
