using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GKEngine.Entities;

public class SpriteString : ISprite
{
	public enum Align
	{
		Left,
		Center,
		Right
	}

	private Color _tintFinal = default(Color);

	protected SpriteManager manager;

	protected SpriteFont font;

	public string text;

	public Color color;

	public bool visible = true;

	protected Vector2 _position;

	public Vector2 scale = new Vector2(1f, 1f);

	public Vector2 origin = new Vector2(0f, 0f);

	public float rotation;

	public float width;

	public float height;

	protected float _length;

	public Align align;

	public float lineHeight;

	public float lineHeightOffset;

	private List<Vector2> stackPositions = new List<Vector2>(1);

	private List<string> stackText = new List<string>(1);

	public int stackCount = 1;

	public Vector2 position
	{
		get
		{
			return _position;
		}
		set
		{
			_position = value;
			SetPositions();
		}
	}

	public float X
	{
		get
		{
			return _position.X;
		}
		set
		{
			_position.X = value;
			SetPositions();
		}
	}

	public float Y
	{
		get
		{
			return _position.Y;
		}
		set
		{
			_position.Y = value;
			SetPositions();
		}
	}

	public float length
	{
		get
		{
			return _length;
		}
		set
		{
			_length = value;
			SetText(text);
		}
	}

	public SpriteString(SpriteManager oManager, SpriteFont oFont, string xText, float xLength)
	{
		manager = oManager;
		font = oFont;
		_position = default(Vector2);
		color = new Color(255, 255, 255);
		_length = xLength;
		stackText.Add(null);
		stackPositions.Add(default(Vector2));
		SetText(xText);
		manager.Add(this);
	}

	public void SetText(string xString)
	{
		text = xString;
		if (_length == 0f)
		{
			stackText[0] = text;
			stackPositions[0] = new Vector2(position.X, position.Y);
			stackCount = 1;
		}
		else if (_length > 0f)
		{
			List<string> list = new List<string>();
			stackText = new List<string>();
			width = 0f;
			height = 0f;
			string[] array = text.Split('\n');
			for (int i = 0; i < array.Length; i++)
			{
				list.Clear();
				string[] array2 = array[i].Split(' ');
				for (int j = 0; j < array2.Length; j++)
				{
					list.Add(array2[j]);
					if (font.MeasureString(string.Join(" ", list.ToArray())).X > length)
					{
						list.RemoveAt(list.Count - 1);
						stackText.Add(string.Join(" ", list.ToArray()));
						list.Clear();
						list.Add(array2[j]);
					}
				}
				stackText.Add(string.Join(" ", list.ToArray()));
			}
		}
		stackCount = stackText.Count;
		SetSize();
		SetPositions();
	}

	public void SetSize()
	{
		if (_length == 0f)
		{
			Vector2 vector = font.MeasureString(text);
			width = vector.X;
			height = vector.Y;
		}
		else
		{
			if (!(_length > 0f))
			{
				return;
			}
			width = 0f;
			for (int i = 0; i < stackCount; i++)
			{
				Vector2 vector2 = font.MeasureString(stackText[i]);
				if (width < vector2.X)
				{
					width = vector2.X;
				}
			}
			height = (float)(stackCount - 1) * lineHeight + font.MeasureString(stackText[stackCount - 1]).Y + lineHeightOffset;
		}
	}

	public void SetPositions()
	{
		if (_length == 0f)
		{
			stackPositions[0] = new Vector2(position.X, position.Y);
		}
		else
		{
			if (!(_length > 0f))
			{
				return;
			}
			stackPositions.Clear();
			for (int i = 0; i < stackCount; i++)
			{
				switch (align)
				{
				case Align.Left:
					stackPositions.Add(new Vector2(_position.X, _position.Y + lineHeight * (float)i));
					break;
				case Align.Center:
					stackPositions.Add(new Vector2(_position.X + (length - font.MeasureString(stackText[i]).X) * 0.5f, _position.Y + lineHeight * (float)i));
					break;
				case Align.Right:
					stackPositions.Add(new Vector2((float)Math.Round(_position.X + (length - font.MeasureString(stackText[i]).X)), (float)Math.Round(_position.Y + lineHeight * (float)i)));
					break;
				}
			}
		}
	}

	public void Set(string xString, float xX, float xY, float xLength, Align oAlign)
	{
		_position.X = xX;
		_position.Y = xY;
		_length = xLength;
		align = oAlign;
		SetText(xString);
	}

	public void Dispose()
	{
		if (manager != null)
		{
			manager.Remove(this);
		}
		stackPositions = null;
		stackText = null;
		font = null;
		manager = null;
	}

	public void Render(GameTime oGameTime, ref SpriteBatch batch, ref Color globalTint)
	{
		if (visible)
		{
			_tintFinal.A = (byte)((float)(int)color.A / 255f * ((float)(int)globalTint.A / 255f) * 255f);
			_tintFinal.R = (byte)((float)(int)color.R / 255f * ((float)(int)globalTint.R / 255f) * 255f);
			_tintFinal.G = (byte)((float)(int)color.G / 255f * ((float)(int)globalTint.G / 255f) * 255f);
			_tintFinal.B = (byte)((float)(int)color.B / 255f * ((float)(int)globalTint.B / 255f) * 255f);
			for (int i = 0; i < stackCount; i++)
			{
				batch.DrawString(font, stackText[i], stackPositions[i], _tintFinal, rotation, origin, scale, SpriteEffects.None, 0f);
			}
		}
	}
}
