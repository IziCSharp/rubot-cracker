using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Security;
using System.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Extreme.Net
{
	public static class Http
	{
		public const string NewLine = "\r\n";

		public static readonly RemoteCertificateValidationCallback AcceptAllCertificationsCallback;

		internal static readonly Dictionary<HttpHeader, string> Headers;

		[ThreadStatic]
		private static Random _rand;

		private static Random Rand
		{
			get
			{
				if (_rand == null)
				{
					_rand = new Random();
				}
				return _rand;
			}
		}

		static Http()
		{
			Headers = new Dictionary<HttpHeader, string>
			{
				{
					HttpHeader.Accept,
					"Accept"
				},
				{
					HttpHeader.AcceptCharset,
					"Accept-Charset"
				},
				{
					HttpHeader.AcceptLanguage,
					"Accept-Language"
				},
				{
					HttpHeader.AcceptDatetime,
					"Accept-Datetime"
				},
				{
					HttpHeader.CacheControl,
					"Cache-Control"
				},
				{
					HttpHeader.ContentType,
					"Content-Type"
				},
				{
					HttpHeader.Date,
					"Date"
				},
				{
					HttpHeader.Expect,
					"Expect"
				},
				{
					HttpHeader.From,
					"From"
				},
				{
					HttpHeader.IfMatch,
					"If-Match"
				},
				{
					HttpHeader.IfModifiedSince,
					"If-Modified-Since"
				},
				{
					HttpHeader.IfNoneMatch,
					"If-None-Match"
				},
				{
					HttpHeader.IfRange,
					"If-Range"
				},
				{
					HttpHeader.IfUnmodifiedSince,
					"If-Unmodified-Since"
				},
				{
					HttpHeader.MaxForwards,
					"Max-Forwards"
				},
				{
					HttpHeader.Pragma,
					"Pragma"
				},
				{
					HttpHeader.Range,
					"Range"
				},
				{
					HttpHeader.Referer,
					"Referer"
				},
				{
					HttpHeader.Upgrade,
					"Upgrade"
				},
				{
					HttpHeader.UserAgent,
					"User-Agent"
				},
				{
					HttpHeader.Via,
					"Via"
				},
				{
					HttpHeader.Warning,
					"Warning"
				},
				{
					HttpHeader.DNT,
					"DNT"
				},
				{
					HttpHeader.AccessControlAllowOrigin,
					"Access-Control-Allow-Origin"
				},
				{
					HttpHeader.AcceptRanges,
					"Accept-Ranges"
				},
				{
					HttpHeader.Age,
					"Age"
				},
				{
					HttpHeader.Allow,
					"Allow"
				},
				{
					HttpHeader.ContentEncoding,
					"Content-Encoding"
				},
				{
					HttpHeader.ContentLanguage,
					"Content-Language"
				},
				{
					HttpHeader.ContentLength,
					"Content-Length"
				},
				{
					HttpHeader.ContentLocation,
					"Content-Location"
				},
				{
					HttpHeader.ContentMD5,
					"Content-MD5"
				},
				{
					HttpHeader.ContentDisposition,
					"Content-Disposition"
				},
				{
					HttpHeader.ContentRange,
					"Content-Range"
				},
				{
					HttpHeader.ETag,
					"ETag"
				},
				{
					HttpHeader.Expires,
					"Expires"
				},
				{
					HttpHeader.LastModified,
					"Last-Modified"
				},
				{
					HttpHeader.Link,
					"Link"
				},
				{
					HttpHeader.Location,
					"Location"
				},
				{
					HttpHeader.P3P,
					"P3P"
				},
				{
					HttpHeader.Refresh,
					"Refresh"
				},
				{
					HttpHeader.RetryAfter,
					"Retry-After"
				},
				{
					HttpHeader.Server,
					"Server"
				},
				{
					HttpHeader.TransferEncoding,
					"Transfer-Encoding"
				}
			};
			AcceptAllCertificationsCallback = AcceptAllCertifications;
		}

		public static string UrlEncode(string str, Encoding encoding = null)
		{
			if (string.IsNullOrEmpty(str))
			{
				return string.Empty;
			}
			encoding = (encoding ?? Encoding.UTF8);
			byte[] bytes = encoding.GetBytes(str);
			int num = 0;
			int num2 = 0;
			for (int i = 0; i < bytes.Length; i++)
			{
				char c = (char)bytes[i];
				if (c == ' ')
				{
					num++;
				}
				else if (!IsUrlSafeChar(c))
				{
					num2++;
				}
			}
			if (num == 0 && num2 == 0)
			{
				return str;
			}
			int num3 = 0;
			byte[] array = new byte[bytes.Length + num2 * 2];
			for (int j = 0; j < bytes.Length; j++)
			{
				char c2 = (char)bytes[j];
				if (IsUrlSafeChar(c2))
				{
					array[num3++] = bytes[j];
					continue;
				}
				if (c2 == ' ')
				{
					array[num3++] = 43;
					continue;
				}
				array[num3++] = 37;
				array[num3++] = (byte)IntToHex((bytes[j] >> 4) & 0xF);
				array[num3++] = (byte)IntToHex(bytes[j] & 0xF);
			}
			return Encoding.ASCII.GetString(array);
		}

		public static string ToQueryString(IEnumerable<KeyValuePair<string, string>> parameters, bool dontEscape)
		{
			if (parameters == null)
			{
				throw new ArgumentNullException("parameters");
			}
			StringBuilder stringBuilder = new StringBuilder();
			foreach (KeyValuePair<string, string> parameter in parameters)
			{
				if (!string.IsNullOrEmpty(parameter.Key))
				{
					stringBuilder.Append(parameter.Key);
					stringBuilder.Append('=');
					if (dontEscape)
					{
						stringBuilder.Append(parameter.Value);
					}
					else
					{
						stringBuilder.Append(Uri.EscapeDataString(parameter.Value ?? string.Empty));
					}
					stringBuilder.Append('&');
				}
			}
			if (stringBuilder.Length != 0)
			{
				stringBuilder.Remove(stringBuilder.Length - 1, 1);
			}
			return stringBuilder.ToString();
		}

		public static string ToPostQueryString(IEnumerable<KeyValuePair<string, string>> parameters, bool dontEscape, Encoding encoding = null)
		{
			if (parameters == null)
			{
				throw new ArgumentNullException("parameters");
			}
			StringBuilder stringBuilder = new StringBuilder();
			foreach (KeyValuePair<string, string> parameter in parameters)
			{
				if (!string.IsNullOrEmpty(parameter.Key))
				{
					stringBuilder.Append(parameter.Key);
					stringBuilder.Append('=');
					if (dontEscape)
					{
						stringBuilder.Append(parameter.Value);
					}
					else
					{
						stringBuilder.Append(UrlEncode(parameter.Value ?? string.Empty, encoding));
					}
					stringBuilder.Append('&');
				}
			}
			if (stringBuilder.Length != 0)
			{
				stringBuilder.Remove(stringBuilder.Length - 1, 1);
			}
			return stringBuilder.ToString();
		}

		public static string DetermineMediaType(string extension)
		{
			string result = "application/octet-stream";
			try
			{
				using (RegistryKey registryKey = Registry.ClassesRoot.OpenSubKey(extension))
				{
					if (registryKey == null)
					{
						return result;
					}
					object value = registryKey.GetValue("Content Type");
					if (value == null)
					{
						return result;
					}
					result = value.ToString();
					return result;
				}
			}
			catch (IOException)
			{
				return result;
			}
			catch (ObjectDisposedException)
			{
				return result;
			}
			catch (UnauthorizedAccessException)
			{
				return result;
			}
			catch (SecurityException)
			{
				return result;
			}
		}

		public static string IEUserAgent()
		{
			string text = RandomWindowsVersion();
			string text2 = null;
			string text3 = null;
			string text4 = null;
			string text5 = null;
			if (text.Contains("NT 5.1"))
			{
				text2 = "9.0";
				text3 = "5.0";
				text4 = "5.0";
				text5 = ".NET CLR 2.0.50727; .NET CLR 3.5.30729";
			}
			else if (text.Contains("NT 6.0"))
			{
				text2 = "9.0";
				text3 = "5.0";
				text4 = "5.0";
				text5 = ".NET CLR 2.0.50727; Media Center PC 5.0; .NET CLR 3.5.30729";
			}
			else
			{
				switch (Rand.Next(3))
				{
				case 0:
					text2 = "10.0";
					text4 = "6.0";
					text3 = "5.0";
					break;
				case 1:
					text2 = "10.6";
					text4 = "6.0";
					text3 = "5.0";
					break;
				case 2:
					text2 = "11.0";
					text4 = "7.0";
					text3 = "5.0";
					break;
				}
				text5 = ".NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; Media Center PC 6.0; .NET4.0C; .NET4.0E";
			}
			return $"Mozilla/{text3} (compatible; MSIE {text2}; {text}; Trident/{text4}; {text5})";
		}

		public static string OperaUserAgent()
		{
			string arg = null;
			string arg2 = null;
			switch (Rand.Next(4))
			{
			case 0:
				arg = "12.16";
				arg2 = "2.12.388";
				break;
			case 1:
				arg = "12.14";
				arg2 = "2.12.388";
				break;
			case 2:
				arg = "12.02";
				arg2 = "2.10.289";
				break;
			case 3:
				arg = "12.00";
				arg2 = "2.10.181";
				break;
			}
			return $"Opera/9.80 ({RandomWindowsVersion()}); U) Presto/{arg2} Version/{arg}";
		}

		public static string ChromeUserAgent()
		{
			string arg = null;
			string arg2 = null;
			switch (Rand.Next(5))
			{
			case 0:
				arg = "41.0.2228.0";
				arg2 = "537.36";
				break;
			case 1:
				arg = "41.0.2227.1";
				arg2 = "537.36";
				break;
			case 2:
				arg = "41.0.2224.3";
				arg2 = "537.36";
				break;
			case 3:
				arg = "41.0.2225.0";
				arg2 = "537.36";
				break;
			case 4:
				arg = "41.0.2226.0";
				arg2 = "537.36";
				break;
			}
			return $"Mozilla/5.0 ({RandomWindowsVersion()}) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/{arg} Safari/{arg2}";
		}

		public static string FirefoxUserAgent()
		{
			string arg = null;
			string arg2 = null;
			switch (Rand.Next(5))
			{
			case 0:
				arg2 = "36.0";
				arg = "20100101";
				break;
			case 1:
				arg2 = "33.0";
				arg = "20100101";
				break;
			case 2:
				arg2 = "31.0";
				arg = "20100101";
				break;
			case 3:
				arg2 = "29.0";
				arg = "20120101";
				break;
			case 4:
				arg2 = "28.0";
				arg = "20100101";
				break;
			}
			return string.Format("Mozilla/5.0 ({0}; rv:{1}) Gecko/{2} Firefox/{1}", RandomWindowsVersion(), arg2, arg);
		}

		public static string OperaMiniUserAgent()
		{
			string text = null;
			string text2 = null;
			string text3 = null;
			string text4 = null;
			switch (Rand.Next(3))
			{
			case 0:
				text = "iOS";
				text2 = "7.0.73345";
				text3 = "11.62";
				text4 = "2.10.229";
				break;
			case 1:
				text = "J2ME/MIDP";
				text2 = "7.1.23511";
				text3 = "12.00";
				text4 = "2.10.181";
				break;
			case 2:
				text = "Android";
				text2 = "7.5.54678";
				text3 = "12.02";
				text4 = "2.10.289";
				break;
			}
			return $"Opera/9.80 ({text}; Opera Mini/{text2}/28.2555; U; ru) Presto/{text4} Version/{text3}";
		}

		private static bool AcceptAllCertifications(object sender, X509Certificate certification, X509Chain chain, SslPolicyErrors sslPolicyErrors)
		{
			return true;
		}

		private static bool IsUrlSafeChar(char c)
		{
			if ((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || (c >= '0' && c <= '9'))
			{
				return true;
			}
			switch (c)
			{
			case '!':
			case '(':
			case ')':
			case '*':
			case '-':
			case '.':
			case '_':
				return true;
			default:
				return false;
			}
		}

		private static char IntToHex(int i)
		{
			if (i <= 9)
			{
				return (char)(i + 48);
			}
			return (char)(i - 10 + 65);
		}

		private static string RandomWindowsVersion()
		{
			string text = "Windows NT ";
			switch (Rand.Next(4))
			{
			case 0:
				text += "5.1";
				break;
			case 1:
				text += "6.0";
				break;
			case 2:
				text += "6.1";
				break;
			case 3:
				text += "6.2";
				break;
			}
			if (Rand.NextDouble() < 0.2)
			{
				text += "; WOW64";
			}
			return text;
		}
	}
}
