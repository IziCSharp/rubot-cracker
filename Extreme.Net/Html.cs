using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Extreme.Net
{
	public static class Html
	{
		private static readonly Dictionary<string, string> _htmlMnemonics = new Dictionary<string, string>
		{
			{
				"apos",
				"'"
			},
			{
				"quot",
				"\""
			},
			{
				"amp",
				"&"
			},
			{
				"lt",
				"<"
			},
			{
				"gt",
				">"
			}
		};

		public static string ReplaceEntities(this string str)
		{
			if (string.IsNullOrEmpty(str))
			{
				return string.Empty;
			}
			Regex regex = new Regex("(\\&(?<text>\\w{1,4})\\;)|(\\&#(?<code>\\w{1,4})\\;)", RegexOptions.Compiled);
			return regex.Replace(str, delegate(Match match)
			{
				if (match.Groups["text"].Success)
				{
					if (_htmlMnemonics.TryGetValue(match.Groups["text"].Value, out string value))
					{
						return value;
					}
				}
				else if (match.Groups["code"].Success)
				{
					int num = int.Parse(match.Groups["code"].Value);
					return ((char)(ushort)num).ToString();
				}
				return match.Value;
			});
		}

		public static string ReplaceUnicode(this string str)
		{
			if (string.IsNullOrEmpty(str))
			{
				return string.Empty;
			}
			Regex regex = new Regex("\\\\u(?<code>[0-9a-f]{4})", RegexOptions.IgnoreCase | RegexOptions.Compiled);
			return regex.Replace(str, delegate(Match match)
			{
				int num = int.Parse(match.Groups["code"].Value, NumberStyles.HexNumber);
				return ((char)(ushort)num).ToString();
			});
		}

		public static string Substring(this string str, string left, int startIndex, StringComparison comparsion = StringComparison.Ordinal)
		{
			if (string.IsNullOrEmpty(str))
			{
				return string.Empty;
			}
			if (left == null)
			{
				throw new ArgumentNullException("left");
			}
			if (left.Length == 0)
			{
				throw ExceptionHelper.EmptyString("left");
			}
			if (startIndex < 0)
			{
				throw ExceptionHelper.CanNotBeLess("startIndex", 0);
			}
			if (startIndex >= str.Length)
			{
				throw new ArgumentOutOfRangeException("startIndex", Resources.ArgumentOutOfRangeException_StringHelper_MoreLengthString);
			}
			int num = str.IndexOf(left, startIndex, comparsion);
			if (num == -1)
			{
				return string.Empty;
			}
			int num2 = num + left.Length;
			int length = str.Length - num2;
			return str.Substring(num2, length);
		}

		public static string Substring(this string str, string left, StringComparison comparsion = StringComparison.Ordinal)
		{
			return str.Substring(left, 0, comparsion);
		}

		public static string ClearSteamSpecialChars(this string str)
		{
			string[] array = new string[8]
			{
				"™",
				"★",
				"☆",
				"★",
				"\\u2605",
				"\\u2122",
				"™",
				"★"
			};
			string[] array2 = array;
			foreach (string oldValue in array2)
			{
				str = str.Replace(oldValue, "");
			}
			return str;
		}

		public static string Substring(this string str, string left, string right, int startIndex, StringComparison comparsion = StringComparison.Ordinal)
		{
			if (string.IsNullOrEmpty(str))
			{
				return string.Empty;
			}
			if (left == null)
			{
				throw new ArgumentNullException("left");
			}
			if (left.Length == 0)
			{
				throw ExceptionHelper.EmptyString("left");
			}
			if (right == null)
			{
				throw new ArgumentNullException("right");
			}
			if (right.Length == 0)
			{
				throw ExceptionHelper.EmptyString("right");
			}
			if (startIndex < 0)
			{
				throw ExceptionHelper.CanNotBeLess("startIndex", 0);
			}
			if (startIndex >= str.Length)
			{
				throw new ArgumentOutOfRangeException("startIndex", Resources.ArgumentOutOfRangeException_StringHelper_MoreLengthString);
			}
			int num = str.IndexOf(left, startIndex, comparsion);
			if (num == -1)
			{
				return string.Empty;
			}
			int num2 = num + left.Length;
			int num3 = str.IndexOf(right, num2, comparsion);
			if (num3 == -1)
			{
				return string.Empty;
			}
			int length = num3 - num2;
			return str.Substring(num2, length);
		}

		public static string Substring(this string str, string left, string right, StringComparison comparsion = StringComparison.Ordinal)
		{
			return str.Substring(left, right, 0, comparsion);
		}

		public static string LastSubstring(this string str, string left, int startIndex, StringComparison comparsion = StringComparison.Ordinal)
		{
			if (string.IsNullOrEmpty(str))
			{
				return string.Empty;
			}
			if (left == null)
			{
				throw new ArgumentNullException("left");
			}
			if (left.Length == 0)
			{
				throw ExceptionHelper.EmptyString("left");
			}
			if (startIndex < 0)
			{
				throw ExceptionHelper.CanNotBeLess("startIndex", 0);
			}
			if (startIndex >= str.Length)
			{
				throw new ArgumentOutOfRangeException("startIndex", Resources.ArgumentOutOfRangeException_StringHelper_MoreLengthString);
			}
			int num = str.LastIndexOf(left, startIndex, comparsion);
			if (num == -1)
			{
				return string.Empty;
			}
			int num2 = num + left.Length;
			int length = str.Length - num2;
			return str.Substring(num2, length);
		}

		public static string LastSubstring(this string str, string left, StringComparison comparsion = StringComparison.Ordinal)
		{
			if (string.IsNullOrEmpty(str))
			{
				return string.Empty;
			}
			return str.LastSubstring(left, str.Length - 1, comparsion);
		}

		public static string LastSubstring(this string str, string left, string right, int startIndex, StringComparison comparsion = StringComparison.Ordinal)
		{
			if (string.IsNullOrEmpty(str))
			{
				return string.Empty;
			}
			if (left == null)
			{
				throw new ArgumentNullException("left");
			}
			if (left.Length == 0)
			{
				throw ExceptionHelper.EmptyString("left");
			}
			if (right == null)
			{
				throw new ArgumentNullException("right");
			}
			if (right.Length == 0)
			{
				throw ExceptionHelper.EmptyString("right");
			}
			if (startIndex < 0)
			{
				throw ExceptionHelper.CanNotBeLess("startIndex", 0);
			}
			if (startIndex >= str.Length)
			{
				throw new ArgumentOutOfRangeException("startIndex", Resources.ArgumentOutOfRangeException_StringHelper_MoreLengthString);
			}
			int num = str.LastIndexOf(left, startIndex, comparsion);
			if (num == -1)
			{
				return string.Empty;
			}
			int num2 = num + left.Length;
			int num3 = str.IndexOf(right, num2, comparsion);
			if (num3 == -1)
			{
				if (num == 0)
				{
					return string.Empty;
				}
				return str.LastSubstring(left, right, num - 1, comparsion);
			}
			int length = num3 - num2;
			return str.Substring(num2, length);
		}

		public static string LastSubstring(this string str, string left, string right, StringComparison comparsion = StringComparison.Ordinal)
		{
			if (string.IsNullOrEmpty(str))
			{
				return string.Empty;
			}
			return str.LastSubstring(left, right, str.Length - 1, comparsion);
		}

		public static string[] Substrings(this string str, string left, string right, int startIndex, StringComparison comparsion = StringComparison.Ordinal)
		{
			if (string.IsNullOrEmpty(str))
			{
				return new string[0];
			}
			if (left == null)
			{
				throw new ArgumentNullException("left");
			}
			if (left.Length == 0)
			{
				throw ExceptionHelper.EmptyString("left");
			}
			if (right == null)
			{
				throw new ArgumentNullException("right");
			}
			if (right.Length == 0)
			{
				throw ExceptionHelper.EmptyString("right");
			}
			if (startIndex < 0)
			{
				throw ExceptionHelper.CanNotBeLess("startIndex", 0);
			}
			if (startIndex >= str.Length)
			{
				throw new ArgumentOutOfRangeException("startIndex", Resources.ArgumentOutOfRangeException_StringHelper_MoreLengthString);
			}
			int startIndex2 = startIndex;
			List<string> list = new List<string>();
			while (true)
			{
				int num = str.IndexOf(left, startIndex2, comparsion);
				if (num == -1)
				{
					break;
				}
				int num2 = num + left.Length;
				int num3 = str.IndexOf(right, num2, comparsion);
				if (num3 == -1)
				{
					break;
				}
				int length = num3 - num2;
				list.Add(str.Substring(num2, length));
				startIndex2 = num3 + right.Length;
			}
			return list.ToArray();
		}

		public static string[] Substrings(this string str, string left, string right, StringComparison comparsion = StringComparison.Ordinal)
		{
			return str.Substrings(left, right, 0, comparsion);
		}
	}
}
