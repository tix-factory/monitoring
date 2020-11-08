using System;
using System.Threading;
using System.Threading.Tasks;

namespace TixFactory.Logging.Service
{
	public interface IElasticLogger
	{
		Task LogAsync(LogRequest logRequest, CancellationToken cancellationToken);

		Task PurgeAsync(DateTime clearBefore, CancellationToken cancellationToken);
	}
}
