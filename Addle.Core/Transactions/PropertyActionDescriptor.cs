using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Addle.Core.Linq;
using JetBrains.Annotations;

namespace Addle.Core.Transactions
{
	public class PropertyActionDescriptor<T> : IActionDescriptor
	{
		readonly Func<T> _getValue;
		readonly Action<T> _setValue;
		readonly Action _propertyChanged;
		T _storedValue;

		public PropertyActionDescriptor(Func<T> getValue, Action<T> setValue, T newValue, Action propertyChanged = null)
		{
			_getValue = getValue;
			_setValue = setValue;
			_storedValue = newValue;
			_propertyChanged = propertyChanged;
		}

		void IActionDescriptor.Do()
		{
			// this is essentially a swap between storedValue and the state of the property we're talking to
			var oldValue = _getValue();
			_setValue(_storedValue);
			_storedValue = oldValue;

			_propertyChanged?.Invoke();
		}

		void IActionDescriptor.Undo()
		{
			// same implementation as Do()
			var oldValue = _getValue();
			_setValue(_storedValue);
			_storedValue = oldValue;

			_propertyChanged?.Invoke();
		}
	}
}
