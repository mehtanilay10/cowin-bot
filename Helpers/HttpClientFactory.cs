using System;
using System.Net.Http;

namespace FindVaccineCenterBot.Helpers
{
    public static class HttpClientFactory
    {
        public static HttpClient Create()
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri("https://cdn-api.co-vin.in/api/v2/appointment/sessions/public/");
            client.DefaultRequestHeaders.ConnectionClose = false;
            client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/90.0.4430.93 Safari/537.36 Edg/90.0.818.51");
            System.Net.ServicePointManager.FindServicePoint(client.BaseAddress).ConnectionLeaseTimeout = 60 * 1000; //1 minute
            return client;
        }
    }
}
