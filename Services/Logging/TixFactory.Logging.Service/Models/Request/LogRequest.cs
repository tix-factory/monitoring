using System.Runtime.Serialization;

namespace TixFactory.Logging.Service
{
	[DataContract]
	public class LogRequest
	{
		/// <summary>
		/// Used for tracking on the consumer side, not attached to stored log data.
		/// </summary>
		[DataMember(Name = "id")]
		public string Id { get; set; }

		[DataMember(Name = "message")]
		public string Message { get; set; }

		[DataMember(Name = "log")]
		public LogData Log { get; set; }

		[DataMember(Name = "host")]
		public HostData Host { get; set; }
	}
}
