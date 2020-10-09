using System.Text.Json.Serialization;

namespace TixFactory.Logging.Service.ElasticSearch
{
	internal class QueryRequest<T>
		where T : class
	{
		[JsonPropertyName("query")]
		public T Query { get; set; }
	}
}
