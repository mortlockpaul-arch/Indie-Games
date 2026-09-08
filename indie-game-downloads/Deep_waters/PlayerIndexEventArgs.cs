using System;
using Microsoft.Xna.Framework;

namespace Deep_waters;

internal class PlayerIndexEventArgs : EventArgs
{
	private PlayerIndex playerIndex;

	public PlayerIndex PlayerIndex => playerIndex;

	public PlayerIndexEventArgs(PlayerIndex playerIndex)
	{
		this.playerIndex = playerIndex;
	}
}
