using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System;
using Serilog;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Serilog.Extensions.Logging;
using Microsoft.Extensions.Logging;
using Frankenstein.Commands;

namespace src
{
	public static class Program
	{
		private static async Task<int> Main(string[] args)
		{
			var Configuration = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
				.AddEnvironmentVariables()
				.AddCommandLine(args)
				.Build();

			Log.Logger = new LoggerConfiguration()
					.ReadFrom.Configuration(Configuration)
					.Enrich.FromLogContext()
				   .CreateLogger();

			var builder = new HostBuilder()
				.ConfigureServices((hostContext, services) =>
				{
					services.AddLogging(config
						=> config.ClearProviders().AddProvider(new SerilogLoggerProvider(Log.Logger)));
				});

			try
			{
				return await builder.RunCommandLineApplicationAsync<FrankensteinDefaultCommand>(args);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				return 1;
			}
		}
	}
}