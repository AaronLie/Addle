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
			@this._leftItems.SetRange(Enumerable.Range(0, value).Select(a => "foo-" + a));
			@this._rightItems.Clear();
			@this._moveRightCommand.CanExecute.OnNext(@this._leftItems.Count > 0);
			@this.UpdateCanExecute();
		});

		[VMProperty]
		readonly ObservableCollection<string> _leftItems = new ObservableCollection<string>();

		[VMProperty(IsWritable = true)]
		readonly AutoProperty<MainWindowVM, int> _leftSelectedIndex = new AutoProperty<MainWindowVM, int>(-1, (@this, value) =>
		{
			@this.UpdateCanExecute();
		});

		[VMProperty]
		readonly AutoCommand<MainWindowVM, string> _moveRightCommand = new AutoCommand<MainWindowVM, string>(@this =>
		{
			MoveItem(@this._leftItems, @this._leftSelectedIndex, @this._rightItems, @this._rightSelectedIndex);
			@this.UpdateCanExecute();
		});

		[VMProperty]
		readonly ObservableCollection<string> _rightItems = new ObservableCollection<string>();

		[VMProperty(IsWritable = true)]
		readonly AutoProperty<MainWindowVM, int> _rightSelectedIndex = new AutoProperty<MainWindowVM, int>(-1, (@this, value) =>
		{
			@this.UpdateCanExecute();
		});

		[VMProperty]
		readonly AutoCommand<MainWindowVM, string> _moveLeftCommand = new AutoCommand<MainWindowVM, string>(@this =>
		{
			MoveItem(@this._rightItems, @this._rightSelectedIndex, @this._leftItems, @this._leftSelectedIndex);
			@this.UpdateCanExecute();
		});

		static void MoveItem(IList<string> fromItems, IAutoProperty<int> fromSelectedIndex, ICollection<string> toItems, IAutoProperty<int> toSelectedIndex)
		{
			var index = fromSelectedIndex.Value;
			var item = fromItems[index];
			fromItems.RemoveAt(index);
			toItems.Add(item);
			fromSelectedIndex.Value = Math.Min(index, fromItems.Count - 1);
			toSelectedIndex.Value = toItems.Count - 1;
		}

		void UpdateCanExecute()
		{
			_moveRightCommand.CanExecute.OnNext(_leftSelectedIndex.Value != -1 && _leftItems.Count > 0);
			_moveLeftCommand.CanExecute.OnNext(_rightSelectedIndex.Value != -1 && _rightItems.Count > 0);
		}
	}
}
