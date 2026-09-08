using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace Microsoft.CSharp.RuntimeBinder;

internal static class BinderEquivalence
{
	internal sealed class BinderEqualityComparer : IEqualityComparer<ICSharpBinder>
	{
		public bool Equals(ICSharpBinder x, ICSharpBinder y)
		{
			return x.IsEquivalentTo(y);
		}

		public int GetHashCode(ICSharpBinder obj)
		{
			return obj.GetGetBinderEquivalenceHash();
		}
	}

	private static int cachedBinderCount;

	private static readonly ConcurrentDictionary<ICSharpBinder, ICSharpBinder> binderEquivalenceCache = new ConcurrentDictionary<ICSharpBinder, ICSharpBinder>(2, 32, new BinderEqualityComparer());

	internal static T TryGetExisting<T>(this T binder) where T : ICSharpBinder
	{
		ICSharpBinder orAdd = binderEquivalenceCache.GetOrAdd(binder, binder);
		if (orAdd == (object)binder && (uint)Interlocked.Increment(ref cachedBinderCount) > 4096u)
		{
			binderEquivalenceCache.Clear();
			cachedBinderCount = 0;
		}
		return (T)orAdd;
	}
}
