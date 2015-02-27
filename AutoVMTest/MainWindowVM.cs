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

		[VMProperty(IsWritable = true, DesignTime = 4)]
		readonly AutoProperty<MainWindowVM, int> _count = new AutoProperty<MainWindowVM, int>(3, (@this, value) =>
		{
			@this._left.Value.SetRange(Enumerable.Range(0, value).Select(a => "foo-" + a));
			@this._right.Value.Clear();
			@this._left.Value.UpdateCanExecute();
			@this._right.Value.UpdateCanExecute();
		});

		[VMProperty]
		readonly IViewModel<ItemListVM> _left = AutoVMFactory.Create<ItemListVM>();

		[VMProperty]
		readonly IViewModel<ItemListVM> _right = AutoVMFactory.Create<ItemListVM>();

		public MainWindowVM()
		{
			_left.Value.ItemMoved.Subscribe(item =>_right.Value.AddItem(item));
			_right.Value.ItemMoved.Subscribe(item => _left.Value.AddItem(item));
		}
	}
}
