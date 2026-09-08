using System.Collections.Generic;

namespace Microsoft.Xna.Framework;

internal class UpdateOrderComparer : IComparer<IUpdateable>
{
	public static readonly UpdateOrderComparer Default = new UpdateOrderComparer();

	public int Compare(IUpdateable x, IUpdateable y)
	{
		if (x == null && y == null)
		{
			return 0;
		}
		if (x == null)
		{
			return 1;
		}
		if (y == null)
		{
			return -1;
		}
		if (x.Equals(y))
		{
			return 0;
		}
		if (x.UpdateOrder < y.UpdateOrder)
		{
			return -1;
		}
		return 1;
	}
}
