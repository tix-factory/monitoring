using System.Runtime.Serialization;

namespace TixFactory.Logging.Service.ElasticSearch
{
	[DataContract]
	internal class SearchResponse
	{
		[DataMember(Name = "hits")]
		public SearchHits Data { get; set; }
	}
}
