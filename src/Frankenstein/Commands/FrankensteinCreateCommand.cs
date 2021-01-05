using Frankenstein.Extensions;
using Frankenstein.Models;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using static Bullseye.Targets;
using static SimpleExec.Command;

namespace Frankenstein.Commands
{
	[Command(
		Name = "create",
		Description = "Creates a new service from the specified yaml file",
		ShowInHelpText = true)]
	public class FrankensteinCreateCommand : FrankensteinCommandBase
	{
		public FrankensteinCreateCommand(
			ILogger<FrankensteinCreateCommand> logger,
			IConsole console)
		{
			_logger = logger;
			_console = console;
		}

		[Option(CommandOptionType.SingleValue,
			ShortName = "w",
			LongName = "working-directory",
			ValueName = "Working directory",
			Description = "Set current working directory, if not specified uses the current directory",
			ShowInHelpText = true)]
		public string WorkingDirectory { get; set; } = ".";

		[Option(CommandOptionType.SingleValue,
			ShortName = "c",
			LongName = "configuration",
			ValueName = "Configuration file",
			Description = "Set config file path, if not specified uses the default file name 'monster.yaml'",
			ShowInHelpText = true)]
		public string ConfigurationFile { get; set; } = "monster.yaml";

		private Dictionary<string, string[]> TaskDependencies = new Dictionary<string, string[]>();

		protected override async Task<int> OnExecute(CommandLineApplication app)
		{
			try
			{
				var schema = YAMLExtensions.ReadSchema($"{WorkingDirectory}\\{ConfigurationFile}");

				CustomVariables = schema.Variables;

				foreach (var taskTemplate in schema.TaskTemplates)
				{
					if (taskTemplate.DependsOn != null && taskTemplate.DependsOn.Length > 0)
					{
						Target(taskTemplate.Name,
							DependsOn(taskTemplate.DependsOn),
							() => ExecuteCommand(SupportedSdks[taskTemplate.Task], taskTemplate.Inputs.Command, ReadWorkingDirectory(taskTemplate, WorkingDirectory), arguments: taskTemplate.Inputs.Arguments));

						TaskDependencies.Add(taskTemplate.Name, taskTemplate.DependsOn ?? new string[] { });
					}
					else
					{
						Target(taskTemplate.Name,
							() => ExecuteCommand(SupportedSdks[taskTemplate.Task], taskTemplate.Inputs.Command, ReadWorkingDirectory(taskTemplate, WorkingDirectory), arguments: taskTemplate.Inputs.Arguments));

						var taskDeps = TaskDependencies.Values?.SelectMany(i => i)?.ToList();

						if (!TaskDependencies.Keys.Contains(taskTemplate.Name) && taskDeps?.Contains(taskTemplate.Name) == false)
						{
							TaskDependencies.Add(taskTemplate.Name, taskTemplate.DependsOn ?? new string[] { });
						}
					}
				}

				var targets = TaskDependencies.Keys.Select(i => i);

				Target("default", targets);
				await RunTargetsAndExitAsync(app.Arguments.Select(i => i.ToString())); // To validate?

				return 0;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				_logger.LogError(ex.Message);
				return 1;
			}
		}

		private void ExecuteCommand(string shell, string command, string workingDirectory, string arguments = "")
			=> Run(shell, $"{command} {arguments}".SubstituteVariables(CustomVariables), workingDirectory);

		private string ReadWorkingDirectory(TaskTemplate template, string workingDirectory)
			=> !string.IsNullOrWhiteSpace(template.WorkingDirectory)
				? $@"{workingDirectory}\{template.WorkingDirectory}"
				: workingDirectory;
	}
}
