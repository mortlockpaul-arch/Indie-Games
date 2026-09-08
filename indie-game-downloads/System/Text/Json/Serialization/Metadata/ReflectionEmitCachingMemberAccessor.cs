using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Threading;

namespace System.Text.Json.Serialization.Metadata;

internal sealed class ReflectionEmitCachingMemberAccessor : MemberAccessor
{
	private sealed class Cache<TKey>
	{
		private sealed class CacheEntry
		{
			public readonly object Value;

			public long LastUsedTicks;

			public CacheEntry(object value)
			{
				Value = value;
			}
		}

		private int _evictLock;

		private long _lastEvictedTicks;

		private readonly long _evictionIntervalTicks;

		private readonly long _slidingExpirationTicks;

		private readonly ConcurrentDictionary<TKey, CacheEntry> _cache = new ConcurrentDictionary<TKey, CacheEntry>();

		public Cache(TimeSpan slidingExpiration, TimeSpan evictionInterval)
		{
			_slidingExpirationTicks = slidingExpiration.Ticks;
			_evictionIntervalTicks = evictionInterval.Ticks;
			_lastEvictedTicks = DateTime.UtcNow.Ticks;
		}

		public TValue GetOrAdd<TValue>(TKey key, Func<TKey, TValue> valueFactory) where TValue : class
		{
			CacheEntry orAdd = _cache.GetOrAdd(key, (TKey arg, Func<TKey, TValue> func) => new CacheEntry(func(arg)), valueFactory);
			long ticks = DateTime.UtcNow.Ticks;
			Volatile.Write(ref orAdd.LastUsedTicks, ticks);
			if (ticks - Volatile.Read(in _lastEvictedTicks) >= _evictionIntervalTicks && Interlocked.CompareExchange(ref _evictLock, 1, 0) == 0)
			{
				if (ticks - _lastEvictedTicks >= _evictionIntervalTicks)
				{
					EvictStaleCacheEntries(ticks);
					Volatile.Write(ref _lastEvictedTicks, ticks);
				}
				Volatile.Write(ref _evictLock, 0);
			}
			return (TValue)orAdd.Value;
		}

		public void Clear()
		{
			_cache.Clear();
			_lastEvictedTicks = DateTime.UtcNow.Ticks;
		}

		private void EvictStaleCacheEntries(long utcNowTicks)
		{
			foreach (KeyValuePair<TKey, CacheEntry> item in _cache)
			{
				if (utcNowTicks - Volatile.Read(in item.Value.LastUsedTicks) >= _slidingExpirationTicks)
				{
					_cache.TryRemove(item.Key, out var _);
				}
			}
		}
	}

	private readonly ReflectionEmitMemberAccessor _sourceAccessor;

	private readonly Cache<(string id, Type declaringType, MemberInfo member)> _cache;

	[RequiresDynamicCode("JSON serialization and deserialization might require types that cannot be statically analyzed and might need runtime code generation. Use System.Text.Json source generation for native AOT applications.")]
	[RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed and might need runtime code generation. Use System.Text.Json source generation for native AOT applications.")]
	public ReflectionEmitCachingMemberAccessor()
	{
		_sourceAccessor = new ReflectionEmitMemberAccessor();
		_cache = new Cache<(string, Type, MemberInfo)>(TimeSpan.FromMilliseconds(1000L), TimeSpan.FromMilliseconds(200L));
	}

	public override void Clear()
	{
		_cache.Clear();
	}

	public override Action<TCollection, object> CreateAddMethodDelegate<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods)] TCollection>()
	{
		return _cache.GetOrAdd(("CreateAddMethodDelegate", typeof(TCollection), null), ((string id, Type declaringType, MemberInfo member) _) => _sourceAccessor.CreateAddMethodDelegate<TCollection>());
	}

	public override Func<object> CreateParameterlessConstructor(Type type, ConstructorInfo ctorInfo)
	{
		return _cache.GetOrAdd(("CreateParameterlessConstructor", type, ctorInfo), ((string id, Type declaringType, MemberInfo member) key) => _sourceAccessor.CreateParameterlessConstructor(key.declaringType, (ConstructorInfo)key.member));
	}

	public override Func<object, TProperty> CreateFieldGetter<TProperty>(FieldInfo fieldInfo)
	{
		return _cache.GetOrAdd(("CreateFieldGetter", typeof(TProperty), fieldInfo), ((string id, Type declaringType, MemberInfo member) key) => _sourceAccessor.CreateFieldGetter<TProperty>((FieldInfo)key.member));
	}

	public override Action<object, TProperty> CreateFieldSetter<TProperty>(FieldInfo fieldInfo)
	{
		return _cache.GetOrAdd(("CreateFieldSetter", typeof(TProperty), fieldInfo), ((string id, Type declaringType, MemberInfo member) key) => _sourceAccessor.CreateFieldSetter<TProperty>((FieldInfo)key.member));
	}

	public override Func<IEnumerable<KeyValuePair<TKey, TValue>>, TCollection> CreateImmutableDictionaryCreateRangeDelegate<TCollection, TKey, TValue>()
	{
		return _cache.GetOrAdd(("CreateImmutableDictionaryCreateRangeDelegate", typeof((TCollection, TKey, TValue)), null), ((string id, Type declaringType, MemberInfo member) _) => _sourceAccessor.CreateImmutableDictionaryCreateRangeDelegate<TCollection, TKey, TValue>());
	}

	public override Func<IEnumerable<TElement>, TCollection> CreateImmutableEnumerableCreateRangeDelegate<TCollection, TElement>()
	{
		return _cache.GetOrAdd(("CreateImmutableEnumerableCreateRangeDelegate", typeof((TCollection, TElement)), null), ((string id, Type declaringType, MemberInfo member) _) => _sourceAccessor.CreateImmutableEnumerableCreateRangeDelegate<TCollection, TElement>());
	}

	public override Func<object[], T> CreateParameterizedConstructor<T>(ConstructorInfo constructor)
	{
		return _cache.GetOrAdd(("CreateParameterizedConstructor", typeof(T), constructor), ((string id, Type declaringType, MemberInfo member) key) => _sourceAccessor.CreateParameterizedConstructor<T>((ConstructorInfo)key.member));
	}

	public override JsonTypeInfo.ParameterizedConstructorDelegate<T, TArg0, TArg1, TArg2, TArg3> CreateParameterizedConstructor<T, TArg0, TArg1, TArg2, TArg3>(ConstructorInfo constructor)
	{
		return _cache.GetOrAdd(("CreateParameterizedConstructor", typeof(T), constructor), ((string id, Type declaringType, MemberInfo member) key) => _sourceAccessor.CreateParameterizedConstructor<T, TArg0, TArg1, TArg2, TArg3>((ConstructorInfo)key.member));
	}

	public override Func<object, TProperty> CreatePropertyGetter<TProperty>(PropertyInfo propertyInfo)
	{
		return _cache.GetOrAdd(("CreatePropertyGetter", typeof(TProperty), propertyInfo), ((string id, Type declaringType, MemberInfo member) key) => _sourceAccessor.CreatePropertyGetter<TProperty>((PropertyInfo)key.member));
	}

	public override Action<object, TProperty> CreatePropertySetter<TProperty>(PropertyInfo propertyInfo)
	{
		return _cache.GetOrAdd(("CreatePropertySetter", typeof(TProperty), propertyInfo), ((string id, Type declaringType, MemberInfo member) key) => _sourceAccessor.CreatePropertySetter<TProperty>((PropertyInfo)key.member));
	}
}
