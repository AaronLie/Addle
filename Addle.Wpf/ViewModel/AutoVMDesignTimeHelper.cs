//#define DEBUG_AUTOVMFACTORY_DESIGNTIMELOG

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
			else if (typeof(T).IsGenericType && typeof(T).GetGenericTypeDefinition() == typeof(IViewModel<>))
			{
				var vmType = typeof(T).GetGenericArguments().Single();
				var generatedType = AutoVMFactory.MakeTypeForDesignTime(vmType);

				result = (T)ConstructIfPossible(generatedType);
			}
			else if (typeof(T).IsClass)
			{
				result = (T)ConstructIfPossible(typeof(T));
			}

			return result;
		}

		static object ConstructIfPossible(Type type)
		{
			object result = null;
			var constructor = type.GetConstructor(Type.EmptyTypes);

			if (constructor != null)
			{
				result = constructor.Invoke(null);
			}

			return result;
		}

		public static TResult GetDesignTimeOverride<TClass, TResult>(string fieldName)
		{
			var type = typeof(TClass);
			var result = default(TResult);
			var field = type.GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);

			if (field != null)
			{
				var attribute = field.GetCustomAttribute<VMPropertyAttribute>();

				if (attribute?.DesignTime != null)
				{
					// ReSharper disable once CanBeReplacedWithTryCastAndCheckForNull
					if (attribute.DesignTime is TResult)
					{
						result = (TResult)attribute.DesignTime;
					}
					else
					{
						try
						{
							result = (TResult)Convert.ChangeType(attribute.DesignTime, typeof(TResult));
						}
						catch
						{
							ReportError("Could not convert DesignTime for {0}.{1} from {2} to field type {3}.", type.FullName, fieldName, attribute.DesignTime.GetType().FullName, typeof(TResult).FullName);
						}
					}
				}
				else
				{
					ReportError("Field {0}.{1} didn't have VMPropertyAttribute.DesignTime set.", type.FullName, fieldName);
				}
			}
			else
			{
				ReportError("Could not find field {0}.{1}", type.FullName, fieldName);
			}

			return result;
		}

		[StringFormatMethod("message")]
		// ReSharper disable UnusedParameter.Local
		static void ReportError(string message, params object[] args)
		// ReSharper restore UnusedParameter.Local
		{
#if DEBUG_AUTOVMFACTORY_DESIGNTIMELOG
			var tempFile = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "autovmfactory-designtimeerrors.txt");
            System.IO.File.AppendAllLines(tempFile, new[] { string.Format(message, args) });
#endif
		}
	}
}
