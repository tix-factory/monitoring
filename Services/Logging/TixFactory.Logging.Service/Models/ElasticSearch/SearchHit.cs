using System.Runtime.Serialization;

namespace TixFactory.Logging.Service.ElasticSearch
{
	[DataContract]
	internal class SearchHit
	{
		[DataMember(Name = "_id")]
		public string Id { get; set; }
	}
}
