using bachelor_work_backend.Services.Stag;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace bachelor_work_backend.Services.Utils
{
    public class StagApiService
    {
        public IConfiguration Configuration { get; private set; }
        public StagUserApiService StagUserApiService { get; private set; }
        public StagApiKalendarService StagApiKalendarService { get; private set; }
        public IHttpClientFactory ClientFactory { get; private set; }

        public string StagApiUrl { get { return Configuration.GetSection("AppSettings").GetValue<string>("StagApiURL"); } }
        public StagApiService(IConfiguration configuration, IHttpClientFactory clientFactory)
        {
            Configuration = configuration;
            ClientFactory = clientFactory;
            StagUserApiService = new StagUserApiService(StagApiUrl, clientFactory);
            StagApiKalendarService = new StagApiKalendarService(StagApiUrl, clientFactory);
        }



    }
}
