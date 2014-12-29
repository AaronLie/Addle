using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Subjects;
using System.Windows.Input;
using Addle.Core.Linq;
using JetBrains.Annotations;

namespace Addle.Wpf.ViewModel
{
	public class AutoCommand<TOwner, TParam> : IAutoCommandInternal, ICommand
	{
		readonly Action<TOwner, TParam> _executeCallback;
		readonly BehaviorSubject<bool> _canExecute = new BehaviorSubject<bool>(true);
		event EventHandler _canExecuteChanged;
		object _owner;

		public AutoCommand(Action<TOwner, TParam> executeCallback)
		{
			_executeCallback = executeCallback;
			_canExecute.Subscribe(OnCanExecuteChanged);
		}

		public AutoCommand(Action<TOwner> executeCallback)
			: this((owner, _) => executeCallback(owner))
		{
		}

		public BehaviorSubject<bool> CanExecute { get { return _canExecute; } }

		void IAutoCommandInternal.Setup(object owner, string propertyName)
		{
			_owner = owner;
		}

		void IAutoCommandInternal.Initialized()
		{
		}

		bool ICommand.CanExecute(object parameter)
		{
			return _canExecute == null || _canExecute.Value;
		}

		void ICommand.Execute(object parameter)
		{
			_executeCallback((TOwner)_owner, (TParam)parameter);
		}

		void OnCanExecuteChanged(bool value)
		{
			_canExecuteChanged?.Invoke(this, new EventArgs());
		}

		event EventHandler ICommand.CanExecuteChanged { add { _canExecuteChanged += value; } remove { _canExecuteChanged -= value; } }
	}

	public class AutoCommand<TOwner> : AutoCommand<TOwner, Unit>
	{
		public AutoCommand(Action<TOwner> executeCallback)
			: base((owner, param) => executeCallback(owner))
		{
		}
	}
}
