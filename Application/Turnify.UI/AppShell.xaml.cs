using Turnify.UI.Views;

namespace Turnify.UI;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
		Routing.RegisterRoute("HomePage", typeof(HomePage));
	}
}
