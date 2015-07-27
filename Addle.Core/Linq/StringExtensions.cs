using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Addle.Core.Linq;
using JetBrains.Annotations;

namespace Addle.Core.Linq
{
	public static class StringExtensions
	{
		[DebuggerNonUserCode]
		[StringFormatMethod("format")]
		public static string FormatWith(this string format, params object[] args)
		{
			return String.Format(format, args);
		}

		[DebuggerNonUserCode]
		// ReSharper disable once InconsistentNaming
		public static bool IEquals(this string a, string b)
		{
			return a.Equals(b, StringComparison.OrdinalIgnoreCase);
		}

		[DebuggerNonUserCode]
		// ReSharper disable once InconsistentNaming
		public static bool IContains(this string a, string b)
		{
			return a.IndexOf(b, StringComparison.OrdinalIgnoreCase) != -1;
		}
	}
}
