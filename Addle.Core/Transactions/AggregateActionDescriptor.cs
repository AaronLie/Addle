using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Addle.Core.Linq;
using JetBrains.Annotations;

namespace Addle.Core.Transactions
{
	public class AggregateActionDescriptor : IActionDescriptor
	{
		readonly IActionDescriptor[] _descriptors;

		public AggregateActionDescriptor(params IActionDescriptor[] descriptors)
		{
			_descriptors = descriptors;
		}

		void IActionDescriptor.Do()
		{
			_descriptors.ForEach(descriptor => descriptor.Do());
		}

		void IActionDescriptor.Undo()
		{
			_descriptors.Reverse().ForEach(descriptor => descriptor.Undo());
		}
	}
}
