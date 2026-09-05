using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GKEngine.Entities;

public class Sprite : Entity2D, ISprite
{
	public enum AxisType
	{
		TopLeft,
		Center
	}

	private Point _render_point = default(Point);

	private Rectangle _render_rect = default(Rectangle);

	private Color _tintFinal = default(Color);

	public string path;

	public SpriteManager manager;

	public bool packed;

	public Rectangle packedRect = default(Rectangle);

	public Vector2 axis = Vector2.Zero;

	private AxisType _axisType;

	protected Texture2D _texture;

	public AxisType axisType
	{
		get
		{
			return _axisType;
		}
		set
		{
			_axisType = value;
			axis = CalcAxis(value);
		}
	}

	public Texture2D texture
	{
		get
		{
			return _texture;
		}
		set
		{
			_texture = value;
			size = (packed ? new Vector2(packedRect.Width, packedRect.Height) : new Vector2(_texture.Width, _texture.Height));
			axis = CalcAxis(_axisType);
		}
	}

	public Sprite(SpriteManager oManager, string xPath)
	{
		manager = oManager;
		scene = manager.scene;
		path = xPath;
		Load();
		manager.Add(this);
	}

	public Sprite(SpriteManager oManager)
	{
		manager = oManager;
		scene = manager.scene;
		path = null;
		manager.Add(this);
	}

	public Sprite()
	{
		manager = null;
		scene = null;
		path = null;
	}

	public Sprite(SpriteManager oManager, Rectangle oClipRect)
	{
		manager = oManager;
		scene = manager.scene;
		path = null;
		packed = true;
		packedRect = oClipRect;
		manager.Add(this);
	}

	public Vector2 CalcAxis(AxisType xType)
	{
		Vector2 result = default(Vector2);
		switch (xType)
		{
		case AxisType.TopLeft:
			result = new Vector2(0f, 0f);
			break;
		case AxisType.Center:
			result = new Vector2(size.X / 2f, size.Y / 2f);
			break;
		}
		return result;
	}

	public override void Load()
	{
		if (path != null)
		{
			texture = GameEngine.Content.Load<Texture2D>(path);
		}
	}

	public override void Dispose()
	{
		base.Dispose();
		if (manager != null)
		{
			manager.Remove(this);
		}
		_texture = null;
	}

	public virtual void Render(GameTime oGameTime, ref SpriteBatch batch, ref Color globalTint)
	{
		if (visible && _texture != null)
		{
			_render_point.X = (int)position.X;
			_render_point.Y = (int)position.Y;
			if (packed)
			{
				_render_rect = packedRect;
			}
			else
			{
				_render_rect.X = 0;
				_render_rect.Y = 0;
				_render_rect.Width = (int)size.X;
				_render_rect.Height = (int)size.Y;
			}
			_tintFinal.A = (byte)((float)(int)tint.A / 255f * ((float)(int)globalTint.A / 255f) * 255f);
			_tintFinal.R = (byte)((float)(int)tint.R / 255f * ((float)(int)globalTint.R / 255f) * 255f);
			_tintFinal.G = (byte)((float)(int)tint.G / 255f * ((float)(int)globalTint.G / 255f) * 255f);
			_tintFinal.B = (byte)((float)(int)tint.B / 255f * ((float)(int)globalTint.B / 255f) * 255f);
			batch.Draw(_texture, position, _render_rect, _tintFinal, rotation, axis, scale, SpriteEffects.None, 0f);
		}
	}
}
