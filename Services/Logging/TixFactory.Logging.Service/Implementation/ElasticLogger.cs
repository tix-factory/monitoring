using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using TixFactory.ApplicationContext;
using TixFactory.Http;
using TixFactory.Http.Client;
using TixFactory.Logging.Service.ElasticSearch;
using HttpMethod = TixFactory.Http.HttpMethod;

namespace TixFactory.Logging.Service
{
	internal class ElasticLogger : IElasticLogger
	{
		private const string _UrlBase = "http://tix-factory-monitoring:9200/tix-factory/logs";
		private readonly IHttpClient _HttpClient;
		private readonly IApplicationContext _ApplicationContext;

		public ElasticLogger(IHttpClient httpClient, IApplicationContext applicationContext)
		{
			_HttpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
			_ApplicationContext = applicationContext ?? throw new ArgumentNullException(nameof(applicationContext));
		}

		public void Log(LogRequest logRequest)
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

			var httpRequest = new HttpRequest(HttpMethod.Put, new Uri($"{_UrlBase}/{Guid.NewGuid()}"));
			var json = JsonSerializer.Serialize(log);
			httpRequest.Body = new StringContent(json);
			httpRequest.Headers.AddOrUpdate("Content-Type", "application/json");

			var httpResponse = _HttpClient.Send(httpRequest);
			if (!httpResponse.IsSuccessful)
			{
				throw new ApplicationException($"Failed to log to elasticsearch ({httpResponse.StatusCode})\n{httpResponse.GetStringBody()}");
			}
		}

		public void Purge(DateTime clearBefore)
		{
			var searchRequestBody = new QueryRequest<RangeRequest<LessThanRange>>
			{
				Query = new RangeRequest<LessThanRange>
				{
					Range = new LessThanRange
					{
						LessThan = clearBefore
					}
				}
			};

			var httpRequest = new HttpRequest(HttpMethod.Post, new Uri($"{_UrlBase}/_search"));
			var json = JsonSerializer.Serialize(searchRequestBody);
			httpRequest.Body = new StringContent(json, Encoding.UTF8, "application/json");

			var httpResponse = _HttpClient.Send(httpRequest);
			if (!httpResponse.IsSuccessful)
			{
				throw new ApplicationException($"Failed to send search query to elasticsearch ({httpResponse.StatusCode})\n{httpResponse.GetStringBody()}");
			}

			var responseJson = httpResponse.GetStringBody();
			var searchResults = JsonSerializer.Deserialize<SearchResponse>(responseJson);

			foreach (var log in searchResults.Data.Data)
			{
				var deleteResponse = Delete(log.Id);
				if (!deleteResponse.IsSuccessful)
				{
					Log(new LogRequest
					{
						Message = $"{nameof(ElasticLogger)}.{nameof(Delete)}({log.Id})\n\tUrl: {deleteResponse.Url}\n\tStatus Code: {deleteResponse.StatusCode}\n{deleteResponse.GetStringBody()}",
						Host = new HostData
						{
							Name = Environment.MachineName
						},
						Log = new LogData
						{
							Name = _ApplicationContext.Name,
							Level = LogLevel.Warning
						}
					});
				}
			}
		}

		private IHttpResponse Delete(string id)
		{
			var httpRequest = new HttpRequest(HttpMethod.Delete, new Uri($"{_UrlBase}/{id}"));
			return _HttpClient.Send(httpRequest);
		}
	}
}
