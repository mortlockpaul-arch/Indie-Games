using System.IO;
using Eyehook.Framework;
using Loot.Dungeon;
using Microsoft.Xna.Framework;

namespace Loot.Widgets;

public class WidgetWallGear : Widget
{
	private float rotation;

	protected override Sprite Sprite
	{
		get
		{
			WidgetSprite.WallGear.Rotation = rotation;
			return WidgetSprite.WallGear;
		}
	}

	public WidgetWallGear(BinaryReader reader)
		: base(reader)
	{
	}

	public WidgetWallGear(Location loc)
		: base(loc)
	{
	}

	public override void Update(GameTime gameTime)
	{
		rotation += (float)(gameTime.ElapsedGameTime.TotalSeconds * 3.1415927410125732 / 8.0);
	}
}
