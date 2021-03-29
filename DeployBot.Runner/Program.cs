using System;
using System.IO;
using System.Threading.Tasks;
using CommandLine;
using DeployBot.Infrastructure.Database;
using Serilog;

namespace DeployBot.Runner
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();

            try
            {
                var result = await Parser.Default.ParseArguments<DeploymentRunner.Options>(args)
                    .MapResult(o =>
                    {
                        var runner = new DeploymentRunner(Log.Logger, o);
                        return runner.RunDeploymentForRelease();
                    }, errors => Task.FromResult(DeploymentStatus.Failed));

                return (int)result;
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "An unexpected error occurred");
                return 1;
            }
        }
    }
}
