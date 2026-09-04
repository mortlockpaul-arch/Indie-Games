using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace XnaLibrary.Graphics;

public class Sprite : GameObject, ICloneable
{
	protected Texture2D texture;

	protected Vector2 position;

	protected Vector2 texturePosition;

	protected Vector2 size;

	protected Color color;

	protected Vector2 scale;

	protected Vector2 origin;

	protected float priority;

	protected float rotate;

	protected float direction;

	protected float speed;

	protected SpriteEffects spriteEffects;

	protected Sprite parent;

	protected List<Sprite> child;

	protected Dictionary<string, object> tags;

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
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return position;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			position = value;
		}
	}

	public Vector2 TexturePosition
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return texturePosition;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			texturePosition = value;
		}
	}

	public Vector2 Size
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return size;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			size = value;
		}
	}

	public Color Color
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return color;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			color = value;
		}
	}

	public Vector2 Scale
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return scale;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			scale = value;
		}
	}

	public Vector2 Origin
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return origin;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			origin = value;
		}
	}

	public float Priority
	{
		get
		{
			return priority;
		}
		set
		{
			priority = value;
		}
	}

	public float Rotate
	{
		get
		{
			return rotate;
		}
		set
		{
			rotate = value;
		}
	}

	public float Direction
	{
		get
		{
			return direction;
		}
		set
		{
			direction = value;
		}
	}

	public float Speed
	{
		get
		{
			return speed;
		}
		set
		{
			speed = value;
		}
	}

	public SpriteEffects SpriteEffects
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return spriteEffects;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			spriteEffects = value;
		}
	}

	public Sprite Parent
	{
		get
		{
			return parent;
		}
		set
		{
			parent = value;
		}
	}

	public List<Sprite> Child => child;

	public Dictionary<string, object> Tags => tags;

	public Rectangle RectanglePosition
	{
		get
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			return new Rectangle
			{
				X = (int)Position.X,
				Y = (int)Position.Y,
				Width = (int)Size.X,
				Height = (int)Size.Y
			};
		}
	}

	public Rectangle SourceRectangle
	{
		get
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			return new Rectangle
			{
				X = (int)TexturePosition.X,
				Y = (int)TexturePosition.Y,
				Width = (int)Size.X,
				Height = (int)Size.Y
			};
		}
	}

	public Sprite(Game game)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		base._002Ector(game);
		texture = null;
		position = Vector2.Zero;
		texturePosition = Vector2.Zero;
		size = Vector2.Zero;
		color = Color.White;
		scale = Vector2.One;
		origin = Vector2.Zero;
		priority = 0f;
		rotate = 0f;
		direction = 0f;
		speed = 1f;
		spriteEffects = (SpriteEffects)0;
		parent = null;
		child = new List<Sprite>();
		tags = new Dictionary<string, object>();
	}

	public static void DefaultDraw(object sender, GameTime gameTime, SpriteBatch batch)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		Sprite sprite = (Sprite)sender;
		if (sprite.Texture != null)
		{
			batch.Draw(sprite.Texture, sprite.Position, (Rectangle?)sprite.SourceRectangle, sprite.Color, sprite.Rotate, sprite.Origin, sprite.Scale, sprite.SpriteEffects, sprite.Priority);
		}
	}

	public object Clone()
	{
		return MemberwiseClone();
	}
}
