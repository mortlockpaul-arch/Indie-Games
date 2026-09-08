using Loot.Dungeon;
using Microsoft.Xna.Framework.Graphics;

namespace Loot.Effects;

public abstract class FXLocation : FX
{
	protected Location Location;

	protected FXLocation()
	{
	}

	public FXLocation(Location loc)
	{
		Location = loc;
	}

	public override void Draw(SpriteBatch spriteBatch)
	{
		Position = DungeonView.Camera.Position(Location);
	}
}
