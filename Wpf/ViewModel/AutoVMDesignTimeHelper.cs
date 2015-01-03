using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace Addle.Wpf.ViewModel
{
	public static class AutoVMDesignTimeHelper
	{
		public static Type GetDesignTimeType(Type type)
		{
			var result = type;

			if (typeof(IEnumerable<string>).IsAssignableFrom(type))
			{
				result = typeof(IEnumerable<string>);
			}

			return result;
		}

		public static T GetDesignTimeValue<T>(string fieldName)
		{
			var result = default(T);

			if (typeof(string) == typeof(T))
			{
				result = (T)Convert.ChangeType(string.Format("[{0}]", fieldName), typeof(T));
			}
			else if (typeof(int) == typeof(T))
			{
				result = (T)Convert.ChangeType(3, typeof(T));
			}
			else if (typeof(IEnumerable<string>).IsAssignableFrom(typeof(T)))
			{
				result = (T)(object)new[] { "hi", "bye", "how" };
			}

			return result;
		}
	}
}
