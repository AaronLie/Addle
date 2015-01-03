using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Addle.Core.Linq;
using JetBrains.Annotations;

namespace Addle.Wpf.ViewModel
{
	public interface IAutoProperty<T>
	{
		T Value { get; set; }
	}

	public class AutoProperty<T> : IAutoProperty<T>, IAutoPropertyInternal
	{
		event PropertyChangedEventHandler _propertyChanged;
		T _value;
		string _propertyName;

		public AutoProperty(T value = default(T))
		{
			_value = value;
		}

		void IAutoPropertyInternal.Setup(object owner, string propertyName)
		{
			_propertyName = propertyName;
		}

		object IAutoPropertyInternal.GetValue() { return Value; }
		void IAutoPropertyInternal.SetValue(object value) { Value = (T)Convert.ChangeType(value, typeof(T)); }
		void IAutoPropertyInternal.Initialized() { }

		event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged { add { _propertyChanged += value; } remove { _propertyChanged -= value; } }

		public T Value
		{
			get { return _value; }
			set { SetProperty(ref _value, value); }
		}

		void OnPropertyChanged(string propertyName)
		{
			_propertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		void SetProperty(ref T property, T value)
		{
			if (Equals(property, value)) return;
			property = value;
			OnPropertyChanged(_propertyName);
		}
	}

	public class AutoProperty<TOwner, T> : IAutoProperty<T>, IAutoPropertyInternal
	{
		readonly Action<TOwner, T> _valueChangedCallback;
		event PropertyChangedEventHandler _propertyChanged;
		T _value;
		object _owner;
		string _propertyName;

		public AutoProperty(T value = default(T), Action<TOwner, T> valueChangedCallback = null)
		{
			_value = value;
			_valueChangedCallback = valueChangedCallback;
		}

		public AutoProperty(T value = default(T), Action<TOwner> valueChangedCallback = null)
		{
			_value = value;
			_valueChangedCallback = (owner, _) => valueChangedCallback(owner);
		}

		void IAutoPropertyInternal.Setup(object owner, string propertyName)
		{
			_owner = owner;
			_propertyName = propertyName;
		}

		object IAutoPropertyInternal.GetValue() { return Value; }
		void IAutoPropertyInternal.SetValue(object value) { Value = (T)Convert.ChangeType(value, typeof(T)); }

		void IAutoPropertyInternal.Initialized()
		{
			if (_valueChangedCallback != null)
			{
				_valueChangedCallback((TOwner)_owner, _value);
				OnPropertyChanged(_propertyName);
			}
		}

		event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged { add { _propertyChanged += value; } remove { _propertyChanged -= value; } }

		public T Value
		{
			get { return _value; }
			set { SetProperty(ref _value, value, _valueChangedCallback); }
		}

		void OnPropertyChanged(string propertyName)
		{
			_propertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		void SetProperty(ref T property, T value, Action<TOwner, T> callback = null)
		{
			if (Equals(property, value)) return;

			property = value;

			callback?.Invoke((TOwner)_owner, value);

			OnPropertyChanged(_propertyName);
		}
	}
}
