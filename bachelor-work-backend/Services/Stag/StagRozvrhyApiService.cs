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
using bachelor_work_backend.Models.Student;
using bachelor_work_backend.Models.Rozvrh;

namespace bachelor_work_backend.Services.Stag
{
    public class StagRozvrhyApiService
    {
        public IHttpClientFactory ClientFactory { get; private set; }

        private readonly string stagApiUrl;
        public StagRozvrhyApiService(string stagApiUrl, IHttpClientFactory clientFactory)
        {
            ClientFactory = clientFactory;
            this.stagApiUrl = stagApiUrl;
        }

        public async Task<List<StagRozvrhoveAkce>?> GetRozvrhoveAkce(string rok, string semestr, string zkrPredm, string wscookie)
        {
            NameValueCollection nvc = new NameValueCollection();
            nvc.Add("outputFormat", "JSON");
            nvc.Add("rokVarianty", rok);
            nvc.Add("semestr", semestr);
            nvc.Add("zkrPredm", zkrPredm);

            var queryStringParams = UtilsService.ToQueryString(nvc);
            var request = new HttpRequestMessage(HttpMethod.Get, stagApiUrl + "/ws/services/rest2/rozvrhy/getRozvrhoveAkce" + queryStringParams);
            request.Headers.Add("Cookie", "WSCOOKIE=" + wscookie + ";");

            using (var client = ClientFactory.CreateClient())
            {
                var response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var jsonResponse = JsonConvert.DeserializeObject<StagRozvrhoveAkceJsonResponse>(jsonString);

                    return jsonResponse.rozvrhovaAkce;
                }
            }

            return default;
        }
    }
}
