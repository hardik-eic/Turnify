using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

public class GooglePlacesService : IGooglePlacesService
{
    private readonly string _apiKey;
    private readonly HttpClient _httpClient;

    public GooglePlacesService(string apiKey)
    {
        _apiKey = apiKey;
        _httpClient = new HttpClient();
    }

    public async Task<List<string>> GetPlaceSuggestionsAsync(string input)
    {
        try
        {
            var url = $"https://maps.googleapis.com/maps/api/place/autocomplete/json?input={input}&key={_apiKey}";
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                return new List<string>();

            var content = await response.Content.ReadAsStringAsync();
            var json = JsonDocument.Parse(content);
            var suggestions = new List<string>();

            foreach (var prediction in json.RootElement.GetProperty("predictions").EnumerateArray())
            {
                if (prediction.TryGetProperty("description", out var description))
                    suggestions.Add(description.GetString());
            }

            return suggestions;
        }
        catch
        {
            // Handle exceptions or logging as needed
            return new List<string>();
        }
    }

    public async Task<RouteInfo> GetRouteAsync(string origin, string destination)
    {
        try
        {
            var url = $"https://maps.googleapis.com/maps/api/directions/json?origin={origin}&destination={destination}&key={_apiKey}";
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                return null;

            var content = await response.Content.ReadAsStringAsync();
            var json = JsonDocument.Parse(content);
            var routeInfo = new RouteInfo();

            var route = json.RootElement.GetProperty("routes")[0];
            var leg = route.GetProperty("legs")[0];

            routeInfo.DistanceText = leg.GetProperty("distance").GetProperty("text").GetString();
            routeInfo.DurationText = leg.GetProperty("duration").GetProperty("text").GetString();
            routeInfo.PolylinePoints = route.GetProperty("overview_polyline").GetProperty("points").GetString();

            return routeInfo;
        }
        catch
        {
            // Handle exceptions or logging as needed
            return null;
        }
    }
}

public class RouteInfo
{
    public string DistanceText { get; set; }
    public string DurationText { get; set; }
    public string PolylinePoints { get; set; }

}
