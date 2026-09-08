using System;
using Microsoft.Xna.Framework;

namespace GameStateManagement;

internal class PlayerIndexEventArgs : EventArgs
{
	private PlayerIndex playerIndex;

	public PlayerIndex PlayerIndex => playerIndex;

	public PlayerIndexEventArgs(PlayerIndex playerIndex)
	{
		this.playerIndex = playerIndex;
	}
}
