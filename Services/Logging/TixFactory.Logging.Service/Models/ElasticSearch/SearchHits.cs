using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TixFactory.Logging.Service.ElasticSearch
{
	internal class SearchHits
	{
		[JsonPropertyName("hits")]
		public IReadOnlyCollection<SearchHit> Data { get; set; }
	}
}
