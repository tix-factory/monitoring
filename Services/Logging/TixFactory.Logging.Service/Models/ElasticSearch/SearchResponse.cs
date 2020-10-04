using System.Text.Json.Serialization;

namespace TixFactory.Logging.Service.ElasticSearch
{
	internal class SearchResponse
	{
		[JsonPropertyName("hits")]
		public SearchHits Data { get; set; }
	}
}
