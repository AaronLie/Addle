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
		public VMPropertyAttribute()
		{
		}

		/// <summary>If this is true, then a property setter is also generated. Default is false.</summary>
		public bool IsWritable { get; set; }

		/// <summary></summary>
		public Type OverrideType { get; set; }
	}
}
