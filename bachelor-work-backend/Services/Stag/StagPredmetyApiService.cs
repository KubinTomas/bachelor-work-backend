using bachelor_work_backend.Models.Authentication;
using bachelor_work_backend.Services.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http.Json;
using System.Net.Http;
using bachelor_work_backend.DTO.subject;
using bachelor_work_backend.Models.Subject;

namespace bachelor_work_backend.Services.Stag
{
    public class StagPredmetyApiService
    {
        public IHttpClientFactory ClientFactory { get; private set; }

        private readonly string stagApiUrl;
        public StagPredmetyApiService(string stagApiUrl, IHttpClientFactory clientFactory)
        {
            ClientFactory = clientFactory;
            this.stagApiUrl = stagApiUrl;
        }
       
        public async Task<List<StagPredmetDTO>?> GetPredmetyByKatedra(string katedra, string rok, string semestr, string wscookie)
        {
            NameValueCollection nvc = new NameValueCollection();
            nvc.Add("outputFormat", "JSON");
            nvc.Add("katedra", katedra);
            nvc.Add("rok", rok);
            nvc.Add("semestr", semestr);

            var queryStringParams = UtilsService.ToQueryString(nvc);
            var request = new HttpRequestMessage(HttpMethod.Get, stagApiUrl + "/ws/services/rest2/predmety/getPredmetyByKatedra" + queryStringParams);
            request.Headers.Add("Cookie", "WSCOOKIE=" + wscookie + ";");

            using (var client = ClientFactory.CreateClient())
            {
                var response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var stagPredmetJsonResponse = JsonConvert.DeserializeObject<StagPredmetJsonResponse>(jsonString);

                    return stagPredmetJsonResponse.predmetKatedry;
                }
            }

            return default;
        }

      
    }
}
// bud model a do nej davat response + data co to vrati