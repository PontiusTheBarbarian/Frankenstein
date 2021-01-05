using System.Collections.Generic;
using System.Threading.Tasks;
using Frankenstein.Models;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;

namespace Frankenstein.Commands
{
	[HelpOption(LongName = "help")]
	public abstract class FrankensteinCommandBase
	{
		protected ILogger _logger;
		protected IConsole _console;

		protected Dictionary<string, string> SupportedSdks = new Dictionary<string, string>()
		{
			{ "DotNetCore@1", "dotnet" },
			{ "Harvest@1", "harvest" },
		};

		protected List<Variable> CustomVariables = new List<Variable>();

		protected virtual Task<int> OnExecute(CommandLineApplication app)
			=> Task.FromResult(0);
	}
}
