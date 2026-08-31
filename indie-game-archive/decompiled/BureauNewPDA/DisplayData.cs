using System;
using Microsoft.Xna.Framework;

namespace BureauNewPDA;

public class DisplayData
{
	public enum ObjectTypeEnum
	{
		TextBox,
		Normal
	}

	public string baseImageName;

	public Vector2 position = Vector2.Zero;

	public float scale = 1f;

	public float rotation;

	public byte alpha = byte.MaxValue;

	public float depth = 0.5f;

	public Vector2 origin = Vector2.Zero;

	public Color myColor = Color.White;

	public int currentFrame;

	public string _textureName;

	public ObjectTypeEnum objectType = ObjectTypeEnum.Normal;

	public int objectId = -1;

	public int examId = -1;

	public int spriteWidthOverride;

	public bool isDisplayed;

	public string textureName
	{
		get
		{
			return _textureName;
		}
		set
		{
			if (value == "")
			{
				Console.WriteLine("error - empy display");
			}
			else
			{
				_textureName = value;
			}
		}
	}
}
