using System.IO;
using Eyehook.Framework;
using Loot.Dungeon;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Loot.Widgets;

public abstract class Widget : BinaryRW, DamageSource
{
	public Location Location;

	protected abstract Sprite Sprite { get; }

	public Widget(Location loc)
	{
		Location = loc;
	}

	public Widget(BinaryReader reader)
	{
		Read(reader);
	}

	public virtual void OnMove()
	{
	}

	public virtual void OnStep()
	{
	}

	public virtual void OnClick()
	{
	}

	public virtual void Update(GameTime gameTime)
	{
	}

	public virtual void Draw(SpriteBatch spriteBatch, Vector2 pos, Color color, float scale)
	{
		Sprite.Draw(spriteBatch, pos, color, scale);
	}

	public virtual void Read(BinaryReader reader)
	{
		Location.Read(reader);
	}

	public virtual void Write(BinaryWriter writer)
	{
		Location.Write(writer);
	}
}
