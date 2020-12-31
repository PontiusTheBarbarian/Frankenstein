using System.Collections.Generic;
using Newtonsoft.Json;

namespace Frankenstein.Models
{
	public class Schema
	{
		[JsonProperty("version")]
		public string Version { get; set; }
		[JsonProperty("templates")]
		public Dictionary<string, Template> Templates { get; set; }
		[JsonProperty("variables")]
		public List<Variable> Variables { get; set; }
	}
}
