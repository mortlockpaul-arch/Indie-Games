using System;
using Microsoft.Xna.Framework;

namespace RenegadeEngine.MenuSystem;

public class PlayerIndexEventArgs : EventArgs
{
	public PlayerIndex Index;

	public PlayerIndexEventArgs(PlayerIndex index)
	{
		Index = index;
	}
}
