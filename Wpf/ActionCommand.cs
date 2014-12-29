using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Windows.Input;

namespace Addle.Wpf
{
	public class ActionCommand : ICommand
	{
		readonly Action _execute;
		readonly Func<bool> _canExecute;

		public ActionCommand(Action execute, Func<bool> canExecute = null, IObservable<Unit> canExecuteChanged = null)
		{
			if (_execute != null) throw new ArgumentNullException("execute");
			_execute = execute;
			_canExecute = canExecute;

			if (canExecuteChanged != null)
			{
				canExecuteChanged.Subscribe(_ => OnCanExecuteChanged());
			}
		}

		public bool CanExecute(object parameter)
		{
			return _canExecute == null || _canExecute();
		}

		public void Execute(object parameter)
		{
			_execute();
		}

		public event EventHandler CanExecuteChanged;

		protected void OnCanExecuteChanged()
		{
			var handler = CanExecuteChanged;
			if (handler != null) handler(this, new EventArgs());
		}
	}
}
