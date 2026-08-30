using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Defenders;

internal class Camera2d
{
	protected float zoom;

	public Matrix transform;

	public Vector2 position;

	protected float rotation;

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
		zoom = 1f;
		rotation = 0f;
		position = Vector2.Zero;
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
		transform = Matrix.Identity * Matrix.CreateTranslation(new Vector3(-position, 0f)) * Matrix.CreateRotationZ(rotation) * Matrix.CreateScale(new Vector3(zoom, zoom, 1f)) * Matrix.CreateTranslation(new Vector3((float)graphicsDevice.Viewport.Width / 2f - position.X * zoom, (float)graphicsDevice.Viewport.Height / 2f - position.Y * zoom, 0f));
		return transform;
	}

	public Matrix get_normalize(GraphicsDevice graphicsDevice)
	{
		transform = Matrix.Identity * Matrix.CreateScale(new Vector3(zoom, zoom, 1f));
		return transform;
	}
}
