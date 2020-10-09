using System;
using System.Text.Json.Serialization;

namespace TixFactory.Logging.Service.ElasticSearch
{
	internal class LessThanRange
	{
		[JsonPropertyName("lte")]
		public DateTime LessThan { get; set; }
	}
}
