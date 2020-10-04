using System.Runtime.Serialization;

namespace TixFactory.Logging.Service.ElasticSearch
{
	[DataContract]
	internal class DateBeforeRequest
	{
		[DataMember(Name = "@timestamp")]
		public LessThanRange Timestamp { get; set; }
	}
}
