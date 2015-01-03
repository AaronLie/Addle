using System;
using System.Collections.Generic;
using System.Linq;
using Addle.Core.Linq;
using JetBrains.Annotations;

namespace Addle.Wpf.ViewModel
{
	[MeansImplicitUse]
	public class VMPropertyAttribute : Attribute
	{
		/// <summary>If this is true, then a property setter is also generated. Default is false.</summary>
		public bool IsWritable { get; set; }

		/// <summary>Override the value that's used at design time.</summary>
		public object DesignTime { get; set; }
	}
}
