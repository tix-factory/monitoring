using System;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using TixFactory.ApplicationContext;
using TixFactory.Http;
using TixFactory.Http.Client;
using TixFactory.Logging.Service.ElasticSearch;
using HttpMethod = TixFactory.Http.HttpMethod;

namespace TixFactory.Logging.Service
{
	internal class ElasticLogger : IElasticLogger
	{
		private readonly IHttpClient _HttpClient;
		private readonly IApplicationContext _ApplicationContext;
		private readonly string _UrlBase;

		public ElasticLogger(IHttpClient httpClient, IApplicationContext applicationContext)
		{
			_HttpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
			_ApplicationContext = applicationContext ?? throw new ArgumentNullException(nameof(applicationContext));
			_UrlBase = Environment.GetEnvironmentVariable("ElasticSearchEndpoint");
		}

		public async Task LogAsync(LogRequest logRequest, CancellationToken cancellationToken)
		{
			if (logRequest == null)
			{
				throw new ArgumentNullException(nameof(logRequest));
			}

			var log = new Log
			{
				Message = logRequest.Message,
				HostData = new ElasticSearch.HostData
				{
					Name = logRequest.Host.Name
				},
				LogData = new ElasticSearch.LogData
				{
					Name = logRequest.Log.Name,
					Level = logRequest.Log.Level
				}
			};

			var httpRequest = new HttpRequest(HttpMethod.Put, new Uri($"{_UrlBase}/_doc/{Guid.NewGuid()}"));
			var json = JsonSerializer.Serialize(log);
			httpRequest.Body = new StringContent(json);
			httpRequest.Headers.AddOrUpdate("Content-Type", "application/json");

			var httpResponse = await _HttpClient.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false);
			if (!httpResponse.IsSuccessful)
			{
				throw new HttpException(httpRequest, httpResponse);
			}
		}

		public async Task<int> PurgeAsync(DateTime clearBefore, CancellationToken cancellationToken)
		{
			var searchRequestBody = new QueryRequest<RangeRequest<DateBeforeRequest>>
			{
				Query = new RangeRequest<DateBeforeRequest>
				{
					Range = new DateBeforeRequest
					{
						Timestamp = new LessThanRange
						{
							LessThan = clearBefore
						}
					}
				}
			};

			var httpRequest = new HttpRequest(HttpMethod.Post, new Uri($"{_UrlBase}/_search?size=1000"));
			var json = JsonSerializer.Serialize(searchRequestBody);
			httpRequest.Body = new StringContent(json);
			httpRequest.Headers.AddOrUpdate("Content-Type", "application/json");

			var httpResponse = await _HttpClient.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false);
			if (!httpResponse.IsSuccessful)
			{
				throw new ApplicationException($"Failed to send search query to elasticsearch ({httpResponse.StatusCode})\n{httpResponse.GetStringBody()}");
			}

			var responseJson = httpResponse.GetStringBody();
			var searchResults = JsonSerializer.Deserialize<SearchResponse>(responseJson);

			var deleteTasks = searchResults.Data.Data.Select(log => DeleteLogAsync(log.Id, cancellationToken)).ToArray();
			await Task.WhenAll(deleteTasks);

			return deleteTasks.Length;
		}
		
		private async Task DeleteLogAsync(string id, CancellationToken cancellationToken)
		{
			var httpRequest = new HttpRequest(HttpMethod.Delete, new Uri($"{_UrlBase}/{id}"));
			var deleteResponse = await _HttpClient.SendAsync(httpRequest, cancellationToken);
			if (!deleteResponse.IsSuccessful)
			{
				await LogAsync(new LogRequest
				{
					Message = $"{nameof(ElasticLogger)}.{nameof(DeleteLogAsync)}({id})\n\tUrl: {deleteResponse.Url}\n\tStatus Code: {deleteResponse.StatusCode}\n{deleteResponse.GetStringBody()}",
					Host = new HostData
					{
						Name = Environment.MachineName
					},
					Log = new LogData
					{
						Name = _ApplicationContext.Name,
						Level = LogLevel.Warning
					}
				}, cancellationToken).ConfigureAwait(false);
			}
		}
	}
}
