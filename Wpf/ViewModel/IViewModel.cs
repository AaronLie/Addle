using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Addle.Core.Linq;
using JetBrains.Annotations;

namespace Addle.Wpf.ViewModel
{
	public interface IViewModel<out T> : INotifyPropertyChanged
	{
		T Value { get; }
	}
}
