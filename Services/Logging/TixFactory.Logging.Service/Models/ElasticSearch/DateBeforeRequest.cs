using System.Text.Json.Serialization;

namespace TixFactory.Logging.Service.ElasticSearch
{
	internal class DateBeforeRequest
	{
		[JsonPropertyName("@timestamp")]
		public LessThanRange Timestamp { get; set; }
	}
}
