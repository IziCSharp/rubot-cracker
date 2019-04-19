using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Extreme.Net
{
	internal static class HttpExtensions
	{
		private static readonly Dictionary<string, string> headerSeparators = new Dictionary<string, string>
		{
			{
				"User-Agent",
				" "
			}
		};

		public static HttpResponseMessage ToHttpResponseMessage(this HttpResponse httpResponse)
		{
			System.Net.HttpStatusCode statusCode = (System.Net.HttpStatusCode)httpResponse.StatusCode;
			HttpResponseMessage httpResponseMessage = new HttpResponseMessage(statusCode);
			Dictionary<string, string>.Enumerator enumerator = httpResponse.EnumerateHeaders();
			while (enumerator.MoveNext())
			{
				KeyValuePair<string, string> current = enumerator.Current;
				httpResponseMessage.Headers.TryAddWithoutValidation(current.Key, current.Value);
			}
			return httpResponseMessage;
		}

		public static HttpRequest ToHttpRequest(this HttpRequestMessage request)
		{
			HttpRequest httpRequest = new HttpRequest();
			HttpRequestHeaders headers = request.Headers;
			object second;
			if (request.Content == null)
			{
				second = Enumerable.Empty<KeyValuePair<string, IEnumerable<string>>>();
			}
			else
			{
				IEnumerable<KeyValuePair<string, IEnumerable<string>>> headers2 = request.Content.Headers;
				second = headers2;
			}
			IEnumerable<KeyValuePair<string, IEnumerable<string>>> enumerable = headers.Union((IEnumerable<KeyValuePair<string, IEnumerable<string>>>)second);
			foreach (KeyValuePair<string, IEnumerable<string>> item in enumerable)
			{
				httpRequest.AddHeader(item.Key, string.Join(GetHeaderSeparator(item.Key), item.Value));
			}
			return httpRequest;
		}

		private static string GetHeaderSeparator(string name)
		{
			if (headerSeparators.ContainsKey(name))
			{
				return headerSeparators[name];
			}
			return ",";
		}
	}
}
