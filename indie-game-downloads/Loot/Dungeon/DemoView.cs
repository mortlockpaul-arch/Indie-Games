using Microsoft.Xna.Framework;

namespace Loot.Dungeon;

public class DemoView : DungeonView
{
	public override void transitionOn()
	{
	}

	public override void transitionOff()
	{
	}

	public override void update(GameTime gameTime)
	{
		updateFog(gameTime);
		DM.Update(gameTime);
	}
}
