using System.Collections.Generic;

namespace D;

internal class _3 : List<_0019>
{
	public _0018 GetNamedItem(string name)
	{
		using (Enumerator enumerator = GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				_0018 current = enumerator.Current;
				if (current.Name == name)
				{
					return current;
				}
			}
		}
		return null;
	}
}
