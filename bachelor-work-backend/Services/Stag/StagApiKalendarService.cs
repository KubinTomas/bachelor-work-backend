using bachelor_work_backend.Models.StagApiKalendar;
using bachelor_work_backend.Services.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace bachelor_work_backend.Services.Stag
{
    public class StagApiKalendarService
    {
        public IHttpClientFactory ClientFactory { get; private set; }

        private readonly string stagApiUrl;
        public StagApiKalendarService(string stagApiUrl, IHttpClientFactory clientFactory)
        {
            ClientFactory = clientFactory;
            this.stagApiUrl = stagApiUrl;
        }

        public async Task<StagAktualniObdobiInfo?> GetAktualniObdobiInfo(string wscookie)
        {
            NameValueCollection nvc = new NameValueCollection();
            nvc.Add("outputFormat", "JSON");

            var queryStringParams = UtilsService.ToQueryString(nvc);
            var request = new HttpRequestMessage(HttpMethod.Get, stagApiUrl + "/ws/services/rest2/kalendar/getAktualniObdobiInfo" + queryStringParams);
            request.Headers.Add("Cookie", "WSCOOKIE=" + wscookie + ";");

            using (var client = ClientFactory.CreateClient())
            {
                var response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var stagStudentInfo = JsonConvert.DeserializeObject<StagAktualniObdobiInfo>(jsonString);

                    return stagStudentInfo;
                }
            }

            return default;
        }
    }
}
