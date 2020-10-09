using System.Runtime.Serialization;

namespace TixFactory.Logging.Service
{
	[DataContract]
	public class HostData
	{
		[DataMember(Name = "name")]
		public string Name { get; set; }
	}
}
