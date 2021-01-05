using System.IO;
using Frankenstein.Models;
using Newtonsoft.Json;
using YamlDotNet.Serialization;

namespace Frankenstein.Extensions
{
	internal static class YAMLExtensions
	{
		internal static Schema ReadSchema(string filePath)
		{
			// Read YAML file
			var deserializer = new Deserializer();
			var yamlObject = deserializer.Deserialize(new StreamReader(filePath));

			// Convert to JSON
			var writer = new StringWriter();
			var jsonSerializer = new JsonSerializer();
			jsonSerializer.Serialize(writer, yamlObject);

			//Convert to POCO
			return JsonConvert.DeserializeObject<Schema>(writer.ToString());
		}
	}
}
