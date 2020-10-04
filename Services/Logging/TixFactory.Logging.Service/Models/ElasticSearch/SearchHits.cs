using System.Collections.Generic;
using System.Runtime.Serialization;

namespace TixFactory.Logging.Service.ElasticSearch
{
	[DataContract]
	internal class SearchHits
	{
		[DataMember(Name = "hits")]
		public IReadOnlyCollection<SearchHit> Data { get; set; }
	}
}
