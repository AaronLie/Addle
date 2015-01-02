using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Addle.Core.Linq;
using JetBrains.Annotations;
using Addle.Wpf.ViewModel;

namespace Addle.AutoVMTest
{
	public class MainWindowVM
	{
		[VMProperty]
		readonly AutoProperty<string> _name = new AutoProperty<string>("Woohoo!");

		[VMProperty(IsWritable = true)]
		readonly AutoProperty<MainWindowVM, int> _count = new AutoProperty<MainWindowVM, int>(3, (@this, value) =>
		{
			@this._strings.SetRange(Enumerable.Range(0, value).Select(a => "foo-" + a));
			@this._removeItemCommand.CanExecute.OnNext(@this._strings.Count > 0);
		});

		[VMProperty(OverrideType = typeof(IEnumerable<string>))]
		readonly ObservableCollection<string> _strings = new ObservableCollection<string>();

		[VMProperty]
		readonly AutoCommand<MainWindowVM, string> _removeItemCommand = new AutoCommand<MainWindowVM, string>(@this =>
		{
			@this._strings.RemoveAt(0);
			@this._removeItemCommand.CanExecute.OnNext(@this._strings.Count > 0);
		});
	}
}
