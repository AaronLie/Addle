using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using Addle.Core.Linq;
using JetBrains.Annotations;

namespace Addle.Core.Linq
{
	public static class ObservableExtensions
	{
		public static IDisposable Subscribe(this IObservable<Unit> observable, Action onNext)
		{
			return observable.Subscribe(_ => onNext());
		}

		public static IDisposable SubscribeWeak<T>(this IObservable<T> observable, Action<T> onNext, Action<Exception> onError = null, Action onCompleted = null)
		{
			return observable.Subscribe(new WeakWrappingObserver<T>(Observer.Create(onNext, onError ?? (_ => { }), onCompleted ?? (() => { }))));
		}

		#region class WeakWrappingObserver<T>

		class WeakWrappingObserver<T> : IObserver<T>
		{
			readonly WeakReference<IObserver<T>> _observer;

			public WeakWrappingObserver(IObserver<T> observer)
			{
				_observer = new WeakReference<IObserver<T>>(observer);
			}

			public void OnCompleted()
			{
				var observer = _observer.TryGetTarget();
				observer?.OnCompleted();
			}

			public void OnError(Exception error)
			{
				var observer = _observer.TryGetTarget();

				observer?.OnError(error);
			}

			public void OnNext(T value)
			{
				var observer = _observer.TryGetTarget();
				observer?.OnNext(value);
			}
		}

		#endregion
	}
}
