using System;
using Microsoft.AspNetCore.Mvc;

namespace TixFactory.Logging.Service.Controllers
{
	[Route("v1/[action]")]
	public class LoggingController
	{
		private readonly IElasticLogger _ElasticLogger;

		public LoggingController(IElasticLogger elasticLogger)
		{
			_ElasticLogger = elasticLogger ?? throw new ArgumentNullException(nameof(elasticLogger));
		}

		[HttpPost]
		public void Log([FromBody] LogRequest request)
		{
			_ElasticLogger.Log(request);
		}

		[HttpDelete]
		public void Purge(int months)
		{
			if (months < 1)
			{
				throw new ArgumentException($"{nameof(months)} must be at least 1.", nameof(months));
			}

			_ElasticLogger.Purge(DateTime.UtcNow - TimeSpan.FromDays(30 * months));
		}
	}
}
