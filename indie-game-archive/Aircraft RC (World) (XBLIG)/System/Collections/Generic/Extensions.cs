namespace System.Collections.Generic;

/// <summary>
/// Class used to extend other classes.
/// </summary>
public static class Extensions
{
	/// <summary>
	/// Removes all elements from the List that match the conditions defined by the specified predicate.
	/// </summary>
	/// <typeparam name="T">The type of elements held by the List.</typeparam>
	/// <param name="list">The List to remove the elements from.</param>
	/// <param name="match">The Predicate delegate that defines the conditions of the elements to remove.</param>
	public static int RemoveAll<T>(this List<T> list, Func<T, bool> match)
	{
		int num = 0;
		for (int num2 = list.Count - 1; num2 >= 0; num2--)
		{
			if (match(list[num2]))
			{
				list.RemoveAt(num2);
				num++;
			}
		}
		return num;
	}

	/// <summary>
	/// Returns true if the List contains elements that match the conditions defined by the specified predicate.
	/// </summary>
	/// <typeparam name="T">The type of elements held by the List.</typeparam>
	/// <param name="list">The List to search for a match in.</param>
	/// <param name="match">The Predicate delegate that defines the conditions of the elements to match against.</param>
	public static bool Exists<T>(this List<T> list, Func<T, bool> match)
	{
		for (int i = 0; i < list.Count; i++)
		{
			if (match(list[i]))
			{
				return true;
			}
		}
		return false;
	}
}
