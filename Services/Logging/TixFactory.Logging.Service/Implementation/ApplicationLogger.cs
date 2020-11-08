using System;
using System.Threading;
using TixFactory.ApplicationContext;

namespace TixFactory.Logging.Service
{
	internal class ApplicationLogger : ILogger
	{
		private readonly IElasticLogger _ElasticLogger;
		private readonly IApplicationContext _ApplicationContext;

		public ApplicationLogger(IElasticLogger elasticLogger, IApplicationContext applicationContext)
		{
			_ElasticLogger = elasticLogger ?? throw new ArgumentNullException(nameof(elasticLogger));
			_ApplicationContext = applicationContext ?? throw new ArgumentNullException(nameof(applicationContext));
		}

		public void Verbose(string message)
		{
			Write(LogLevel.Verbose, message);
		}

		public void Info(string message)
		{
			Write(LogLevel.Information, message);
		}

		public void Warn(string message)
		{
			Write(LogLevel.Warning, message);
		}

		public void Error(Exception ex)
		{
			Error($"{ex}");
		}

		public void Error(string message)
		{
			Write(LogLevel.Error, message);
		}

		public void Write(LogLevel logLevel, string message)
		{
			try
			{
				var logTask = _ElasticLogger.LogAsync(new LogRequest
				{
					Message = message,
					Host = new HostData
					{
						Name = Environment.MachineName
					},
					Log = new LogData
					{
						Name = _ApplicationContext.Name,
						Level = logLevel
					}
				}, CancellationToken.None);

				logTask.Wait();
			}
			catch(Exception e)
			{
				Console.WriteLine($"{nameof(ApplicationLogger)}.{nameof(Write)}\n{e}");
			}
		}
	}
}
