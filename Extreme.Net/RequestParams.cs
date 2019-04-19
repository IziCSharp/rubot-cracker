using System;
using System.Collections.Generic;

namespace Extreme.Net
{
	internal class RequestParams : List<KeyValuePair<string, string>>
	{
		public object this[string paramName]
		{
			set
			{
				if (paramName == null)
				{
					throw new ArgumentNullException("paramName");
				}
				if (paramName.Length == 0)
				{
					throw ExceptionHelper.EmptyString("paramName");
				}
				string value2 = (value == null) ? string.Empty : value.ToString();
				Add(new KeyValuePair<string, string>(paramName, value2));
			}
		}
	}
}
