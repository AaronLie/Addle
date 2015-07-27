using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Subjects;
using Addle.Core.Linq;
using JetBrains.Annotations;

namespace Addle.Core.Linq
{
	public static class SubjectExtensions
	{
		public static void OnNext(this ISubject<Unit> subject)
		{
			subject.OnNext(default(Unit));
		}
	}
}
