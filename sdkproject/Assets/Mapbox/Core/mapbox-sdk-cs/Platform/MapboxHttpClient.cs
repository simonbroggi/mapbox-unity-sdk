#if NET_4_6
namespace Mapbox.Platform
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Net;
	using System.Net.Http;
	using System.Threading.Tasks;

	public sealed class MapboxHttpClient
	{

		private static readonly Lazy<MapboxHttpClient> lazy = new Lazy<MapboxHttpClient>(() => new MapboxHttpClient());

		public static MapboxHttpClient Instance => lazy.Value;


		private HttpClient _client;

		private MapboxHttpClient()
		{
			//ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;
			//var cookies = new CookieContainer();

			_client = new HttpClient(new HttpClientHandler
			{
				AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip,
				//AutomaticRedirection = true,
				//CookieContainer = cookies

				//not available on Android
				//MaxConnectionsPerServer = 2,

				//UseProxy = true,
				//Proxy = new WebProxy("192.168.1.125", 8888)
			});

			//_client.DefaultRequestHeaders.Add("Accept", "text/html, application/json");
			_client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate");
			//_client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/61.0.3163.100 Safari/537.36");
		}



		public async Task<Response> Get(
			string url
			, Dictionary<string, string> headers = null
		)
		{
			Response response = null;

			try
			{
				HttpRequestMessage request = new HttpRequestMessage()
				{
					RequestUri = new Uri(url),
					Method = HttpMethod.Get
				};

				if (null != headers)
				{
					foreach (var hdr in headers)
					{
						request.Headers.Add(hdr.Key, hdr.Value);
					}
				}

				using (HttpResponseMessage resp = await _client.SendAsync(request))
				{
					response = await Response.FromHttpResponseMessage(resp);
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}

			return response;
		}




	}
}
#endif
