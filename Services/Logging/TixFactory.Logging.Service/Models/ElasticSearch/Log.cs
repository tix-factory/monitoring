using System;
using System.Runtime.Serialization;

namespace TixFactory.Logging.Service.ElasticSearch
{
	[DataContract]
	internal class Log
	{
		[DataMember(Name = "message")]
		public string Message { get; set; }

		[DataMember(Name = "log")]
		public LogData LogData { get; set; }

		[DataMember(Name = "host")]
		public HostData HostData { get; set; }

		[DataMember(Name = "labels")]
		public object Labels { get; } = new object();

		[DataMember(Name = "@timestamp")]
		public DateTime Created => DateTime.UtcNow;
	}
}
