using System.Runtime.Serialization;

namespace TixFactory.Logging.Service.ElasticSearch
{
	[DataContract]
	internal class QueryRequest<T>
		where T : class
	{
		[DataMember(Name = "query")]
		public T Query { get; set; }
	}
}
