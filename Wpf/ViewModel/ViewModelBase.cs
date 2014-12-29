using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Addle.Wpf.ViewModel
{
	public class ViewModelBase : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		void OnPropertyChanged(string propertyName)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
		}

		protected void SetProperty<T>(ref T property, T value, Action<T> callback = null, [CallerMemberName] string name = null)
		{
			SetProperty(name, ref property, value, callback);
		}

		protected void SetProperty<T>(string name, ref T property, T value, Action<T> callback = null)
		{
			if (Equals(property, value)) return;

			property = value;

			if (callback != null)
			{
				callback(value);
			}

			OnPropertyChanged(name);
		}
	}
}
