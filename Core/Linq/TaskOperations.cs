using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Addle.Core.Linq;
using JetBrains.Annotations;

namespace Addle.Core.Linq
{
	public static class TaskOperations
	{
		public static T WaitAndGet<T>(this Task<T> task)
		{
			task.Wait();
			return task.Result;
		}
	}
}
