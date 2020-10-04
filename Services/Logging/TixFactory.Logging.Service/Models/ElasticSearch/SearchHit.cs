using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace TixFactory.Logging.Service.ElasticSearch
{
	internal class SearchHit
	{
		[JsonPropertyName("_id")]
		public string Id { get; set; }
	}
}
