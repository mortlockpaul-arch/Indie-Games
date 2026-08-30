using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MaximinusDataTypes;

public class SpriteSheet
{
	[ContentSerializer]
	private Texture2D texture;

	[ContentSerializer]
	private List<Rectangle> spriteRectangles;

	[ContentSerializer]
	private Dictionary<string, int> spriteNames;

	public Texture2D Texture => texture;

	public Rectangle SourceRectangle(string spriteName)
	{
		int index = GetIndex(spriteName);
		return spriteRectangles[index];
	}

	public Rectangle SourceRectangle(int spriteIndex)
	{
		if (spriteIndex < 0 || spriteIndex >= spriteRectangles.Count)
		{
			throw new ArgumentOutOfRangeException("spriteIndex");
		}
		return spriteRectangles[spriteIndex];
	}

	public int GetIndex(string spriteName)
	{
		if (!spriteNames.TryGetValue(spriteName, out var value))
		{
			string format = "SpriteSheet does not contain a sprite named '{0}'.";
			throw new KeyNotFoundException(string.Format(format, spriteName));
		}
		return value;
	}
}
