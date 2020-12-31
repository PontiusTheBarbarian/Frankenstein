using Newtonsoft.Json;

namespace Frankenstein.Models
{
	public class Template
	{
		[JsonProperty("engine")]
		public string Engine { get; set; }
		[JsonProperty("shell")]
		public string Shell { get; set; }
		[JsonProperty("command")]
		public string Command { get; set; }
		[JsonProperty("workingDirectory")]
		public string WorkingDirectory { get; set; }
		[JsonProperty("arguments")]
		public string Arguments { get; set; }
	}
}
