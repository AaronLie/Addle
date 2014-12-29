using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Addle.Wpf.Linq
{
	public static class DispatcherExtensions
	{
		public static void BeginInvoke(this Dispatcher dispatcher, Action action, DispatcherPriority dispatcherPriority = DispatcherPriority.Normal)
		{
			dispatcher.BeginInvoke(action, dispatcherPriority);
		}

		public static Task BeginInvokeAsync(this Dispatcher dispatcher, Action action, DispatcherPriority dispatcherPriority = DispatcherPriority.Normal)
		{
			return dispatcher.BeginInvoke(action, dispatcherPriority).Task;
		}
	}
}
