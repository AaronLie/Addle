using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Addle.Core.Linq;
using JetBrains.Annotations;

namespace Addle.Wpf.ViewModel
{
	interface IAutoCommandInternal
	{
		void Setup(object owner, string propertyName);
		void Initialized();
	}

	interface IAutoPropertyInternal : INotifyPropertyChanged
	{
		void Setup(object owner, string propertyName);
		void Initialized();

		object GetValue();
		void SetValue(object value);
	}
}
