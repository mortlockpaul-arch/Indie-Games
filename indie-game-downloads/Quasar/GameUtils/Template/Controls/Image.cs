using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Quasar.GUI;

namespace Quasar.GameUtils.Template.Controls;

public class Image : Control
{
	public const string Type = "Image";

	private Texture2D texture;

	private Vector2 position;

	private Vector2 size;

	public override string ControlType => "Image";

	public Texture2D Texture
	{
		get
		{
			return texture;
		}
		set
		{
			texture = value;
		}
	}

	public Vector2 Position
	{
		get
		{
			return position;
		}
		set
		{
			position = value;
			if (OnPositionChanged != null)
			{
				OnPositionChanged(this, value);
			}
		}
	}

	public Vector2 Size
	{
		get
		{
			return size;
		}
		set
		{
			size = value;
			if (OnSizeChanged != null)
			{
				OnSizeChanged(this, value);
			}
		}
	}

	public event Action<Image, Vector2> OnPositionChanged;

	public event Action<Image, Vector2> OnSizeChanged;

	public Image(Texture2D texture, Layout layout)
		: base(layout)
	{
		this.texture = texture;
	}
}
