using Loot.Dungeon;
using Microsoft.Xna.Framework.Graphics;

namespace Loot.Effects;

public abstract class FXCharacter : FX
{
	protected Character Character;

	protected FXCharacter()
	{
	}

	public FXCharacter(Character c)
	{
		Character = c;
	}

	public override void Draw(SpriteBatch spriteBatch)
	{
		Position = DungeonView.Camera.Position(Character);
	}
}
