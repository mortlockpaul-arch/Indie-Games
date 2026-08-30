using System;
using Microsoft.Xna.Framework;

namespace Kobingo.Xna.Library.Game;

public class GameTimeEventArgs : EventArgs
{
	public GameTime Value { get; set; }
}
