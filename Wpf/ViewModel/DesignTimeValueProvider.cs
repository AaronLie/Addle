using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Addle.Wpf.ViewModel
{
	public class DesignTimeValueProvider : IAutoVMFactoryValueProvider
	{
		T IAutoVMFactoryValueProvider.GetValue<T>(string fieldName)
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

		void IAutoVMFactoryValueProvider.SetValue<T>(T value, string fieldName)
		{
		}
	}
}
