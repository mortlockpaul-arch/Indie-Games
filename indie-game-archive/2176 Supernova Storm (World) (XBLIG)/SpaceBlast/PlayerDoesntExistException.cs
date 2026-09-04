using System;

namespace SpaceBlast;

internal class PlayerDoesntExistException : Exception
{
	public int PlayerIndex;

	public PlayerDoesntExistException(int index)
	{
		PlayerIndex = index;
	}
}
