using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DemoWebApp {
    public class Program {
        public static async Task Main(string[] args) {
            var builder = Host.CreateDefaultBuilder(args);
            builder.ConfigureWebHostDefaults(webBuilder => {
                webBuilder.UseStartup<Startup>();
            });
            using (var host = builder.Build()) {
                //host.Run();
                await host.StartAsync();
                await host.WaitForShutdownAsync(CancellationToken.None);
            }
        }
    }
}
