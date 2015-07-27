using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Subjects;
using Addle.Core.Linq;
using Addle.Wpf.ViewModel;
using JetBrains.Annotations;

namespace Addle.AutoVMTest
{
	public class ItemListVM
	{
		readonly Subject<string> _itemMoved = new Subject<string>();

		[VMProperty]
		readonly ObservableCollection<string> _items = new ObservableCollection<string>();

		[VMProperty(IsWritable = true, DesignTime = -1)]
		readonly AutoProperty<ItemListVM, int> _selectedIndex = new AutoProperty<ItemListVM, int>(-1, (@this, value) =>
		{
			@this.UpdateCanExecute();
        });

		[VMProperty]
		readonly AutoCommand<ItemListVM, string> _moveCommand = new AutoCommand<ItemListVM, string>(@this =>
		{
			var index = @this._selectedIndex.Value;
			var item = @this._items[index];
			@this._items.RemoveAt(index);
			@this._selectedIndex.Value = Math.Min(index, @this._items.Count - 1);
			@this.UpdateCanExecute();
			@this._itemMoved.OnNext(item);
        });

		public void UpdateCanExecute()
		{
			_moveCommand.CanExecute.OnNext(_selectedIndex.Value != -1 && _items.Count > 0);
		}

		public void SetRange(IEnumerable<string> items)
		{
			_items.SetRange(items);
			UpdateCanExecute();
		}

		public void Clear()
		{
			_items.Clear();
			UpdateCanExecute();
		}

		public void AddItem(string item)
		{
			_items.Add(item);
			UpdateCanExecute();
		}

		public IObservable<string> ItemMoved => _itemMoved;
	}
}
