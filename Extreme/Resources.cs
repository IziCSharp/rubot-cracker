using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Extreme
{
	[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
	[DebuggerNonUserCode]
	[CompilerGenerated]
	internal class Resources
	{
		private static ResourceManager resourceMan;

		private static CultureInfo resourceCulture;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static ResourceManager ResourceManager
		{
			get
			{
				if (resourceMan == null)
				{
					ResourceManager resourceManager = resourceMan = new ResourceManager("Extreme.Resources", typeof(Resources).Assembly);
				}
				return resourceMan;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static CultureInfo Culture
		{
			get
			{
				return resourceCulture;
			}
			set
			{
				resourceCulture = value;
			}
		}

		internal static string ArgumentException_CanNotReadOrSeek => ResourceManager.GetString("ArgumentException_CanNotReadOrSeek", resourceCulture);

		internal static string ArgumentException_EmptyString => ResourceManager.GetString("ArgumentException_EmptyString", resourceCulture);

		internal static string ArgumentException_HttpRequest_SetNotAvailableHeader => ResourceManager.GetString("ArgumentException_HttpRequest_SetNotAvailableHeader", resourceCulture);

		internal static string ArgumentException_MultiThreading_BegIndexRangeMoreEndIndex => ResourceManager.GetString("ArgumentException_MultiThreading_BegIndexRangeMoreEndIndex", resourceCulture);

		internal static string ArgumentException_OnlyAbsoluteUri => ResourceManager.GetString("ArgumentException_OnlyAbsoluteUri", resourceCulture);

		internal static string ArgumentException_WrongPath => ResourceManager.GetString("ArgumentException_WrongPath", resourceCulture);

		internal static string ArgumentOutOfRangeException_CanNotBeGreater => ResourceManager.GetString("ArgumentOutOfRangeException_CanNotBeGreater", resourceCulture);

		internal static string ArgumentOutOfRangeException_CanNotBeLess => ResourceManager.GetString("ArgumentOutOfRangeException_CanNotBeLess", resourceCulture);

		internal static string ArgumentOutOfRangeException_CanNotBeLessOrGreater => ResourceManager.GetString("ArgumentOutOfRangeException_CanNotBeLessOrGreater", resourceCulture);

		internal static string ArgumentOutOfRangeException_StringHelper_MoreLengthString => ResourceManager.GetString("ArgumentOutOfRangeException_StringHelper_MoreLengthString", resourceCulture);

		internal static string ArgumentOutOfRangeException_StringLengthCanNotBeMore => ResourceManager.GetString("ArgumentOutOfRangeException_StringLengthCanNotBeMore", resourceCulture);

		internal static string DirectoryNotFoundException_DirectoryNotFound => ResourceManager.GetString("DirectoryNotFoundException_DirectoryNotFound", resourceCulture);

		internal static string FormatException_ProxyClient_WrongPort => ResourceManager.GetString("FormatException_ProxyClient_WrongPort", resourceCulture);

		internal static string HttpException_ClientError => ResourceManager.GetString("HttpException_ClientError", resourceCulture);

		internal static string HttpException_ConnectTimeout => ResourceManager.GetString("HttpException_ConnectTimeout", resourceCulture);

		internal static string HttpException_Default => ResourceManager.GetString("HttpException_Default", resourceCulture);

		internal static string HttpException_FailedConnect => ResourceManager.GetString("HttpException_FailedConnect", resourceCulture);

		internal static string HttpException_FailedGetHostAddresses => ResourceManager.GetString("HttpException_FailedGetHostAddresses", resourceCulture);

		internal static string HttpException_FailedReceiveMessageBody => ResourceManager.GetString("HttpException_FailedReceiveMessageBody", resourceCulture);

		internal static string HttpException_FailedReceiveResponse => ResourceManager.GetString("HttpException_FailedReceiveResponse", resourceCulture);

		internal static string HttpException_FailedSendRequest => ResourceManager.GetString("HttpException_FailedSendRequest", resourceCulture);

		internal static string HttpException_FailedSslConnect => ResourceManager.GetString("HttpException_FailedSslConnect", resourceCulture);

		internal static string HttpException_LimitRedirections => ResourceManager.GetString("HttpException_LimitRedirections", resourceCulture);

		internal static string HttpException_ReceivedEmptyResponse => ResourceManager.GetString("HttpException_ReceivedEmptyResponse", resourceCulture);

		internal static string HttpException_ReceivedWrongResponse => ResourceManager.GetString("HttpException_ReceivedWrongResponse", resourceCulture);

		internal static string HttpException_SeverError => ResourceManager.GetString("HttpException_SeverError", resourceCulture);

		internal static string HttpException_WaitDataTimeout => ResourceManager.GetString("HttpException_WaitDataTimeout", resourceCulture);

		internal static string HttpException_WrongChunkedBlockLength => ResourceManager.GetString("HttpException_WrongChunkedBlockLength", resourceCulture);

		internal static string HttpException_WrongCookie => ResourceManager.GetString("HttpException_WrongCookie", resourceCulture);

		internal static string HttpException_WrongHeader => ResourceManager.GetString("HttpException_WrongHeader", resourceCulture);

		internal static string InvalidOperationException_ChainProxyClient_NotProxies => ResourceManager.GetString("InvalidOperationException_ChainProxyClient_NotProxies", resourceCulture);

		internal static string InvalidOperationException_HttpResponse_HasError => ResourceManager.GetString("InvalidOperationException_HttpResponse_HasError", resourceCulture);

		internal static string InvalidOperationException_NotSupportedEncodingFormat => ResourceManager.GetString("InvalidOperationException_NotSupportedEncodingFormat", resourceCulture);

		internal static string InvalidOperationException_ProxyClient_WrongHost => ResourceManager.GetString("InvalidOperationException_ProxyClient_WrongHost", resourceCulture);

		internal static string InvalidOperationException_ProxyClient_WrongPassword => ResourceManager.GetString("InvalidOperationException_ProxyClient_WrongPassword", resourceCulture);

		internal static string InvalidOperationException_ProxyClient_WrongPort => ResourceManager.GetString("InvalidOperationException_ProxyClient_WrongPort", resourceCulture);

		internal static string InvalidOperationException_ProxyClient_WrongUsername => ResourceManager.GetString("InvalidOperationException_ProxyClient_WrongUsername", resourceCulture);

		internal static string NetException_Default => ResourceManager.GetString("NetException_Default", resourceCulture);

		internal static string ProxyException_CommandError => ResourceManager.GetString("ProxyException_CommandError", resourceCulture);

		internal static string ProxyException_ConnectTimeout => ResourceManager.GetString("ProxyException_ConnectTimeout", resourceCulture);

		internal static string ProxyException_Default => ResourceManager.GetString("ProxyException_Default", resourceCulture);

		internal static string ProxyException_Error => ResourceManager.GetString("ProxyException_Error", resourceCulture);

		internal static string ProxyException_FailedConnect => ResourceManager.GetString("ProxyException_FailedConnect", resourceCulture);

		internal static string ProxyException_FailedGetHostAddresses => ResourceManager.GetString("ProxyException_FailedGetHostAddresses", resourceCulture);

		internal static string ProxyException_NotSupportedAddressType => ResourceManager.GetString("ProxyException_NotSupportedAddressType", resourceCulture);

		internal static string ProxyException_ReceivedEmptyResponse => ResourceManager.GetString("ProxyException_ReceivedEmptyResponse", resourceCulture);

		internal static string ProxyException_ReceivedWrongResponse => ResourceManager.GetString("ProxyException_ReceivedWrongResponse", resourceCulture);

		internal static string ProxyException_ReceivedWrongStatusCode => ResourceManager.GetString("ProxyException_ReceivedWrongStatusCode", resourceCulture);

		internal static string ProxyException_Socks5_FailedAuthOn => ResourceManager.GetString("ProxyException_Socks5_FailedAuthOn", resourceCulture);

		internal static string ProxyException_WaitDataTimeout => ResourceManager.GetString("ProxyException_WaitDataTimeout", resourceCulture);

		internal static string Socks_UnknownError => ResourceManager.GetString("Socks_UnknownError", resourceCulture);

		internal static string Socks4_CommandReplyRequestRejectedCannotConnectToIdentd => ResourceManager.GetString("Socks4_CommandReplyRequestRejectedCannotConnectToIdentd", resourceCulture);

		internal static string Socks4_CommandReplyRequestRejectedDifferentIdentd => ResourceManager.GetString("Socks4_CommandReplyRequestRejectedDifferentIdentd", resourceCulture);

		internal static string Socks4_CommandReplyRequestRejectedOrFailed => ResourceManager.GetString("Socks4_CommandReplyRequestRejectedOrFailed", resourceCulture);

		internal static string Socks5_AuthMethodReplyNoAcceptableMethods => ResourceManager.GetString("Socks5_AuthMethodReplyNoAcceptableMethods", resourceCulture);

		internal static string Socks5_CommandReplyAddressTypeNotSupported => ResourceManager.GetString("Socks5_CommandReplyAddressTypeNotSupported", resourceCulture);

		internal static string Socks5_CommandReplyCommandNotSupported => ResourceManager.GetString("Socks5_CommandReplyCommandNotSupported", resourceCulture);

		internal static string Socks5_CommandReplyConnectionNotAllowedByRuleset => ResourceManager.GetString("Socks5_CommandReplyConnectionNotAllowedByRuleset", resourceCulture);

		internal static string Socks5_CommandReplyConnectionRefused => ResourceManager.GetString("Socks5_CommandReplyConnectionRefused", resourceCulture);

		internal static string Socks5_CommandReplyGeneralSocksServerFailure => ResourceManager.GetString("Socks5_CommandReplyGeneralSocksServerFailure", resourceCulture);

		internal static string Socks5_CommandReplyHostUnreachable => ResourceManager.GetString("Socks5_CommandReplyHostUnreachable", resourceCulture);

		internal static string Socks5_CommandReplyNetworkUnreachable => ResourceManager.GetString("Socks5_CommandReplyNetworkUnreachable", resourceCulture);

		internal static string Socks5_CommandReplyTTLExpired => ResourceManager.GetString("Socks5_CommandReplyTTLExpired", resourceCulture);

		internal Resources()
		{
		}
	}
}
