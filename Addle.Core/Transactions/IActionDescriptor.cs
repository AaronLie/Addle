using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Addle.Core.Linq;
using JetBrains.Annotations;

namespace Addle.Core.Transactions
{
	public interface IActionDescriptor
	{
		void Do();
		void Undo();
	}
}
