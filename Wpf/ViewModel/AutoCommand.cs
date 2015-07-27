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
	public interface IAutoCommand
	{
		BehaviorSubject<bool> CanExecute { get; }
	}

    public class AutoCommand<TOwner, TParam> : IAutoCommand, IAutoCommandInternal, ICommand
	{
		readonly Action<TOwner, TParam> _executeCallback;
	    event EventHandler _canExecuteChanged;
		object _owner;

		public AutoCommand(Action<TOwner, TParam> executeCallback)
		{
			_executeCallback = executeCallback;
			CanExecute.Subscribe(OnCanExecuteChanged);
		}

		public AutoCommand(Action<TOwner> executeCallback)
			: this((owner, _) => executeCallback(owner))
		{
		}

		public BehaviorSubject<bool> CanExecute { get; } = new BehaviorSubject<bool>(true);

	    void IAutoCommandInternal.Setup(object owner, string propertyName)
		{
			_owner = owner;
		}

		void IAutoCommandInternal.Initialized()
		{
		}

		bool ICommand.CanExecute(object parameter)
		{
			return CanExecute == null || CanExecute.Value;
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
