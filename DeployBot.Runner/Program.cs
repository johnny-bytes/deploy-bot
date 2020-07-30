using System;
using System.IO;
using System.Threading.Tasks;
using CommandLine;
using DeployBot.Infrastructure.Database;
using Microsoft.Extensions.Logging;

namespace DeployBot.Runner
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            loggerFactory.AddFile("Logs\\{Date}.txt");

            try
            {
                var result = await Parser.Default.ParseArguments<DeploymentRunner.Options>(args)
                    .MapResult(o =>
                    {
                        var runner = new DeploymentRunner(loggerFactory.CreateLogger<DeploymentRunner>(), o);
                        return runner.RunDeploymentForRelease();
                    },  errors => Task.FromResult(DeploymentStatus.Failed));

                return (int) result;
            }
            catch (Exception ex)
            {
                loggerFactory.CreateLogger<Program>().LogError(ex, "An unexpected error occurred");
                return 1;
            }
        }
    }
}
