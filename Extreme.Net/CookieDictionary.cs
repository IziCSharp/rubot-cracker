using System;
using System.Collections.Generic;
using System.Text;

namespace Extreme.Net
{
	internal class CookieDictionary : Dictionary<string, string>
	{
		public bool IsLocked
		{
			get;
			set;
		}

		public CookieDictionary(bool isLocked = false)
			: base((IEqualityComparer<string>)StringComparer.OrdinalIgnoreCase)
		{
			IsLocked = isLocked;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			using (Enumerator enumerator = GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					KeyValuePair<string, string> current = enumerator.Current;
					stringBuilder.AppendFormat("{0}={1}; ", current.Key, current.Value);
				}
			}
			if (stringBuilder.Length > 0)
			{
				stringBuilder.Remove(stringBuilder.Length - 2, 2);
			}
			return stringBuilder.ToString();
		}
	}
}
