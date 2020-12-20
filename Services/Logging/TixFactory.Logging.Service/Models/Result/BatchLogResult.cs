using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace TixFactory.Logging.Service
{
	[DataContract]
	public class BatchLogResult
	{
		[DataMember(Name = "successfulLogIds")]
		[JsonPropertyName("successfulLogIds")]
		public IReadOnlyCollection<string> SuccessfulLogIds { get; set; }

		[DataMember(Name = "failedLogIds")]
		[JsonPropertyName("failedLogIds")]
		public IReadOnlyCollection<string> FailedLogIds { get; set; }
	}
}
