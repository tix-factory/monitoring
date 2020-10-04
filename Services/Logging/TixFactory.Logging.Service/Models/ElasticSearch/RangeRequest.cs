using System.Runtime.Serialization;

namespace TixFactory.Logging.Service.ElasticSearch
{
	[DataContract]
	internal class RangeRequest<T>
		where T : class
	{
		[DataMember(Name = "range")]
		public T Range { get; set; }
	}
}
