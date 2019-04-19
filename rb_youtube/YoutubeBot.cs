using Extreme.Net;
using Other;
using rubotNet.Lcheck;
using System;
using System.Linq;
using System.Threading;
using Timers;

namespace rb_youtube
{
	public class YoutubeBot
	{
		public Random rand;

		public static bool CheckSmth;

		public static bool chk_cl;

		public static bool prx_cl;

		public static int int_cl;

		private static int Generation;

		public static int countWorkThreads;

		public Random random_volume;

		private string channelName;

		public static string cmt_cl;

		public static int rt_cl;

		public static int rtn_cl;

		public static string st_cl;

		public static string et_cl;

		public static string rti_cl;

		public static string lact_cl;

		public static string options_cl;

		public static string parinfo;

		public int methodYoutube;

		public static string fmt_cl;

		public static string ptracking;

		private int gen;

		public int typeProxy;

		private YoutubeBot(string channelName, int gen, int typeProxy, int methodYoutube)
		{
			this.gen = gen;
			this.channelName = channelName;
			this.typeProxy = typeProxy;
			this.methodYoutube = methodYoutube;
			if (methodYoutube == 0)
			{
				Thread thread = new Thread(LoopMethod1);
				thread.IsBackground = true;
				thread.Start();
			}
			if (methodYoutube == 1)
			{
				Thread thread2 = new Thread(LoopMethod2);
				thread2.IsBackground = true;
				thread2.Start();
			}
		}

		private bool Working()
		{
			return gen == Generation;
		}

		public static void StartAll(string login, string channelName, int count, int interval, int typeProxy, int methodYoutube, int qualityYoutube, string UrlProxy)
		{
			StopAll();
            cmt_cl = "843.8";
            et_cl = "843.8";
            rt_cl = 15;
            rti_cl = "500";
            rtn_cl = 25;
            st_cl = "803.801";
            lact_cl = "1725";
            chk_cl = false;
            options_cl = "10";
            prx_cl = true;
            if (qualityYoutube == 0)
			{
				fmt_cl = "160";
			}
			if (qualityYoutube == 1)
			{
				fmt_cl = "133";
			}
			if (qualityYoutube == 2)
			{
				fmt_cl = "134";
			}
			if (qualityYoutube == 3)
			{
				fmt_cl = "135";
			}
			if (qualityYoutube == 4)
			{
				fmt_cl = "136";
			}
			if (qualityYoutube == 5)
			{
				fmt_cl = "298";
			}
			if (qualityYoutube == 6)
			{
				fmt_cl = "137";
			}
			if (qualityYoutube == 7)
			{
				fmt_cl = "299";
			}
			channelName = channelName.Trim();
			int gen = Generation;
		
			Thread thread = new Thread((ThreadStart)delegate
			{
				int num = Enumerable.Range(0, count).Select((Func<int, YoutubeBot>)delegate
				{
					Thread.Sleep(interval);
					return new YoutubeBot(channelName, gen, typeProxy, methodYoutube);
				}).Count();
			});
			thread.IsBackground = true;
			thread.Start();
		}

		private void LoopMethod1()
		{
			rand = new Random();
			int num = 10;
			int num2 = 40000;
			int num3 = 1;
			int num4 = 10;
			int num5 = 600;
			int num6 = 1;
			countWorkThreads++;
			while (Working())
			{
				try
				{
					string cPN = GetCPN();
					HttpRequest httpRequest = new HttpRequest();
					httpRequest.Cookies = new CookieDictionary();
					httpRequest.KeepAlive = true;
					httpRequest.ConnectTimeout = 10;
                    string[] useragent = new string[] { "Mozilla/5.0 (iPhone; CPU iPhone OS 6_1_4 like Mac OS X) AppleWebKit/536.26 (KHTML, like Gecko) Version/6.0 Mobile/10B350 Safari/8536.25", "Mozilla/5.0 (iPhone; CPU iPhone OS 7_0 like Mac OS X) AppleWebKit/537.51.1 (KHTML, like Gecko) Mobile/11A465 Twitter for iPhone", "Mozilla/5.0 (iPhone; CPU iPhone OS 7_0 like Mac OS X) AppleWebKit/537.51.1 (KHTML, like Gecko) Version/7.0 Mobile/11A465 Safari/9537.53", "Mozilla/5.0 (iPhone; CPU iPhone OS 7_0 like Mac OS X) AppleWebKit/537.51.1 (KHTML, like Gecko) Mobile/11A4449d Twitter for iPhone", "Mozilla/5.0 (iPad; CPU OS 7_0 like Mac OS X) AppleWebKit/537.51.1 (KHTML, like Gecko) Mobile/11A465 Twitter for iPhone", "Mozilla/5.0 (iPhone; CPU iPhone OS 6_1_3 like Mac OS X) AppleWebKit/536.26 (KHTML, like Gecko) Version/6.0 Mobile/10B329 Safari/8536.25", "Mozilla/5.0 (iPhone; CPU iPhone OS 6_0_1 like Mac OS X; nl-nl) AppleWebKit/536.26 (KHTML, like Gecko) CriOS/23.0.1271.96 Mobile/10A523 Safari/8536.25 (1C019986-AF73-46A7-8F31-0E86ADFFCDB4)" };

                    httpRequest.UserAgent = useragent[new Random().Next(0, useragent.Length)];
					if (prx_cl)
					{
						if (typeProxy == 0)
						{
							httpRequest.Proxy = HttpProxyClient.Parse(Otherparams.smartProxyList[rand.Next(Otherparams.smartProxyList.Length)]);
						}
						if (typeProxy == 1)
						{
							httpRequest.Proxy = Socks4ProxyClient.Parse(Otherparams.smartProxyList[rand.Next(Otherparams.smartProxyList.Length)]);
						}
						if (typeProxy == 2)
						{
							httpRequest.Proxy = Socks5ProxyClient.Parse(Otherparams.smartProxyList[rand.Next(Otherparams.smartProxyList.Length)]);
						}
					}
					httpRequest.AddHeader(HttpHeader.Accept, "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
					httpRequest.AddHeader(HttpHeader.AcceptLanguage, "ru");
					httpRequest.AddHeader("Upgrade-Insecure-Request", "1");
					string str = httpRequest.Get("https://m.youtube.com/watch?v=" + channelName).ToString();
					string text = str.Substring("plid=", "\\");
					string text2 = str.Substring("ei=", "\\");
					string text3 = str.Substring("oid\":\"", "\"");
					string text4 = str.Substring("ptchn\":\"", "\"");
					string value = str.Substring("visitorData\":\"", "\"").Replace("%3D", "=");
					string text5 = str.Substring("INNERTUBE_CONTEXT_CLIENT_VERSION\":\"", "\"");
					string text6 = str.Substring("PAGE_CL\":", ",");
					string value2 = str.Substring("PAGE_BUILD_LABEL\":\"", "\"");
					string value3 = str.Substring("VARIANTS_CHECKSUM\":\"", "\"");
					string text7 = str.Substring("c\":\"", "\"");
					string text8 = str.Substring("INNERTUBE_CONTEXT_HL\":\"", "\"");
					string text9 = str.Substring("hl\":\"", "\"");
					string text10 = str.Substring("cr\":\"", "\"");
					string text11 = str.Substring("INNERTUBE_CONTEXT_GL\":\"", "\"");
					string str2 = str.Substring("INNERTUBE_API_KEY\":\"", "\"");
					string text12 = str.Substring("of=", "\\");
					string text13 = str.Substring("vm=", "\\");
					string text14 = str.Substring("idpj\":\"", "\"");
					string text15 = str.Substring("ldpj\":\"", "\"");
					string text16 = str.Substring("ps\":\"", "\"");
					httpRequest.AddHeader(HttpHeader.Accept, "image/png,image/svg+xml,image/*;q=0.8,video/*;q=0.8,*/*;q=0.5");
					httpRequest.AddHeader(HttpHeader.AcceptLanguage, "ru-RU,ru;q=0.9,en-US;q=0.8,en;q=0.7");
					httpRequest.AddHeader(HttpHeader.Referer, "https://m.youtube.com/watch?v=" + channelName);
					httpRequest.Get("https://www.youtube.com/ptracking?html5=1&video_id=" + channelName + "&cpn=" + cPN + "&plid=" + text + "&ei=" + text2 + "&ptk=youtube_single&oid=" + text3 + "&ptchn=" + text4 + "&pltype=contentugclive");
					while (Working())
					{
						httpRequest.AddHeader(HttpHeader.Accept, "image/png,image/svg+xml,image/*;q=0.8,video/*;q=0.8,*/*;q=0.5");
						httpRequest.AddHeader(HttpHeader.AcceptLanguage, "ru-RU,ru;q=0.9,en-US;q=0.8,en;q=0.7");
						httpRequest.AddHeader(HttpHeader.Referer, "https://m.youtube.com/watch?v=" + channelName);
						httpRequest.Get("https://m.youtube.com/live_204?ns=yt&el=detailpage&euri&fmt=" + fmt_cl + "&html5=1&plid=" + text + "&cpn=" + cPN + "&ei=" + text2 + "&ps=" + text16 + "&noflv=1&st=" + num6 + "&video_id=" + channelName + "&metric=heartbeat&tpmt=3298.919387000001&с=" + text7 + "&сver=" + text5 + "&cplayer=UNIPLAYER&cbrand=apple&cbr=Safari%20Mobile&cbrver=12.0.15E148&cmodel=iphone&cos=iPhone&cosver=12_0_1&cplatform=MOBILE");
						httpRequest.AddHeader(HttpHeader.Accept, "*/*");
						httpRequest.AddHeader(HttpHeader.AcceptLanguage, "ru-RU,ru;q=0.9,en-US;q=0.8,en;q=0.7");
						httpRequest.AddHeader(HttpHeader.Referer, "https://m.youtube.com/watch?v=" + channelName);
						httpRequest.AddHeader("X-Goog-Visitor-Id", value);
						httpRequest.AddHeader("X-YouTube-Client-Name", "2");
						httpRequest.AddHeader("X-YouTube-Client-Version", text5);
						httpRequest.AddHeader("X-YouTube-Page-CL", text6);
						httpRequest.AddHeader("X-YouTube-Page-Label", value2);
						httpRequest.AddHeader("X-YouTube-Utc-Offset", "180");
						httpRequest.AddHeader("X-YouTube-Variants-Checksum", value3);
						string str3 = "{\"context\":{\"client\":{\"clientName\":\"" + text7 + "\",\"clientVersion\":\"" + text5 + "\",\"hl\":\"" + text8 + "\",\"gl\":\"" + text11 + "\",\"utc_offset_minutes\":180}},\"videoId\":\"" + channelName + "\",\"ut\":\"\"}";
						httpRequest.Post("https://m.youtube.com/youtubei/v1/player/live_state?alt=json&key=" + str2, str3, "application/json");
						httpRequest.AddHeader(HttpHeader.Accept, "image/png,image/svg+xml,image/*;q=0.8,video/*;q=0.8,*/*;q=0.5");
						httpRequest.AddHeader(HttpHeader.AcceptLanguage, "ru-RU,ru;q=0.8,en-US;q=0.5,en;q=0.3");
						httpRequest.AddHeader(HttpHeader.Referer, "https://m.youtube.com/watch?v=" + channelName);
						httpRequest.Get("https://s.youtube.com/api/stats/watchtime?ns=yt&el=detailpage&cpn=" + cPN + "&docid=" + channelName + "&ver=2&cmt=" + num + "&plid=" + text + "&ei=" + text2 + "&fmt=" + fmt_cl + "&fs=0&rt=" + num3 + "&of=" + text12 + "&euri&lact=" + num2 + "&live=dvr&cl=" + text6 + "&state=playing&vm=" + text13 + "&volume=100&c=" + text7 + "&cver=" + text5 + "&cplayer=UNIPLAYER&cbrand=apple&cbr=Safari%20Mobile&cbrver=12.0.15E148&cmodel=iphone&cos=iPhone&cosver=12_0_1&cplatform=MOBILE&delay=5&hl=" + text9 + "&cr=" + text10 + "&rtn=" + num5 + "&idpj=" + text14 + "&ldpj=" + text15 + "&rti=" + num4 + "&muted=0&st=" + num6 + "&et=" + num).ToString();
                      
                        httpRequest.Close();
                        Thread.Sleep(10000);

                    }
				}
				catch
				{
				}
			}
			countWorkThreads--;
		}

		private void LoopMethod2()
		{
			rand = new Random();
			countWorkThreads++;
			while (Working())
			{
				try
				{
					string cPN = GetCPN();
					HttpRequest httpRequest = new HttpRequest();
					httpRequest.Cookies = new CookieDictionary();
					httpRequest.KeepAlive = true;
					httpRequest.UserAgent = "Mozilla/5.0 (Linux; Android 6.0; Nexus 5 Build/MRA58N) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/66.0.3359.139 Mobile Safari/537.36";
					if (prx_cl)
					{
						int typeProxy2 = typeProxy;
						int typeProxy3 = typeProxy;
						int typeProxy4 = typeProxy;
					}
					httpRequest.AddHeader(HttpHeader.Accept, "image/webp,image/apng,image/*,*/*;q=0.8");
					httpRequest.AddHeader(HttpHeader.AcceptLanguage, "ru-RU,ru;q=0.9,en-US;q=0.8,en;q=0.7");
					httpRequest.AddHeader(HttpHeader.Referer, "https://m.youtube.com/watch?v=" + channelName);
					string str = httpRequest.Get("https://m.youtube.com/watch?v=" + channelName).ToString();
					string text = str.Substring("plid\":\"", "\"");
					string text2 = str.Substring("eventid\":\"", "\"");
					string text3 = str.Substring("of\":\"", "\"");
					string text4 = str.Substring("vm\":\"", "\"");
					string text5 = str.Substring("cl\":\"", "\"");
					string text6 = str.Substring("cver\":\"", "\"");
					string text7 = str.Substring("hl\":\"", "\"");
					string text8 = str.Substring("cr\":\"", "\"");
					string text9 = str.Substring("idpj\":\"", "\"");
					string text10 = str.Substring("ldpj\":\"", "\"");
					string text11 = str.Substring("timestamp\":\"", "\",\"");
					string str2 = str.Substring("innertube_api_key\":\"", "\"");
					string text12 = str.Substring("INNERTUBE_CONTEXT_GL\":\"", "\"");
					string text13 = str.Substring("HTML_LANG\":\"", "\"");
					while (Working())
					{
						httpRequest.AddHeader(HttpHeader.Accept, "image/webp,image/apng,image/*,*/*;q=0.8");
						httpRequest.AddHeader(HttpHeader.AcceptLanguage, "ru-RU,ru;q=0.9,en-US;q=0.8,en;q=0.7");
						httpRequest.AddHeader(HttpHeader.Referer, "https://m.youtube.com/watch?v=" + channelName);
						httpRequest.Get("https://s.youtube.com/api/stats/watchtime?ns=yt&el=detailpage&cpn=" + cPN + "&docid=" + channelName + "&ver=2&cmt=" + cmt_cl + ".8&plid=" + text + "&ei=" + text2 + "&fmt=&fs=0&rt=" + rt_cl + ".001&of=" + text3 + "&euri&lact=" + lact_cl + "&live=dvr&cl=" + text5 + "&state=playing&vm=" + text4 + "&volume=100&c=MWEB&cver=" + text6 + "&cplayer=UNIPLAYER&cbrand=google&cbr=Chrome%20Mobile&cbrver=66.0.3359.139&cmodel=nexus%205&cos=Android&cosver=6.0&cplatform=MOBILE&delay=5&hl=" + text7 + "&cr=" + text8 + "&uga=m27&rtn=" + rtn_cl + "&afmt=140&lio=" + text11 + ".201&idpj=" + text9 + "&ldpj=" + text10 + "&rti=" + rt_cl + "&muted=0&st=" + st_cl + "&et=" + et_cl + ".8").ToString();
						string str3 = "{\"context\":{\"client\":{\"clientName\":\"MWEB\",\"clientVersion\":\"" + text6 + "\",\"hl\":\"" + text13 + "\",\"gl\":\"" + text12 + "\"}},\"videoId\":\"" + channelName + "\",\"ut\":\"\"}";
						httpRequest.Post("https://m.youtube.com/youtubei/v1/player/live_state?alt=json&key=" + str2, str3, "application/json");
						Thread.Sleep(int_cl);
					}
				}
				catch
				{
				}
			}
			countWorkThreads--;
		}

		private static string GetCPN(int len = 16)
		{
			string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_";
			Random rand = new Random();
			return string.Join("", from _ in Enumerable.Range(0, len)
			select chars[rand.Next(chars.Length)]);
		}

		public static void StopAll()
		{
			Generation++;
		}

		public static void LikeBot(string channelName, string acc)
		{
			string value = acc.ToString().Split(':')[0].ToString();
			string value2 = acc.ToString().Split(':')[1].ToString();
			HttpRequest httpRequest = new HttpRequest();
			httpRequest.Cookies = new CookieDictionary();
			HttpResponse httpResponse = httpRequest.Get("https://accounts.google.com/ServiceLogin?continue=http%3A%2F%2Fyoutube.com&nojavascript=1&service=alerts&hl=en");
			string str = httpResponse.ToString();
			string value3 = str.Substring("gxf\" value=\"", "\"");
			RequestParams reqParams = new RequestParams
			{
				["Page"] = "RememberedSignIn",
				["gxf"] = value3,
				["continue"] = "http://youtube.com",
				["service"] = "alerts",
				["hl"] = "en",
				["_utf8"] = "?",
				["bgresponse"] = "js_disabled",
				["pstMsg"] = "0",
				["checkConnection"] = "",
				["checkedDomains"] = "youtube",
				["Email"] = value,
				["Passwd"] = value2,
				["PersistentCookie"] = "yes",
				["signIn"] = "Sign in"
			};
			httpRequest.AllowAutoRedirect = true;
			httpRequest.Post("https://accounts.google.com/signin/challenge/sl/password", reqParams);
			httpRequest.AddHeader(HttpHeader.Accept, "text/html, application/xhtml+xml, image/jxr, */*");
			httpRequest.AddHeader(HttpHeader.AcceptLanguage, "ru,en-US;q=0.7,en;q=0.3");
			httpRequest.KeepAlive = true;
			httpRequest.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0) like Gecko";
			HttpResponse httpResponse2 = httpRequest.Get("https://www.youtube.com/watch?v=" + channelName);
			string str2 = httpResponse2.ToString();
			string value4 = str2.Substring("XSRF_TOKEN': \"", "\"");
			string value5 = str2.Substring("X-YouTube-Identity-Token': \"", "\"");
			string value6 = str2.Substring("PAGE_CL': ", ",");
			string value7 = str2.Substring("PAGE_BUILD_LABEL': \"", "\"");
			string value8 = str2.Substring("VARIANTS_CHECKSUM': \"", "\"");
			string text = str2.Substring("itct\":\"", "\"");
			string value9 = str2.Substring("eventid\":\"", "\"");
			RequestParams requestParams = new RequestParams();
			requestParams["sej"] = "{\"clickTrackingParams\":\"" + text + "\",\"likeEndpoint\":{\"status\":\"LIKE\",\"target\":{\"videoId\":\"" + channelName + "\"}}}";
			requestParams["csn"] = value9;
			requestParams["session_token"] = value4;
			RequestParams reqParams2 = requestParams;
			httpRequest.AddHeader(HttpHeader.Accept, "*/*");
			httpRequest.AddHeader("X-YouTube-Client-Name", "1");
			httpRequest.AddHeader("X-YouTube-Client-Version", "1.20171128");
			httpRequest.AddHeader("X-Youtube-Identity-Token", value5);
			httpRequest.AddHeader("X-YouTube-Page-CL", value6);
			httpRequest.AddHeader("X-YouTube-Page-Label", value7);
			httpRequest.AddHeader("X-YouTube-Variants-Checksum", value8);
			httpRequest.AddHeader(HttpHeader.AcceptLanguage, "ru,en-US;q=0.7,en;q=0.3");
			httpRequest.AddHeader("Cache-Control", "no-cache");
			httpRequest.Post("https://www.youtube.com/service_ajax?name=likeEndpoint", reqParams2);
			httpRequest.Close();
		}
	}
}
