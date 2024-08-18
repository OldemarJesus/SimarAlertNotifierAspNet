using SimarAlertNotifier.Models;
using System.Net.Http.Headers;
using System.Text.Json;

namespace SimarAlertNotifier.Services
{
    public class SimarAlertService
    {
        // holds http client instance
        private static HttpClient _httpClient = new HttpClient();

        // instantiate http client to be used with simar endpoint api
        public SimarAlertService()
        {
            
            string? simarApiUri = Environment.GetEnvironmentVariable("SIMAR_ALERT_API_URL");

            if (simarApiUri is null)
            {
                throw new InvalidDataException("SimarAlertApiUrl configuration is missing");
            }

            _httpClient.BaseAddress = new Uri(simarApiUri);
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json")
            );
        }

        // get all available alerts in simar
        public async Task<List<Alert>> GetAlertsAsync()
        {
            // empty alert list
            List<Alert>? alerts = new List<Alert>();
            HttpResponseMessage response = await _httpClient.GetAsync("/alerts");

            // case we succeed get request
            if (response.IsSuccessStatusCode)
            {
                // raw json result
                string jsonStr = await response.Content.ReadAsStringAsync();

                // list of strings with alerts Ex: ["alert 1"]
                var alertsStr = JsonSerializer.Deserialize<List<string>>(jsonStr, new JsonSerializerOptions{});

                // return empty list if no data retrieved
                if (alertsStr is null) return new List<Alert>();

                // convert alerts string to list of object Alert
                foreach(string alert in alertsStr)
                {
                    alerts.Add(new Alert { message = alert });
                }
            }

            return alerts;
        }
    }
}
