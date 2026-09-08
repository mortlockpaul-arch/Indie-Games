using System.Collections;
using System.Collections.Generic;

namespace System.Net.Http.Headers;

/// <summary>Represents a collection of header values.</summary>
/// <typeparam name="T">The header collection type.</typeparam>
public sealed class HttpHeaderValueCollection<T> : ICollection<T>, IEnumerable<T>, IEnumerable where T : class
{
	private readonly HeaderDescriptor _descriptor;

	private readonly HttpHeaders _store;

	/// <summary>Gets the number of headers in the <see cref="T:System.Net.Http.Headers.HttpHeaderValueCollection`1" />.</summary>
	/// <returns>The number of headers in a collection</returns>
	public int Count => GetCount();

	/// <summary>Gets a value indicating whether the <see cref="T:System.Net.Http.Headers.HttpHeaderValueCollection`1" /> instance is read-only.</summary>
	/// <returns>
	///   <see langword="true" /> if the <see cref="T:System.Net.Http.Headers.HttpHeaderValueCollection`1" /> instance is read-only; otherwise, <see langword="false" />.</returns>
	public bool IsReadOnly => false;

	internal HttpHeaderValueCollection(HeaderDescriptor descriptor, HttpHeaders store)
	{
		_store = store;
		_descriptor = descriptor;
	}

	/// <summary>Adds an entry to the <see cref="T:System.Net.Http.Headers.HttpHeaderValueCollection`1" />.</summary>
	/// <param name="item">The item to add to the header collection.</param>
	public void Add(T item)
	{
		CheckValue(item);
		_store.AddParsedValue(_descriptor, item);
	}

	/// <summary>Parses and adds an entry to the <see cref="T:System.Net.Http.Headers.HttpHeaderValueCollection`1" />.</summary>
	/// <param name="input">The entry to add.</param>
	public void ParseAdd(string? input)
	{
		_store.Add(_descriptor, input);
	}

	/// <summary>Determines whether the input could be parsed and added to the <see cref="T:System.Net.Http.Headers.HttpHeaderValueCollection`1" />.</summary>
	/// <param name="input">The entry to validate.</param>
	/// <returns>
	///   <see langword="true" /> if the <paramref name="input" /> could be parsed and added to the <see cref="T:System.Net.Http.Headers.HttpHeaderValueCollection`1" /> instance; otherwise, <see langword="false" /></returns>
	public bool TryParseAdd(string? input)
	{
		return _store.TryParseAndAddValue(_descriptor, input);
	}

	/// <summary>Removes all entries from the <see cref="T:System.Net.Http.Headers.HttpHeaderValueCollection`1" />.</summary>
	public void Clear()
	{
		_store.Remove(_descriptor);
	}

	/// <summary>Determines if the <see cref="T:System.Net.Http.Headers.HttpHeaderValueCollection`1" /> contains an item.</summary>
	/// <param name="item">The item to find to the header collection.</param>
	/// <returns>
	///   <see langword="true" /> if the entry is contained in the <see cref="T:System.Net.Http.Headers.HttpHeaderValueCollection`1" /> instance; otherwise, <see langword="false" /></returns>
	public bool Contains(T item)
	{
		CheckValue(item);
		return _store.ContainsParsedValue(_descriptor, item);
	}

	/// <summary>Copies the entire <see cref="T:System.Net.Http.Headers.HttpHeaderValueCollection`1" /> to a compatible one-dimensional <see cref="T:System.Array" />, starting at the specified index of the target array.</summary>
	/// <param name="array">The one-dimensional <see cref="T:System.Array" /> that is the destination of the elements copied from <see cref="T:System.Net.Http.Headers.HttpHeaderValueCollection`1" />. The <see cref="T:System.Array" /> must have zero-based indexing.</param>
	/// <param name="arrayIndex">The zero-based index in <paramref name="array" /> at which copying begins.</param>
	public void CopyTo(T[] array, int arrayIndex)
	{
		ArgumentNullException.ThrowIfNull(array, "array");
		ArgumentOutOfRangeException.ThrowIfNegative(arrayIndex, "arrayIndex");
		ArgumentOutOfRangeException.ThrowIfGreaterThan(arrayIndex, array.Length, "arrayIndex");
		object parsedAndInvalidValues = _store.GetParsedAndInvalidValues(_descriptor);
		if (parsedAndInvalidValues == null)
		{
			return;
		}
		if (!(parsedAndInvalidValues is List<object> list))
		{
			if (!(parsedAndInvalidValues is HttpHeaders.InvalidValue))
			{
				if (arrayIndex == array.Length)
				{
					throw new ArgumentException(System.SR.net_http_copyto_array_too_small);
				}
				array[arrayIndex] = (T)parsedAndInvalidValues;
			}
			return;
		}
		foreach (object item in list)
		{
			if (!(item is HttpHeaders.InvalidValue))
			{
				if (arrayIndex == array.Length)
				{
					throw new ArgumentException(System.SR.net_http_copyto_array_too_small);
				}
				array[arrayIndex++] = (T)item;
			}
		}
	}

	/// <summary>Removes the specified item from the <see cref="T:System.Net.Http.Headers.HttpHeaderValueCollection`1" />.</summary>
	/// <param name="item">The item to remove.</param>
	/// <returns>
	///   <see langword="true" /> if the <paramref name="item" /> was removed from the <see cref="T:System.Net.Http.Headers.HttpHeaderValueCollection`1" /> instance; otherwise, <see langword="false" /></returns>
	public bool Remove(T item)
	{
		CheckValue(item);
		return _store.RemoveParsedValue(_descriptor, item);
	}

	/// <summary>Returns an enumerator that iterates through the <see cref="T:System.Net.Http.Headers.HttpHeaderValueCollection`1" />.</summary>
	/// <returns>An enumerator for the <see cref="T:System.Net.Http.Headers.HttpHeaderValueCollection`1" /> instance.</returns>
	public IEnumerator<T> GetEnumerator()
	{
		object parsedAndInvalidValues = _store.GetParsedAndInvalidValues(_descriptor);
		if (parsedAndInvalidValues != null && !(parsedAndInvalidValues is HttpHeaders.InvalidValue))
		{
			return Iterate(parsedAndInvalidValues);
		}
		return ((IEnumerable<T>)Array.Empty<T>()).GetEnumerator();
		static IEnumerator<T> Iterate(object storeValue)
		{
			if (storeValue is List<object> list)
			{
				foreach (object item in list)
				{
					if (!(item is HttpHeaders.InvalidValue))
					{
						yield return (T)item;
					}
				}
			}
			else
			{
				yield return (T)storeValue;
			}
		}
	}

	/// <summary>Returns an enumerator that iterates through the <see cref="T:System.Net.Http.Headers.HttpHeaderValueCollection`1" />.</summary>
	/// <returns>An enumerator for the <see cref="T:System.Net.Http.Headers.HttpHeaderValueCollection`1" /> instance.</returns>
	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	/// <summary>Returns a string that represents the current <see cref="T:System.Net.Http.Headers.HttpHeaderValueCollection`1" /> object. object.</summary>
	/// <returns>A string that represents the current object.</returns>
	public override string ToString()
	{
		return _store.GetHeaderString(_descriptor);
	}

	private void CheckValue(T item)
	{
		ArgumentNullException.ThrowIfNull(item, "item");
		if (_descriptor.Parser == GenericHeaderParser.TokenListParser)
		{
			HeaderUtilities.CheckValidToken((string)(object)item, "item");
		}
	}

	private int GetCount()
	{
		object parsedAndInvalidValues = _store.GetParsedAndInvalidValues(_descriptor);
		if (parsedAndInvalidValues == null)
		{
			return 0;
		}
		if (!(parsedAndInvalidValues is List<object> list))
		{
			if (!(parsedAndInvalidValues is HttpHeaders.InvalidValue))
			{
				return 1;
			}
			return 0;
		}
		int num = 0;
		foreach (object item in list)
		{
			if (!(item is HttpHeaders.InvalidValue))
			{
				num++;
			}
		}
		return num;
	}
}
