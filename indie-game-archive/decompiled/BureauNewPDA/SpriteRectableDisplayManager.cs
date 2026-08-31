using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace BureauNewPDA;

public class SpriteRectableDisplayManager
{
	private class spriteData
	{
		public string textureName = "";

		public Dictionary<string, Dictionary<int, Rectangle>> newSpriteRectableLookup = new Dictionary<string, Dictionary<int, Rectangle>>();
	}

	private List<spriteData> spriteList = new List<spriteData>();

	private Rectangle empty = Rectangle.Empty;

	public bool checkForTetxureData(string textureName)
	{
		foreach (spriteData sprite in spriteList)
		{
			if (sprite.textureName == textureName)
			{
				return true;
			}
		}
		return false;
	}

	public Rectangle getSpriteRectangle(string textureName, string baseName, int frame)
	{
		foreach (spriteData sprite in spriteList)
		{
			if (sprite.textureName == textureName)
			{
				try
				{
					return sprite.newSpriteRectableLookup[baseName][frame];
				}
				catch
				{
					Console.WriteLine("error -231- sprite not found " + baseName + " " + frame);
					return empty;
				}
			}
		}
		return empty;
	}

	public void addSpriteData(string _textureName, Dictionary<string, Dictionary<int, Rectangle>> sprites)
	{
		spriteData spriteData2 = new spriteData();
		spriteData2.textureName = _textureName;
		spriteData2.newSpriteRectableLookup = sprites;
		spriteList.Add(spriteData2);
	}
}
