using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Maps.Handlers;
using Turnify.UI.Controls;

namespace Turnify.Platforms.Android
{
    public class CustomPinHandler : MapPinHandler
    {
        protected override void ConnectHandler(Microsoft.Maui.Maps.IMapPin mapPin, Android.Gms.Maps.GoogleMap googleMap)
        {
            base.ConnectHandler(mapPin, googleMap);

            if (mapPin is CustomPin customPin)
            {
                var markerOptions = new MarkerOptions()
                    .SetPosition(new LatLng(customPin.Location.Latitude, customPin.Location.Longitude))
                    .SetTitle(customPin.Label);

                if (!string.IsNullOrEmpty(customPin.PinImage))
                {
                    // Use a custom image for the pin
                    var icon = BitmapDescriptorFactory.FromAsset(customPin.PinImage);
                    markerOptions.SetIcon(icon);
                }

                // Add the custom marker to the Google Map
                googleMap.AddMarker(markerOptions);
            }
        }
    }
}
