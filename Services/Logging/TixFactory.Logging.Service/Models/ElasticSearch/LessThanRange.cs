using System;
using System.Runtime.Serialization;

namespace TixFactory.Logging.Service.ElasticSearch
{
	[DataContract]
	internal class LessThanRange
	{
		[DataMember(Name = "lte")]
		public DateTime LessThan { get; set; }
	}
}
