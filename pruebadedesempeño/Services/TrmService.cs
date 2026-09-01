
using System.Text.Json;
using pruebadedesempeño.Models;

namespace pruebadedesempeño.Services
{
    public class TrmService
    {
        private static readonly HttpClient HttpClient = new HttpClient();
        private const string ApiUrl = "https://www.datos.gov.co/resource/32sa-8pi3.json?$order=vigenciadesde%20DESC&$limit=1";

        public async Task<TrmRate> GetCurrentTrmAsync()
        {
            try
            {
                var response = await HttpClient.GetAsync(ApiUrl);

                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }

                string json = await response.Content.ReadAsStringAsync();
                var rates = JsonSerializer.Deserialize<List<TrmRate>>(json);

                if (rates != null && rates.Count > 0)
                {
                    return rates[0];
                }

                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
