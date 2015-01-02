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

		public AutoVMGenerator(string className, string wrappedClassName, IEnumerable<FieldDescription> fieldDescriptions)
		{
			_className = className;
			_wrappedClassName = wrappedClassName;
			_fieldDescriptions = fieldDescriptions;
		}

		static string GetPropertyType(FieldDescription fieldDescription)
		{
			var type = fieldDescription.Attribute.OverrideType;

			if (type == null)
			{
				if (fieldDescription.IsAutoProperty)
				{
					type = fieldDescription.FieldInfo.FieldType.GenericTypeArguments.Last();
				}
				else if (fieldDescription.IsAutoCommand)
				{
					type = typeof(ICommand);
				}
				else
				{
					type = fieldDescription.FieldInfo.FieldType;
                }
			}

			var result = ConvertGenericTypeName(type);
			return result;
		}

		static string ConvertGenericTypeName(Type type)
		{
			string result;

            if (type.IsGenericType)
			{
				var baseName = type.GetGenericTypeDefinition().FullName.Substring(0, type.GetGenericTypeDefinition().FullName.IndexOf('`'));
                var args = type.GetGenericArguments().Select(ConvertGenericTypeName);
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
