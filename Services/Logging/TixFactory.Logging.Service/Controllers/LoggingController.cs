using System;
using System.Threading;
using System.Threading.Tasks;
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
		public async Task<IActionResult> LogAsync([FromBody] LogRequest request, CancellationToken cancellationToken)
		{
			await _ElasticLogger.LogAsync(request, cancellationToken).ConfigureAwait(false);
			return new NoContentResult();
		}

		[HttpDelete]
		public async Task<IActionResult> Purge(int months, CancellationToken cancellationToken)
		{
			if (months < 1)
			{
				throw new ArgumentException($"{nameof(months)} must be at least 1.", nameof(months));
			}

			await _ElasticLogger.PurgeAsync(DateTime.UtcNow - TimeSpan.FromDays(30 * months), cancellationToken).ConfigureAwait(false);
			return new NoContentResult();
		}
	}
}
