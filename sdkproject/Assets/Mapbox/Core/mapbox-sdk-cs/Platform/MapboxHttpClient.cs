#if NET_STANDARD_2_0
namespace Mapbox.Platform
{
	using Mapbox.Unity.Utilities;
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Net;
	using System.Net.Http;
	using System.Threading;
	using System.Threading.Tasks;

	public sealed class MapboxHttpClient
	{

		//private static readonly Lazy<MapboxHttpClient> lazy = new Lazy<MapboxHttpClient>(() => new MapboxHttpClient());
		//public static MapboxHttpClient Instance => lazy.Value;

		private static volatile MapboxHttpClient _instance;
		private static object _syncRoot = new object();


		public static MapboxHttpClient Instance
		{
			get
			{
				// implementation from: Implementing Singleton in C#
				// https://msdn.microsoft.com/en-us/library/ff650316.aspx
				if (null == _instance)
				{
					lock (_syncRoot)
					{
						if (null == _instance)
						{
							_instance = new MapboxHttpClient();
						}
					}
				}

				return _instance;
			}
		}


		private HttpClient _client;

		private MapboxHttpClient()
		{
			UnityEngine.Debug.Log("MapboxHttpClient constructor");
			ServicePointManager.Expect100Continue = true;
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
			ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
			//ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;
			//var cookies = new CookieContainer();

			_client = new HttpClient(new HttpClientHandler
			{
				AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip,

				//AutomaticRedirection = true,
				//MaxConnectionsPerServer = Environment.ProcessorCount,

				//CookieContainer = cookies
				//UseProxy = true,
				//Proxy = new WebProxy("192.168.1.125", 8888)
			});

			//_client.DefaultRequestHeaders.Add("Accept", "text/html, application/json");
			_client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate");

			// TO FIX: UserAgent.GetUserAgentEditor() currently doesn't work as we are not on the main thread
			_client.DefaultRequestHeaders.Add("User-Agent", "MapboxEventsUnity - WILLY testing 2018.1 BETA");
		}



		public async Task<Response> Get(
			string url
			, int timeout
			, Dictionary<string, string> headers = null
		)
		{
			Response response = null;
			var cts = new CancellationTokenSource();
			// timeout being passed in is in seconds
			cts.CancelAfter(timeout * 1000);

			HttpResponseMessage responseMessage = null;
			List<Exception> exceptions = new List<Exception>();

			try
			{
				HttpRequestMessage requestMessage = new HttpRequestMessage()
				{
					RequestUri = new Uri(url),
					Method = HttpMethod.Get
				};

				if (null != headers)
				{
					foreach (var hdr in headers)
					{
						requestMessage.Headers.Add(hdr.Key, hdr.Value);
					}
				}

				responseMessage = await _client.SendAsync(requestMessage, cts.Token);
				response = await Response.FromHttpResponseMessage(responseMessage);
			}
			catch (WebException webEx)
			{
				logException(webEx);
				exceptions.Add(webEx);
			}
			catch (TaskCanceledException cancelTaskEx)
			{
				exceptions.Add(cancelTaskEx);
				logException(cancelTaskEx);
				if (
					cancelTaskEx.CancellationToken == cts.Token
					|| cancelTaskEx.CancellationToken.IsCancellationRequested
				)
				{
					// a real cancellation, triggered by the caller
					exceptions.Add(new Exception("aborted"));
				}
				else
				{
					exceptions.Add(new Exception("timed out"));
					// a web request timeout (possibly other things!?)
				}
			}
			catch (OperationCanceledException cancelOpEx)
			{
				exceptions.Add(cancelOpEx);
				logException(cancelOpEx);
				if (cancelOpEx.CancellationToken.IsCancellationRequested)
				{
					// a real cancellation, triggered by the caller
					exceptions.Add(new Exception("aborted"));
				}
				else
				{
					exceptions.Add(new Exception("timed out"));
					// a web request timeout (possibly other things!?)
				}
			}
			catch (Exception ex)
			{
				exceptions.Add(ex);
				logException(ex);
			}
			finally
			{
				if (null != responseMessage)
				{
					responseMessage.Dispose();
					responseMessage = null;
				}
			}

			if (null == response)
			{
				response = await Response.FromHttpResponseMessage(null);
			}

			exceptions.ForEach(ex => response.AddException(ex));

			return response;
		}


		private void logException(Exception ex)
		{
#if UNITY_2018_1_OR_NEWER
			UnityEngine.Debug.LogError(ex);
#else
			Console.WriteLine(ex);
#endif
		}

	}
}
#endif
