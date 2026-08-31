using System.Collections.Generic;

namespace _0001
{
	internal class H : List<_0012>
	{
		public B GetNamedItem(string name)
		{
			using (Enumerator enumerator = GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					B current = enumerator.Current;
					if (current.Name == name)
					{
						return current;
					}
				}
			}
			return null;
		}
	}
}
namespace _0003
{
	internal enum H
	{
		Point,
		Directional
	}
}
