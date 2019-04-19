using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Extreme.Net
{
	public class ProxyHandler : HttpClientHandler
	{
		private ProxyClient proxyClient;

		public ProxyHandler(ProxyClient proxyClient)
		{
			this.proxyClient = proxyClient;
		}

		protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			HttpRequest httpRequest = request.ToHttpRequest();
			httpRequest.Proxy = proxyClient;
			cancellationToken.ThrowIfCancellationRequested();
			HttpResponse httpResponse = (request.Method == System.Net.Http.HttpMethod.Get) ? (await GetAsync(httpRequest, request)) : ((!(request.Method == System.Net.Http.HttpMethod.Post)) ? (await RawAsync(httpRequest, request)) : (await PostAsync(httpRequest, request)));
			cancellationToken.ThrowIfCancellationRequested();
			HttpResponseMessage httpResponseMessage = httpResponse.ToHttpResponseMessage();
			httpResponseMessage.RequestMessage = request;
			MemoryStream memoryStream = httpResponse.ToMemoryStream();
			if (memoryStream != null)
			{
				httpResponseMessage.Content = new ProgressStreamContent(memoryStream, CancellationToken.None);
			}
			return httpResponseMessage;
		}

		private async Task<HttpResponse> GetAsync(HttpRequest request, HttpRequestMessage message)
		{
			return await request.GetAsync(message.RequestUri.ToString());
		}

		private async Task<HttpResponse> PostAsync(HttpRequest request, HttpRequestMessage message)
		{
			byte[] bytes = await message.Content.ReadAsByteArrayAsync();
			return await request.PostAsync(message.RequestUri.ToString(), bytes);
		}

		private async Task<HttpResponse> RawAsync(HttpRequest request, HttpRequestMessage message)
		{
			HttpMethod method = ConvertMethod(message.Method);
			return await request.RawAsync(method, message.RequestUri.ToString());
		}

		private HttpMethod ConvertMethod(System.Net.Http.HttpMethod netHttpMethod)
		{
			if (netHttpMethod == System.Net.Http.HttpMethod.Head)
			{
				return HttpMethod.HEAD;
			}
			if (netHttpMethod == System.Net.Http.HttpMethod.Delete)
			{
				return HttpMethod.DELETE;
			}
			if (netHttpMethod == System.Net.Http.HttpMethod.Put)
			{
				return HttpMethod.PUT;
			}
			throw new HttpException($"Method {netHttpMethod} not supported");
		}
	}
}
