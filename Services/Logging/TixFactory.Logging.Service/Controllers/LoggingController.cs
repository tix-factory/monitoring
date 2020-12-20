using System;
using System.Linq;
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
		public async Task<IActionResult> Log([FromBody] LogRequest request, CancellationToken cancellationToken)
		{
			await _ElasticLogger.LogAsync(request, cancellationToken).ConfigureAwait(false);
			return new NoContentResult();
		}

		[HttpPost]
		public async Task<BatchLogResult> BatchLog([FromBody] LogRequest[] requests, CancellationToken cancellationToken)
		{
			var logTasks = requests.Where(r => !string.IsNullOrWhiteSpace(r?.Id)).ToDictionary(r => r.Id, r => _ElasticLogger.LogAsync(r, cancellationToken));

			try
			{
				await Task.WhenAll(logTasks.Values).ConfigureAwait(false);
			}
			catch
			{
				// rip
				// maybe we should log this at some point
			}

			return new BatchLogResult
			{
				SuccessfulLogIds = logTasks.Where(t => t.Value.IsCompletedSuccessfully).Select(t => t.Key).ToArray(),
				FailedLogIds = logTasks.Where(t => !t.Value.IsCompletedSuccessfully).Select(t => t.Key).ToArray()
			};
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
