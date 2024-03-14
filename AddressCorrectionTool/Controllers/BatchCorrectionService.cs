using System.Text;
using AddressCorrectionTool.Models;
using Newtonsoft.Json;
using System.Net.Http;


namespace AddressCorrectionTool.Controllers
{

    public class BatchCorrectionService
    {
        private string baseUrl = "https://addressapi-api.azure-api.net/v1";

        private readonly HttpClient _httpClient;

        public BatchCorrectionService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task ProcessAddressesAsync(List<Address> addressList, Func<float, Task> progressCallback)
        {
            int totalAddresses = addressList.Count;
            int addressesProcessed = 0;

            float progress = 0;

            foreach (Address address in addressList)
            {
                // add api key to header
                _httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "1a393ebe5b4a49b28b163b73ad918d01");

                // serialise into json
                var json = JsonConvert.SerializeObject(address);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("https://addressapi-api.azure-api.net/v1/BatchInputAddresses", content);

                Console.WriteLine($"Processing address: {address.Number} {address.Street} {address.City} {address.Postcode}");

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    var correctedAddress = JsonConvert.DeserializeObject<InputAddress>(result);


                    Console.WriteLine($"Corrected address: {correctedAddress.Number} {correctedAddress.Street} {correctedAddress.City} {correctedAddress.Postcode}");

                }
                else
                {
                    Console.WriteLine($"Failed to process address: {response.StatusCode}");
                    break;
                }

                addressesProcessed++;
                progress = (float)addressesProcessed / totalAddresses;

                await progressCallback(progress);
            }
        }
    }
}