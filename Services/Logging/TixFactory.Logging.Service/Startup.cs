using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using TixFactory.Http.Client;

namespace TixFactory.Logging.Service
{
	public class Startup : TixFactory.Http.Service.Startup
	{
		private static readonly IElasticLogger _ElasticLogger;

		static Startup()
		{
			var httpClient = new HttpClient();
			_ElasticLogger = new ElasticLogger(httpClient);
		}

		public Startup()
			: base(CreateLogger(_ElasticLogger))
		{
		}

		public override void ConfigureServices(IServiceCollection services)
		{
			services.AddTransient(s => _ElasticLogger);

			base.ConfigureServices(services);
		}

		public override void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			app.UseMiddleware<IpVerificationHandler>(Logger);

			base.Configure(app, env);
		}

		private static ILogger CreateLogger(IElasticLogger elasticLogger)
		{
			return new ApplicationLogger(elasticLogger, TixFactory.ApplicationContext.ApplicationContext.Singleton);
		}
	}
}
