using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Addle.Core.Linq;
using JetBrains.Annotations;

namespace Addle.Core.Linq
{
	public static class DisposableOperations
	{
		public static void DisposeAndNullOut(ref IDisposable disposable)
		{
			disposable.Dispose();
			disposable = null;
		}

		public static void SafeDisposeAndNullOut(ref IDisposable disposable)
		{
			disposable?.Dispose();
			disposable = null;
		}
	}
}
