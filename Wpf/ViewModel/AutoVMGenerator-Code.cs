using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Addle.Core.Linq;
using JetBrains.Annotations;

namespace Addle.Wpf.ViewModel
{
	partial class AutoVMGenerator
	{
		readonly string _className;
		readonly string _wrappedClassName;
		readonly IEnumerable<FieldDescription> _fieldDescriptions;
		readonly bool _isDesignTime;

		public AutoVMGenerator(string className, string wrappedClassName, IEnumerable<FieldDescription> fieldDescriptions, bool isDesignTime)
		{
			_className = className;
			_wrappedClassName = wrappedClassName;
			_fieldDescriptions = fieldDescriptions;
			_isDesignTime = isDesignTime;
		}

		static Type GetPropertyType(FieldDescription fieldDescription)
		{
			Type result;

			if (fieldDescription.IsAutoProperty)
			{
				result = fieldDescription.FieldInfo.FieldType.GenericTypeArguments.Last();
			}
			else if (fieldDescription.IsAutoCommand)
			{
				result = typeof(ICommand);
			}
			else
			{
				result = fieldDescription.FieldInfo.FieldType;
            }

			return result;
		}

		static string ConvertTypeNameToString(Type type)
		{
			string result;

            if (type.IsGenericType)
			{
				var baseName = type.GetGenericTypeDefinition().FullName.Substring(0, type.GetGenericTypeDefinition().FullName.IndexOf('`'));
                var args = type.GetGenericArguments().Select(ConvertTypeNameToString);
				result = "{0}<{1}>".FormatWith(baseName, string.Join(", ", args));
			}
			else
			{
				result = type.FullName;
			}

			return result;
		}
	}
}
