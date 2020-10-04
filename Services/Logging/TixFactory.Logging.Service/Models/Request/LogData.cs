using System.Runtime.Serialization;

namespace TixFactory.Logging.Service
{
	[DataContract]
	public class LogData
	{
		[DataMember(Name = "name")]
		public string Name { get; set; }

		[DataMember(Name = "level")]
		public LogLevel Level { get; set; }
	}
}
