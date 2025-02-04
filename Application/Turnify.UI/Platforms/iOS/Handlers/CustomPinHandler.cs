// using MapKit;
// using Microsoft.Maui.Maps;
// using Microsoft.Maui.Maps.Handlers;
// using Turnify.UI.Controls;
// using UIKit;

// namespace Turnify.Platforms.iOS
// {
//     public class CustomPinHandler : MapPinHandler
//     {
//         // This method will be triggered when a pin is added to the map
//         public override void SetVirtualView(IMapPin mapPin)
//         {
//             base.SetVirtualView(mapPin);

//             if (mapPin is CustomPin customPin)
//             {
//                 var mapView = (MKMapView)PlatformView;

//                 var annotation = new MKPointAnnotation
//                 {
//                     Title = customPin.Label,
//                     Coordinate = new CoreLocation.CLLocationCoordinate2D(customPin.Location.Latitude, customPin.Location.Longitude)
//                 };

//                 mapView.AddAnnotation(annotation);

//                 // Handle custom pin image for iOS
//                 if (!string.IsNullOrEmpty(customPin.PinImage))
//                 {
//                     var annotationView = new MKAnnotationView(annotation, null)
//                     {
//                         Image = UIImage.FromBundle(customPin.PinImage),
//                         CanShowCallout = true
//                     };

//                     mapView.AddAnnotation(annotationView.Annotation);
//                 }
//             }
//         }
//     }
// }
