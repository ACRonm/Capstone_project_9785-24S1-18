using System.Text;
using AddressCorrectionTool.Models;
using Newtonsoft.Json;
using System.Net.Http;


namespace AddressCorrectionTool.Services
{

    public class BatchCorrectionService
    {
        private string? baseUrl;
        private string? apiKey;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public BatchCorrectionService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task ProcessAddressesAsync(List<Address> addressList, Func<float, Task> progressCallback)
        {
            baseUrl = _configuration["ApiUrl"];
            apiKey = _configuration["ApiKey"];

            int totalAddresses = addressList.Count;
            int addressesProcessed = 0;
            float progress;

            _httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "1a393ebe5b4a49b28b163b73ad918d01");

            foreach (Address address in addressList)
            {
                var response = await _httpClient.PostAsJsonAsync($"{baseUrl}/BatchInputAddresses", address);

                response.EnsureSuccessStatusCode();

                var correctedAddress = await response.Content.ReadFromJsonAsync<InputAddress>();

                addressesProcessed++;
                progress = (float)addressesProcessed / totalAddresses;

                await progressCallback(progress);
            }
        }
    }
}