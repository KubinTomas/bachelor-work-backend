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

namespace bachelor_work_backend.Services.Stag
{
    // TODO RETURN ERRORS
    public class StagUserApiService
    {
        public IHttpClientFactory ClientFactory { get; private set; }

        private readonly string stagApiUrl;
        public StagUserApiService(string stagApiUrl, IHttpClientFactory clientFactory)
        {
            ClientFactory = clientFactory;
            this.stagApiUrl = stagApiUrl;
        }
        public async Task<string?> GetUcitelIdnoAsync(string wscookie)
        {
            var user = await GetStagUserAsync(wscookie);

            return user == null || string.IsNullOrEmpty(user.activeStagUserInfo.UcitIdno) ? null : user.activeStagUserInfo.UcitIdno;
        }
        public async Task<bool> IsStagUserCookieValidAsync(string wscookie)
        {
            return await GetStagUserListForLoginTicketAsync(wscookie) != null;
        }
        public async Task<User?> GetStagUserAsync(string wscookie)
        {
            var stagUserInfoList = await GetStagUserListForLoginTicketAsync(wscookie);

            if (stagUserInfoList == null)
            {
                return null;
            }

            var stagUser = stagUserInfoList.First().UserName;

            var actualStagUser = await GetStagUserForActualUserAsync(stagUser, wscookie);

            var user = new User()
            {
                activeStagUserInfo = actualStagUser,
                stagUserInfo = stagUserInfoList,
                IsStagUser = true,
                Email = string.Empty,
                Name = "",
                RoleId = 1,
            };

            if (!string.IsNullOrEmpty(user.activeStagUserInfo.UcitIdno))
            {
                var ucitelInfo = await GetUcitelInfoAsync(user.activeStagUserInfo.UcitIdno, wscookie);
                user.Email = ucitelInfo.Email;
                user.Name = ucitelInfo.TitulPred + " " + ucitelInfo.Jmeno + " " + ucitelInfo.Prijmeni + ", " + ucitelInfo.TitulZa;

                var externalLogin = await GetExternalLoginByUcitIdno(user.activeStagUserInfo.UcitIdno, wscookie);

                var stagUserInfoByExternalLogin = await GetStagUserListForExternalLogin(externalLogin, wscookie);

                if (stagUserInfoByExternalLogin != null)
                {
                    user.stagUserInfo = stagUserInfoByExternalLogin.Where(c => c.Aktivni == Constants.Stag.Aktivni).ToList();
                }
            }
            else if (!string.IsNullOrEmpty(user.activeStagUserInfo.UserName))
            {
                var studentInfo = await GetStudentInfo(user.activeStagUserInfo.UserName, wscookie);
                user.Email = studentInfo.email;
                user.Name = studentInfo.jmeno + " " + studentInfo.prijmeni;
            }

            return user;
        }

        public async Task<List<StagUserInfo>?> GetStagUserListForExternalLogin(string externalLogin, string ticket)
        {
            NameValueCollection nvc = new NameValueCollection();
            nvc.Add("outputFormat", "JSON");
            nvc.Add("externalLogin", externalLogin);

            var queryStringParams = UtilsService.ToQueryString(nvc);
            var request = new HttpRequestMessage(HttpMethod.Get, stagApiUrl + "/ws/services/rest2/users/getStagUserListForExternalLogin" + queryStringParams);
            request.Headers.Add("Cookie", "WSCOOKIE=" + ticket + ";");

            using (var client = ClientFactory.CreateClient())
            {
                var response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var stagUserInfoJsonResponse = JsonConvert.DeserializeObject<StagUserInfoJsonResponse>(jsonString);

                    return stagUserInfoJsonResponse.stagUserInfo;
                }
            }

            return default;
        }

        public async Task<string> GetExternalLoginByUcitIdno(string ucitIdno, string wscookie)
        {
            NameValueCollection nvc = new NameValueCollection();
            nvc.Add("ucitIdno", ucitIdno);

            var queryStringParams = UtilsService.ToQueryString(nvc);
            var request = new HttpRequestMessage(HttpMethod.Get, stagApiUrl + "/ws/services/rest2/users/getExternalLoginByUcitIdno" + queryStringParams);
            request.Headers.Add("Cookie", "WSCOOKIE=" + wscookie + ";");

            using (var client = ClientFactory.CreateClient())
            {
                var response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    //var stagStudentInfo = JsonConvert.DeserializeObject<StagStudentInfo>(jsonString);

                    return jsonString;
                }
            }

            return default;
        }

        public async Task<StagStudentInfo> GetStudentInfo(string osCislo, string wscookie)
        {
            NameValueCollection nvc = new NameValueCollection();
            nvc.Add("outputFormat", "JSON");
            nvc.Add("osCislo", osCislo);

            var queryStringParams = UtilsService.ToQueryString(nvc);
            var request = new HttpRequestMessage(HttpMethod.Get, stagApiUrl + "/ws/services/rest2/student/getStudentInfo" + queryStringParams);
            request.Headers.Add("Cookie", "WSCOOKIE=" + wscookie + ";");

            using (var client = ClientFactory.CreateClient())
            {
                var response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var stagStudentInfo = JsonConvert.DeserializeObject<StagStudentInfo>(jsonString);

                    return stagStudentInfo;
                }
            }

            return default;
        }

        public async Task<StagUcitelInfo> GetUcitelInfoAsync(string ucitIdno, string wscookie)
        {
            NameValueCollection nvc = new NameValueCollection();
            nvc.Add("outputFormat", "JSON");
            nvc.Add("ucitIdno", ucitIdno);

            var queryStringParams = UtilsService.ToQueryString(nvc);
            var request = new HttpRequestMessage(HttpMethod.Get, stagApiUrl + "/ws/services/rest2/ucitel/getUcitelInfo" + queryStringParams);
            request.Headers.Add("Cookie", "WSCOOKIE=" + wscookie + ";");

            using (var client = ClientFactory.CreateClient())
            {
                var response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var stagUcitelInfo = JsonConvert.DeserializeObject<StagUcitelInfo>(jsonString);

                    return stagUcitelInfo;
                }
            }

            return default;
        }

        public async Task<List<StagUserInfo>?> GetStagUserListForLoginTicketAsync(string ticket)
        {
            NameValueCollection nvc = new NameValueCollection();
            nvc.Add("outputFormat", "JSON");
            nvc.Add("ticket", ticket);

            var queryStringParams = UtilsService.ToQueryString(nvc);
            var request = new HttpRequestMessage(HttpMethod.Get, stagApiUrl + "/ws/services/rest2/help/getStagUserListForLoginTicket" + queryStringParams);
            request.Headers.Add("Cookie", "WSCOOKIE=" + ticket + ";");

            using (var client = ClientFactory.CreateClient())
            {
                var response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var stagUserInfoJsonResponse = JsonConvert.DeserializeObject<StagUserInfoJsonResponse>(jsonString);


                    return stagUserInfoJsonResponse.stagUserInfo;
                }
            }

            return default;
        }

        public async Task<StagUserInfo> GetStagUserForActualUserAsync(string stagUser, string wscookie)
        {
            NameValueCollection nvc = new NameValueCollection();
            nvc.Add("outputFormat", "JSON");
            nvc.Add("stagUser", stagUser);

            var queryStringParams = UtilsService.ToQueryString(nvc);
            var request = new HttpRequestMessage(HttpMethod.Get, stagApiUrl + "/ws/services/rest2/help/getStagUserForActualUser" + queryStringParams);
            request.Headers.Add("Cookie", "WSCOOKIE=" + wscookie + ";");

            using (var client = ClientFactory.CreateClient())
            {
                var response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var stagUserInfo = JsonConvert.DeserializeObject<StagUserInfo>(jsonString);

                    return stagUserInfo;
                }
            }

            return default;
        }
    }
}
// bud model a do nej davat response + data co to vrati