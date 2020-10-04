using System;
using System.Text.Json.Serialization;

namespace TixFactory.Logging.Service.ElasticSearch
{
	internal class Log
	{
		[JsonPropertyName("message")]
		public string Message { get; set; }

		[JsonPropertyName("log")]
		public LogData LogData { get; set; }

		[JsonPropertyName("host")]
		public HostData HostData { get; set; }

		[JsonPropertyName("labels")]
		public object Labels { get; } = new object();

		[JsonPropertyName("@timestamp")]
		public DateTime Created => DateTime.UtcNow;
	}
}
