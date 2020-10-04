using System.Text.Json.Serialization;

namespace TixFactory.Logging.Service.ElasticSearch
{
	internal class HostData
	{
		[JsonPropertyName("name")]
		public string Name { get; set; }
	}
}
