using System;
using System.Collections.Generic;

namespace Microsoft.CSharp.RuntimeBinder.ComInterop;

internal static class CollectionExtensions
{
	internal static T[] RemoveFirst<T>(this T[] array)
	{
		T[] array2 = new T[array.Length - 1];
		Array.Copy(array, 1, array2, 0, array2.Length);
		return array2;
	}

	internal static T[] AddFirst<T>(this IList<T> list, T item)
	{
		T[] array = new T[list.Count + 1];
		array[0] = item;
		list.CopyTo(array, 1);
		return array;
	}

	internal static T[] ToArray<T>(this IList<T> list)
	{
		T[] array = new T[list.Count];
		list.CopyTo(array, 0);
		return array;
	}

	internal static T[] AddLast<T>(this IList<T> list, T item)
	{
		T[] array = new T[list.Count + 1];
		list.CopyTo(array, 0);
		array[list.Count] = item;
		return array;
	}
}
