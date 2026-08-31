using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace BureauNewPDA;

public class CoreDisplayElements
{
	public SpriteRectableDisplayManager spriteRDM = new SpriteRectableDisplayManager();

	public List<TextureData> myCurrentTextures = new List<TextureData>();

	public SpriteFont myPDAFontHeader;

	public SpriteFont myPDAFontRegular;

	public SpriteFont MainFontRegular;

	public Texture2D getTexture(string textureName)
	{
		foreach (TextureData myCurrentTexture in myCurrentTextures)
		{
			if (myCurrentTexture.textureName == textureName)
			{
				return myCurrentTexture.texture;
			}
		}
		TextureData textureData = new TextureData();
		Console.WriteLine("Error 341:1 - Texture not found = " + textureName);
		return textureData.texture;
	}

	public string parseText(string text, SpriteFont myFont, int width)
	{
		string text2 = string.Empty;
		string text3 = string.Empty;
		string[] array = text.Split(' ');
		string[] array2 = array;
		foreach (string text4 in array2)
		{
			if (myFont.MeasureString(text2 + text4).Length() > (float)width)
			{
				text3 = text3 + text2 + '\n';
				text2 = string.Empty;
			}
			text2 = text2 + text4 + ' ';
		}
		return text3 + text2;
	}
}
