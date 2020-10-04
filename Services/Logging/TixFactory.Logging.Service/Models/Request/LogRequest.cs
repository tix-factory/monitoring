using System.Runtime.Serialization;

namespace TixFactory.Logging.Service
{
	[DataContract]
	public class LogRequest
	{
		[DataMember(Name = "message")]
		public string Message { get; set; }

		[DataMember(Name = "log")]
		public LogData Log { get; set; }

		[DataMember(Name = "host")]
		public HostData Host { get; set; }
	}
}
