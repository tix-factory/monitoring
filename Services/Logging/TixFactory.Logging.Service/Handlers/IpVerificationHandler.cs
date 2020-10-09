using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace TixFactory.Logging.Service
{
	internal class IpVerificationHandler
	{
		private readonly RequestDelegate _NextHandler;
		private readonly ILogger _Logger;
		private readonly ISet<string> _AllowedIpAddresses;
		private readonly ISet<Regex> _RFC1918IpRegexes;

		/// <summary>
		/// Initializes a new <see cref="IpVerificationHandler"/>.
		/// </summary>
		/// <param name="nextHandler">A delegate for triggering the next handler.</param>
		/// <param name="logger">An <see cref="ILogger"/>.</param>
		/// <exception cref="ArgumentNullException">
		/// - <paramref name="nextHandler"/>
		/// - <paramref name="logger"/>
		/// </exception>
		public IpVerificationHandler(RequestDelegate nextHandler, ILogger logger)
		{
			_NextHandler = nextHandler ?? throw new ArgumentNullException(nameof(nextHandler));
			_Logger = logger ?? throw new ArgumentNullException(nameof(logger));

			var allowedIpAddressesVariable = Environment.GetEnvironmentVariable("LoggingServiceAllowedIpAddressesCsv") ?? string.Empty;
			_AllowedIpAddresses = new HashSet<string>(allowedIpAddressesVariable.Split(','));

			// https://stackoverflow.com/a/2814102/1663648
			_RFC1918IpRegexes = new HashSet<Regex>(new []
			{
				new Regex(@"^127\."),
				new Regex(@"^10\."),
				new Regex(@"(^172\.1[6-9]\.)|(^172\.2[0-9]\.)|(^172\.3[0-1]\.)"),
				new Regex(@"^192\.168\.")
			});
		}

		/// <summary>
		/// The method to invoke the handler.
		/// </summary>
		/// <param name="context">An <see cref="HttpContext"/>.</param>
		public Task Invoke(HttpContext context)
		{
			if (IsRequestValid(context.Request))
			{
				return _NextHandler(context);
			}

			var jsonBytes = Encoding.UTF8.GetBytes("{}");
			context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
			context.Response.ContentType = "application/json";
			return context.Response.Body.WriteAsync(jsonBytes, 0, jsonBytes.Length);
		}

		private bool IsRequestValid(HttpRequest request)
		{
			var connection = request.HttpContext.Connection;
			var remoteIpAddressString = connection.RemoteIpAddress.ToString();
			if (connection.RemoteIpAddress.AddressFamily == AddressFamily.InterNetworkV6)
			{
				remoteIpAddressString = connection.RemoteIpAddress.MapToIPv4().ToString();
			}

			if (IsLocalHost(request))
			{
				return true;
			}

			if (_AllowedIpAddresses.Contains(remoteIpAddressString))
			{
				return true;
			}

			if (IsRfc1918Address(remoteIpAddressString))
			{
				return true;
			}

			_Logger.Verbose("Request verification failed."
				+ $"\n\t{nameof(connection.LocalIpAddress)}: {connection.LocalIpAddress}"
				+ $"\n\t{nameof(connection.RemoteIpAddress)}: {connection.RemoteIpAddress}"
				+ $"\n\t{nameof(remoteIpAddressString)}: {remoteIpAddressString}"
				+ $"\n\tPath: {request.Path}");

			return false;
		}

		private bool IsRfc1918Address(string ipAddress)
		{
			foreach (var regex in _RFC1918IpRegexes)
			{
				if (regex.IsMatch(ipAddress))
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Whether or not an <see cref="HttpRequest"/> is coming from the machine itself.
		/// </summary>
		/// <remarks>
		/// https://www.strathweb.com/2016/04/request-islocal-in-asp-net-core/
		/// </remarks>
		/// <param name="request">The <see cref="HttpRequest"/>.</param>
		/// <returns><c>true</c> if the request is sent the same machine as the server.</returns>
		private bool IsLocalHost(HttpRequest request)
		{
			var connection = request.HttpContext.Connection;

			if (connection.RemoteIpAddress != null)
			{
				if (connection.LocalIpAddress != null)
				{
					return connection.RemoteIpAddress.Equals(connection.LocalIpAddress);
				}

				return IsLoopbackIpAddress(connection.RemoteIpAddress);
			}

			if (connection.RemoteIpAddress == null && connection.LocalIpAddress == null)
			{
				return true;
			}

			return false;
		}

		/// <summary>
		/// Whether or not an <see cref="IPAddress"/> is a loopback IP address.
		/// </summary>
		/// <remarks>
		/// https://docs.microsoft.com/en-us/dotnet/api/system.net.ipaddress.isloopback
		/// </remarks>
		/// <param name="ipAddress">The <see cref="IPAddress"/>.</param>
		/// <returns><c>true</c> if the address is a loopback IP address.</returns>
		private bool IsLoopbackIpAddress(IPAddress ipAddress)
		{
			if (!IPAddress.IsLoopback(ipAddress))
			{
				return false;
			}

			return ipAddress.AddressFamily == AddressFamily.InterNetwork
				   || ipAddress.AddressFamily == AddressFamily.InterNetworkV6;
		}
	}
}
