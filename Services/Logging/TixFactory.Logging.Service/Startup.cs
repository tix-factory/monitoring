using FakeItEasy;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace TixFactory.Logging.Service
{
	public class Startup : TixFactory.Http.Service.Startup
	{
		public Startup()
			: base(CreateLogger())
		{
		}

		public override void ConfigureServices(IServiceCollection services)
		{
			base.ConfigureServices(services);
		}

		public override void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			app.UseMiddleware<IpVerificationHandler>(Logger);

			base.Configure(app, env);
		}

		private static ILogger CreateLogger()
		{
			return A.Fake<ILogger>();
		}
	}
}
