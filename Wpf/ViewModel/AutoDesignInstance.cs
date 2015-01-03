using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace Addle.Wpf.ViewModel
{
	public class AutoDesignInstance : MarkupExtension
	{
		public Type Type { get; set; }

		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			if (Type == null) return null;

			return AutoVMFactory.MakeTypeForDesignTime(Type);
		}
	}
}
