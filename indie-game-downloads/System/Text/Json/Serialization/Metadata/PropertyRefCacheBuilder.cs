using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace System.Text.Json.Serialization.Metadata;

internal sealed class PropertyRefCacheBuilder(PropertyRef[] originalCache)
{
	private readonly List<PropertyRef> _propertyRefs = new List<PropertyRef>();

	private readonly HashSet<PropertyRef> _added = new HashSet<PropertyRef>();

	public readonly PropertyRef[] OriginalCache = originalCache;

	public int TotalCount => OriginalCache.Length + _propertyRefs.Count;

	public PropertyRef[] ToArray()
	{
		PropertyRef[] originalCache = OriginalCache;
		List<PropertyRef> propertyRefs = _propertyRefs;
		int num = 0;
		PropertyRef[] array = new PropertyRef[originalCache.Length + propertyRefs.Count];
		ReadOnlySpan<PropertyRef> readOnlySpan = new ReadOnlySpan<PropertyRef>(originalCache);
		readOnlySpan.CopyTo(new Span<PropertyRef>(array).Slice(num, readOnlySpan.Length));
		num += readOnlySpan.Length;
		Span<PropertyRef> span = CollectionsMarshal.AsSpan(propertyRefs);
		span.CopyTo(new Span<PropertyRef>(array).Slice(num, span.Length));
		num += span.Length;
		return array;
	}

	public void TryAdd(PropertyRef propertyRef)
	{
		if (_added.Add(propertyRef))
		{
			_propertyRefs.Add(propertyRef);
		}
	}
}
