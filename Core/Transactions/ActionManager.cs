using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Subjects;
using Addle.Core.Linq;
using JetBrains.Annotations;

namespace Addle.Core.Transactions
{
	public class ActionManager
	{
		readonly Stack<IActionDescriptor> _undoStack = new Stack<IActionDescriptor>();
		readonly Stack<IActionDescriptor> _redoStack = new Stack<IActionDescriptor>();
		readonly Subject<Unit> _undoRedoChanged = new Subject<Unit>();

		public bool CanUndo { get { return _undoStack.Any(); } }
		public bool CanRedo { get { return _redoStack.Any(); } }
		public IObservable<Unit> UndoRedoChanged { get { return _undoRedoChanged; } }

		public void DoAction(IActionDescriptor descriptor)
		{
			descriptor.Do();

			_undoStack.Push(descriptor);
			_redoStack.Clear();

			_undoRedoChanged.OnNext();
		}

		public void Undo()
		{
			var descriptor = _undoStack.Pop();
			_redoStack.Push(descriptor);

			descriptor.Undo();

			_undoRedoChanged.OnNext();
		}

		public void Redo()
		{
			var descriptor = _redoStack.Pop();
			_undoStack.Push(descriptor);

			descriptor.Do();

			_undoRedoChanged.OnNext();
		}
	}
}
