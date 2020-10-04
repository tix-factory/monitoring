using System.Runtime.Serialization;

namespace TixFactory.Logging.Service.ElasticSearch
{
	[DataContract]
	internal class HostData
	{
		[DataMember(Name = "name")]
		public string Name { get; set; }
	}
}
