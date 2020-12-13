﻿using bachelor_work_backend.Models.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace bachelor_work_backend.Services.Stag
{
    public class StagUserApiService
    {
        public IHttpClientFactory ClientFactory { get; private set; }

        private readonly string stagApiUrl;
        public StagUserApiService(string stagApiUrl, IHttpClientFactory clientFactory)
        {
            ClientFactory = clientFactory;
            this.stagApiUrl = stagApiUrl;
        }

        public User GetStagUser(string wscookie)
        {

            // TODO - Send HTTP requests
            //using (var client = new HttpClient())
            //{
            //    var response = client.GetAsync("umbraco/api/Member/Get?username=test");
            //    response.Wait();
            //}


            return null;
        }

        public async Task<StagUserInfo> GetStagUserListForLoginTicket(string ticket)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, stagApiUrl + "/ws/services/rest2/help/getStagUserListForLoginTicket");

            using (var client = new HttpClient())
            {
                var response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                   // var data = response.Content.readFrom
                }
            }

            return null;
        }
    }
}
// bud model a do nej davat response + data co to vrati