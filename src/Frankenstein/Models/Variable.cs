using Newtonsoft.Json;

namespace Frankenstein.Models
{
	public class Variable
	{
		[JsonProperty("name")]
		public string Name { get; set; }
		[JsonProperty("value")]
		public string Value { get; set; }

		public string AsPowershellVariable() => $"$({Name})";
	}
}
