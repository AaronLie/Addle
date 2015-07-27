using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using Addle.Core.Linq;
using JetBrains.Annotations;

namespace Addle.Core.IO
{
	public class Serializer<T>
	{
		readonly DataContractSerializer _serializer;

		public Serializer(IEnumerable<Type> knownTypes = null)
		{
			knownTypes = knownTypes ?? Enumerable.Empty<Type>();
			_serializer = new DataContractSerializer(typeof(T), knownTypes);
		}

		public T Deserialize(string data)
		{
			var stream = new MemoryStream(Convert.FromBase64String(data));
			return (T)_serializer.ReadObject(stream);
		}

		public string Serialize(T value)
		{
			var ms = new MemoryStream();
			_serializer.WriteObject(ms, value);
			return Convert.ToBase64String(ms.ToArray());
		}

		public long GetUniqueId(T value)
		{
			var ms = new MemoryStream();
			_serializer.WriteObject(ms, value);
			var bytes = ms.ToArray();
			return bytes.Aggregate(0, (current, c) => current * 397 + c);
		}
	}
}
