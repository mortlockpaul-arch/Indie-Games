using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Hammer;

public class Camera2d
{
	protected float zoom;

	public Matrix transform;

	public Vector2 position;

	protected float rotation;

	public Rectangle limits = Rectangle.Empty;

	public float Zoom
	{
		get
		{
			return zoom;
		}
		set
		{
			zoom = value;
			if (zoom < 0.1f)
			{
				zoom = 0.1f;
			}
		}
	}

	public float Rotation
	{
		get
		{
			return rotation;
		}
		set
		{
			rotation = value;
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
		}
	}

	public Camera2d()
	{
		Initialize(Rectangle.Empty);
	}

	public Camera2d(Rectangle limit)
	{
		Initialize(limit);
	}

	public void Initialize(Rectangle limit)
	{
		zoom = 1f;
		rotation = 0f;
		position = Vector2.Zero;
		limits = limit;
	}

	public void Move(Vector2 displacement, bool respectRotation)
	{
		if (respectRotation)
		{
			displacement = Vector2.Transform(displacement, Matrix.CreateRotationZ(0f - Rotation));
		}
		Position += displacement;
	}

	public Vector2 get_mouse_vpos(GraphicsDevice graphicsDevice)
	{
		Vector2 vector = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
		return Vector2.Transform(vector, Matrix.Invert(get_transformation(graphicsDevice)));
	}

	public Vector2 getScreenPosition(Vector2 spritePosition, GraphicsDevice graphicsDevice)
	{
		return Vector2.Transform(spritePosition, get_transformation(graphicsDevice));
	}

	public Matrix get_transformation(GraphicsDevice graphicsDevice)
	{
		LimitCamera(graphicsDevice);
		transform = Matrix.Identity * Matrix.CreateTranslation(new Vector3(-position, 0f)) * Matrix.CreateRotationZ(rotation) * Matrix.CreateScale(new Vector3(zoom, zoom, 1f)) * Matrix.CreateTranslation(new Vector3((float)graphicsDevice.Viewport.Width / 2f - position.X * zoom, (float)graphicsDevice.Viewport.Height / 2f - position.Y * zoom, 0f));
		return transform;
	}

	private void LimitCamera(GraphicsDevice graphicsDevice)
	{
		if (limits.X + limits.Width + limits.Y + limits.Height > 0)
		{
			Vector2 vector = new Vector2(-limits.Width / 2, -limits.Height / 2);
			Vector2 vector2 = position + vector;
			float num = (float)(limits.X - limits.Width / 2) * zoom;
			float num2 = (float)(limits.X + limits.Width / 2) * zoom;
			float num3 = (float)(limits.Y - limits.Height / 2) * zoom;
			float num4 = (float)(limits.Y + limits.Height / 2) * zoom;
			if (vector2.X < num)
			{
				vector2.X = num;
			}
			if (vector2.X > num2)
			{
				vector2.X = num2;
			}
			if (vector2.Y < num3)
			{
				vector2.Y = num3;
			}
			if (vector2.Y > num4)
			{
				vector2.Y = num4;
			}
			vector2 -= vector;
			position = vector2;
		}
	}

	private void ValidatePosition(GraphicsDevice graphicsDevice)
	{
		if (limits != Rectangle.Empty)
		{
			Vector2 vector = Vector2.Transform(Vector2.Zero, Matrix.Invert(transform));
			Vector2 vector2 = new Vector2(graphicsDevice.Viewport.Width, graphicsDevice.Viewport.Height) / zoom;
			Vector2 min = new Vector2(limits.X, limits.Y);
			Vector2 vector3 = new Vector2(limits.X + limits.Width, limits.Y + limits.Height);
			Vector2 vector4 = position - vector;
			position = Vector2.Clamp(vector, min, vector3 - vector2) + vector4;
		}
	}

	public void ControlKeyboard()
	{
		KeyboardState state = Keyboard.GetState();
		if (state.IsKeyDown(Keys.Left))
		{
			Move(new Vector2(-1f, 0f), respectRotation: true);
		}
		if (state.IsKeyDown(Keys.Right))
		{
			Move(new Vector2(1f, 0f), respectRotation: true);
		}
		if (state.IsKeyDown(Keys.Up))
		{
			Move(new Vector2(0f, -1f), respectRotation: true);
		}
		if (state.IsKeyDown(Keys.Down))
		{
			Move(new Vector2(0f, 1f), respectRotation: true);
		}
		if (state.IsKeyDown(Keys.Q))
		{
			Rotation += 0.1f;
		}
		if (state.IsKeyDown(Keys.W))
		{
			Rotation -= 0.1f;
		}
		if (state.IsKeyDown(Keys.Z))
		{
			Zoom += 0.1f;
		}
		if (state.IsKeyDown(Keys.X))
		{
			Zoom -= 0.1f;
		}
	}
}
