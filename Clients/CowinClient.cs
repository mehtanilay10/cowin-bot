using System;
using System.Net.Http;
using System.Threading.Tasks;
using FindVaccineCenterBot.Helpers;
using FindVaccineCenterBot.Models.ApiResponse;
using Newtonsoft.Json;

namespace FindVaccineCenterBot.Clients
{
    public sealed class CowinClient : IDisposable
    {
        private readonly HttpClient _client;

        public CowinClient()
        {
            _client = HttpClientFactory.Create();
        }

        public void Dispose() => _client.Dispose();

        public async Task<CentersResponse> FindByZipcode(string zipcode)
        {
            var response = await _client.GetAsync($"calendarByPin?pincode={zipcode}&date={DateTime.Now.ToString("dd-MM-yyyy")}");
            var responseContent = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<CentersResponse>(responseContent);
            }
            else
            {
                var exception = $"{response.StatusCode} - ${response.Content}";
                throw new Exception(exception);
            }
        }

        public async Task<CentersResponse> FindByDistrict(int districtId)
        {
            var response = await _client.GetAsync($"calendarByDistrict?district_id={districtId}&date={DateTime.Now.ToString("dd-MM-yyyy")}");
            var responseContent = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<CentersResponse>(responseContent);
            }
            else
            {
                var exception = $"{response.StatusCode} - ${response.Content}";
                throw new Exception(exception);
            }
        }
    }
}
