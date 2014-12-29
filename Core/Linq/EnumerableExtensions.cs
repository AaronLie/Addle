using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Addle.Core.Linq;
using JetBrains.Annotations;

namespace Addle.Core.Linq
{
	public static class EnumerableExtensions
	{
		static readonly object _lock = new object();
		static readonly Random _random = new Random();

		public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> list, Random random = null)
		{
			var array = list.ToArray();

			for (var i = 0; i < array.Length - 1; i++)
			{
				int index;

				lock (_lock)
				{
					index = (random ?? _random).Next(i, array.Length);
				}

				var swap = array[index];
				array[index] = array[i];
				array[i] = swap;
			}

			return array;
		}

		public static void ForEach<T>(this IEnumerable<T> list, Action<T> action)
		{
			foreach (var item in list)
			{
				action(item);
			}
		}

		public static IEnumerable<T> Concat<T>(this IEnumerable<T> @this, T other)
		{
			return @this.Concat(EnumerableEx.Return(other));
		}

		// ReSharper disable once InconsistentNaming
		public static bool IContains(this IEnumerable<string> @this, string other)
		{
			return @this.Any(a => a.IEquals(other));
		}
	}

	public static class EnumerableEx
	{
		public static IEnumerable<T> Return<T>(T @this)
		{
			yield return @this;
		}
	}
}
