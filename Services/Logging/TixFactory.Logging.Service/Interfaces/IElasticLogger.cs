using System;

namespace TixFactory.Logging.Service
{
	public interface IElasticLogger
	{
		void Log(LogRequest logRequest);

		void Purge(DateTime clearBefore);
	}
}
