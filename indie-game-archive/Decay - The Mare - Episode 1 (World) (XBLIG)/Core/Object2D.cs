using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SGSCore;

namespace Core;

public class Object2D : SGSContent
{
	public Object2D(string content_path)
		: base(content_path)
	{
	}

	public override void Clear()
	{
	}

	public virtual void Update(TimeSpan elapsed)
	{
	}

	public virtual void Draw(SpriteBatch SB)
	{
	}

	public virtual void Draw(SpriteBatch SB, Color color)
	{
	}

	public virtual void Draw(SpriteBatch SB, Vector2 pos)
	{
	}

	public virtual void Draw(SpriteBatch SB, Vector2 pos, Rectangle source_rect, Color color)
	{
	}

	public virtual void Draw(SpriteBatch SB, Vector2 pos, Color color)
	{
	}

	public virtual void Draw(SpriteBatch SB, Rectangle dest_rect, Rectangle source_rect, Color color)
	{
	}

	public virtual void DrawMultiply(SpriteBatch SB, Color color)
	{
	}
}
