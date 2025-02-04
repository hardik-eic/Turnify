using Microsoft.Extensions.Logging;
using Turnify.UI.Controls;
// using Turnify.Platforms.Android;
// using Turnify.Platforms.iOS;

namespace Turnify.UI;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.UseMauiMaps()
			.ConfigureMauiHandlers(handlers =>
			{
				// handlers.AddHandler(typeof(CustomPin), typeof(CustomPinHandler)); // Register custom pin handler
			})
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
