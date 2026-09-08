using System.Collections.Generic;

namespace Microsoft.Xna.Framework.Graphics;

internal sealed class StateHashComparer : IEqualityComparer<StateHash>
{
	public static readonly StateHashComparer Instance = new StateHashComparer();

	public bool Equals(StateHash x, StateHash y)
	{
		return x.Equals(y);
	}

	public int GetHashCode(StateHash obj)
	{
		return obj.GetHashCode();
	}
}
