using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using Addle.Core.Linq;
using JetBrains.Annotations;
using Microsoft.CSharp;

namespace Addle.Wpf.ViewModel
{
	public static class AutoVMFactory
	{
		static readonly Dictionary<Type, IDictionary<string, FieldDescription>> _typeFieldDescriptions = new Dictionary<Type, IDictionary<string, FieldDescription>>();
		static int _randomIncrement;

		public static IViewModel<T> Create<T>()
			where T : class, new()
		{
			return Wrap(new T());
		}

		public static IViewModel<T> Wrap<T>(T vmToWrap)
			where T : class
		{
			var vmType = typeof(T);
			//TODO: need to perform more verification on vmType
			if (vmType.IsNotPublic) throw new ArgumentException("vmToWrap must be public", "vmToWrap");

			var fieldDescriptions = _typeFieldDescriptions.TryGetValue(typeof(T));

			if (fieldDescriptions == null)
			{
				var fieldDescriptionValues =
					from fieldInfo in typeof(T).GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
					let attr = fieldInfo.GetCustomAttribute<VMPropertyAttribute>()
					where attr != null
					let trimmed = fieldInfo.Name.TrimStart('_')
					let propertyName = trimmed.Substring(0, 1).ToUpperInvariant() + trimmed.Substring(1)
					select new FieldDescription(propertyName, fieldInfo, attr);
				fieldDescriptions = fieldDescriptionValues.ToDictionary(a => a.PropertyName, a => a);
				_typeFieldDescriptions[typeof(T)] = fieldDescriptions;
			}

			var result = Generate(vmToWrap, fieldDescriptions.Values);

			// ReSharper disable once SuspiciousTypeConversion.Global
			var generated = (IGeneratedViewModel)result;
			var valueProvider = new ValueProvider(fieldDescriptions, vmToWrap);
			SetupGenerated(generated, vmToWrap, valueProvider, fieldDescriptions.Values);

			return result;
		}

		static IViewModel<T> Generate<T>(T vmToWrap, IEnumerable<FieldDescription> values)
		{
			var increment = Interlocked.Increment(ref _randomIncrement);
			var random = new Random();
			var className = "Gen_{0}_{1}_{2}".FormatWith(typeof(T).Name, random.Next(0, int.MaxValue), increment);

			var generator = new AutoVMGenerator(className, typeof(T).FullName, values);
			var text = generator.TransformText();

			var codeProvider = new CSharpCodeProvider();
			var compilerParameters = new CompilerParameters
			{
				GenerateInMemory = true,
#if DEBUG
				IncludeDebugInformation = true,
#endif
			};

			var assembliesToAdd = new List<string>
				{
					"Microsoft.CSharp.dll",
					"System.dll",
					"System.Core.dll",
					typeof(AutoVMFactory).Assembly.Location,
					typeof(EnumerableExtensions).Assembly.Location,
					typeof(T).Assembly.Location
				};
			compilerParameters.ReferencedAssemblies.AddRange(assembliesToAdd.ToArray());

			var results = codeProvider.CompileAssemblyFromSource(compilerParameters, text);

			if (results.Errors.Count > 0)
			{
				throw new Exception(results.Errors[0].ToString());
			}

			var generatedType = results.CompiledAssembly.GetTypes().Single(a => a.Name.Equals(className));

			var constructor = generatedType.GetConstructors().Single();
			Debug.Assert(constructor != null, "Constructor not found for {0}({1})".FormatWith(generatedType.Name, typeof(T).Name));

			var result = constructor.Invoke(new object[] { vmToWrap });
			return (IViewModel<T>)result;
		}

		static void SetupGenerated<T>(IGeneratedViewModel generated, T vmToWrap, ValueProvider valueProvider, ICollection<FieldDescription> fieldDescriptions)
		{
			var autoCommands = (
				from fieldInfo in fieldDescriptions
				where fieldInfo.IsAutoCommand
				let autoField = fieldInfo.FieldInfo.GetValue(vmToWrap)
				select new { AutoField = (IAutoCommandInternal)autoField, fieldInfo.PropertyName }
				).ToList();

			var autoProperties = (
				from fieldInfo in fieldDescriptions
				where fieldInfo.IsAutoProperty
				let autoField = fieldInfo.FieldInfo.GetValue(vmToWrap)
				select new { AutoProperty = (IAutoPropertyInternal)autoField, fieldInfo.PropertyName }
				).ToList();

			// set the owners for all the autoProperties so that the autoProperties can callback their parents
			autoCommands.ForEach(a => a.AutoField.Setup(vmToWrap, a.PropertyName));
			autoProperties.ForEach(a => a.AutoProperty.Setup(vmToWrap, a.PropertyName));

			// now that everything's set up on the properties, call the changed callback so each property gets at least one changed call
			autoCommands.ForEach(a => a.AutoField.Initialized());
			autoProperties.ForEach(a => a.AutoProperty.Initialized());

			generated.Initialize(valueProvider, autoProperties.Select(a => a.AutoProperty));
		}

		#region class ValueProvider

		sealed class ValueProvider : IAutoVMFactoryValueProvider
		{
			readonly IDictionary<string, FieldDescription> _fieldDescriptions;
			readonly object _instance;

			public ValueProvider(IDictionary<string, FieldDescription> fieldDescriptions, object instance)
			{
				_fieldDescriptions = fieldDescriptions;
				_instance = instance;
			}

			T IAutoVMFactoryValueProvider.GetValue<T>(string fieldName)
			{
				var result = default(T);
				var fieldDescription = _fieldDescriptions.TryGetValue(fieldName);

				if (fieldDescription != null)
				{
					if (fieldDescription.IsAutoProperty)
					{
						var autoProperty = (IAutoPropertyInternal)fieldDescription.FieldInfo.GetValue(_instance);
						result = (T)autoProperty.GetValue();
					}
					else
					{
						result = (T)fieldDescription.FieldInfo.GetValue(_instance);
					}
				}

				return result;
			}

			void IAutoVMFactoryValueProvider.SetValue<T>(T value, string fieldName)
			{
				var fieldDescription = _fieldDescriptions.TryGetValue(fieldName);

				if (fieldDescription != null)
				{
					if (fieldDescription.Attribute.IsWritable)
					{
						if (fieldDescription.IsAutoProperty)
						{
							var autoProperty = (IAutoPropertyInternal)fieldDescription.FieldInfo.GetValue(_instance);
							autoProperty.SetValue(value);
						}
						else
						{
							fieldDescription.FieldInfo.SetValue(_instance, value);
						}
					}
				}
			}
		}

		#endregion
	}

	#region class FieldDescription

	sealed class FieldDescription
	{
		public FieldDescription(string propertyName, FieldInfo fieldInfo, VMPropertyAttribute attribute)
		{
			FieldInfo = fieldInfo;
			Attribute = attribute;
			IsAutoCommand = typeof(IAutoCommandInternal).IsAssignableFrom(fieldInfo.FieldType);
			IsAutoProperty = typeof(IAutoPropertyInternal).IsAssignableFrom(fieldInfo.FieldType);
			PropertyName = propertyName;
		}

		public string PropertyName { get; private set; }
		public FieldInfo FieldInfo { get; private set; }
		public VMPropertyAttribute Attribute { get; private set; }
		public bool IsAutoCommand { get; private set; }
		public bool IsAutoProperty { get; private set; }
	}

	#endregion

	public interface IAutoVMFactoryValueProvider
	{
		T GetValue<T>([CallerMemberName] string fieldName = null);
		void SetValue<T>(T value, [CallerMemberName] string fieldName = null);
	}

	public interface IGeneratedViewModel
	{
		void Initialize(IAutoVMFactoryValueProvider valueProvider, IEnumerable<INotifyPropertyChanged> notifiers);
	}
}
