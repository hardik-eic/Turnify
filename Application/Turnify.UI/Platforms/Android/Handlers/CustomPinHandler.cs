// using Android.Gms.Maps;
// using Android.Gms.Maps.Model;
// using Microsoft.Maui.Maps;
// using Microsoft.Maui.Maps.Handlers;
// using Turnify.UI.Controls;
// using Microsoft.Maui.Handlers;

// namespace Turnify.Platforms.Android
// {
//     public class CustomPinHandler : MapPinHandler
//     {
//         // This method will be triggered when a pin is added to the map
//         public override void SetVirtualView(IMapPin mapPin)
//         {
//             base.SetVirtualView(mapPin);

//             if (mapPin is CustomPin customPin)
//             {
//                 var map = (GoogleMap)PlatformView;

//                 // Create a MarkerOptions object to configure the pin
//                 var markerOptions = new MarkerOptions()
//                     .SetPosition(new LatLng(customPin.Location.Latitude, customPin.Location.Longitude))
//                     .SetTitle(customPin.Label);

//                 // Use custom image for the pin if provided
//                 if (!string.IsNullOrEmpty(customPin.PinImage))
//                 {
//                     var icon = BitmapDescriptorFactory.FromAsset(customPin.PinImage);
//                     markerOptions.SetIcon(icon);
//                 }

//                 // Add the marker to the map
//                 map.AddMarker(markerOptions);  // Correct usage of AddMarker
//             }
//         }
//     }
// }
