using System.Collections.Generic;
using System.IO;
using CommandLine;
using Frankenstein.Extensions;
using Frankenstein.Models;
using Newtonsoft.Json;
using YamlDotNet.Serialization;
using static SimpleExec.Command;

namespace src
{
	public class Options
	{
		[Option('w', "work-directory", Default = ".", Required = false, HelpText = "Set current working directory")]
		public string WorkingDirectory { get; set; }
		[Option('c', "config", Default = "monster.yaml", Required = false, HelpText = "Set config file path")]
		public string ConfigurationFile { get; set; }
	}

	public class Program
	{
		private static readonly string[] supportedSDKs = new[] { "dotnet", "dotnet.exe", "cmd", "cmd.exe" };

		public static void Main(string[] args)
		=> Parser.Default.ParseArguments<Options>(args)
			.WithParsed<Options>(o =>
			{
#if (DEBUG)
				o.WorkingDirectory = "C:\\Users\\liama\\Desktop\\TestFolder";
				o.ConfigurationFile = "monster.yaml";
#endif
				var filePath = $"{o.WorkingDirectory}\\{o.ConfigurationFile}";
				var schema = ReadSchema(filePath);

				//TODO: move somewhere else
				StringExtensions.Variables = schema.Variables;

				foreach (KeyValuePair<string, Template> template in schema.Templates)
				{
					var shell = ReadShell(template.Value);
					var workingDirectory = ReadWorkingDirectory(template.Value, o.WorkingDirectory);

					//if (!supportedSDKs.Contains(shell))
					//{
					//	throw new Exception("SDK/Shell not supported");
					//}

					ExecuteCommand(shell, template.Value.Command, workingDirectory, arguments: template.Value.Arguments);
				}
			});

		private static void ExecuteCommand(string shell, string command, string workingDirectory, string arguments = "")
			=> Run(shell, $"{command} {arguments}".SubstituteVariables(), workingDirectory);
		private static Schema ReadSchema(string filePath)
		{
			// Read YAML file
			var reader = new StreamReader(filePath);
			var deserializer = new Deserializer();
			var yamlObject = deserializer.Deserialize(reader);

			// Convert the object to JSON
			JsonSerializer js = new JsonSerializer();

			var writer = new StringWriter();
			js.Serialize(writer, yamlObject);
			string jsonText = writer.ToString();

			//Convert to POCO
			return JsonConvert.DeserializeObject<Schema>(jsonText);
		}
		private static string ReadWorkingDirectory(Template template, string workingDirectory)
			=> !string.IsNullOrWhiteSpace(template.WorkingDirectory)
				? $@"{workingDirectory}\{template.WorkingDirectory}"
				: workingDirectory;
		private static string ReadShell(Template template)
			=> !string.IsNullOrWhiteSpace(template.Engine)
							? template.Engine
							: template.Shell;
	}
}
