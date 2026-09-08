using System.IO;
using Eyehook.Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Loot.Skills;

public abstract class Skill
{
	public int Level;

	public abstract string Name { get; }

	public abstract Sprite Sprite { get; }

	public Skill()
	{
		Level = 0;
	}

	public Skill(BinaryReader reader)
	{
		Read(reader);
	}

	public virtual void Update(GameTime gameTime)
	{
	}

	public virtual void Draw(SpriteBatch spriteBatch, Vector2 offset, float scale)
	{
	}

	protected virtual void Read(BinaryReader reader)
	{
		Level = reader.ReadInt32();
	}

	public virtual void Write(BinaryWriter writer)
	{
		writer.Write(Level);
	}
}
