using System.Text.Json.Serialization;

namespace TixFactory.Logging.Service.ElasticSearch
{
	internal class LogData
	{
		[JsonPropertyName("name")]
		public string Name { get; set; }

		[JsonPropertyName("level")]
		[JsonConverter(typeof(JsonStringEnumConverter))]
		public LogLevel Level { get; set; }
	}
}
