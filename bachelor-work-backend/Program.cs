using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using bachelor_work_backend.Database;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace bachelor_work_backend
{
    public class Program
    {
        public async static Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            try
            {
                using (var scope = host.Services.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<BachContext>();
                    await dbContext.Database.MigrateAsync();
                }
            }
            catch (Exception)
            {

                
            }

            host.Run();

            //CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
//Scaffold-DbContext "Data Source=DESKTOP-I36NBNO;initial catalog=BachelorkWorkDb;Integrated Security=True;ConnectRetryCount=0" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Database -Context BachContext  -DataAnnotations -force
