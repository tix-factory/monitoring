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
	}
}
