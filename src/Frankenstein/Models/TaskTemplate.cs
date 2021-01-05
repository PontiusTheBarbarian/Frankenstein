using Newtonsoft.Json;

namespace Frankenstein.Models
{
	public class TaskTemplate
	{
		[JsonProperty("task")]
		public string Task { get; set; }
		[JsonProperty("name")]
		public string Name { get; set; }
		[JsonProperty("dependsOn")]
		public string[] DependsOn { get; set; }
		[JsonProperty("workingDirectory")]
		public string WorkingDirectory { get; set; }
		[JsonProperty("inputs")]
		public TaskInputs Inputs { get; set; }
	}

	public class TaskInputs
	{
		[JsonProperty("command")]
		public string Command { get; set; }
		[JsonProperty("arguments")]
		public string Arguments { get; set; }
	}
}
