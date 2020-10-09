using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace TixFactory.Logging.Service.ElasticSearch
{
	internal class RangeRequest<T>
		where T : class
	{
		[JsonPropertyName("range")]
		public T Range { get; set; }
	}
}
