using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Frankenstein.Commands
{
	[Command(
		Name = "frankenstein",
		OptionsComparison = StringComparison.OrdinalIgnoreCase)]
	[VersionOptionFromMember(
		"--version",
		MemberName = nameof(GetVersion))]
	[Subcommand(typeof(FrankensteinCreateCommand))]
	public class FrankensteinDefaultCommand : FrankensteinCommandBase
	{
		public FrankensteinDefaultCommand(
			ILogger<FrankensteinDefaultCommand> logger,
			IConsole console)
		{
			_logger = logger;
			_console = console;
		}

		protected override Task<int> OnExecute(CommandLineApplication app)
		{
			app.ShowHelp(); // Show help
			return Task.FromResult(0);
		}

		public static string GetVersion() => typeof(FrankensteinDefaultCommand).Assembly.GetName().Version.ToString();
	}
}
