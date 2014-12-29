using System;
using System.Collections.Generic;
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

			var instance = Type.GetConstructor(Type.EmptyTypes).Invoke(null);

			var method = typeof(AutoVMFactory).GetMethod("Wrap");
			method = method.MakeGenericMethod(Type);

			var generated = method.Invoke(null, new[] { instance });
			return generated;
		}
	}
}
