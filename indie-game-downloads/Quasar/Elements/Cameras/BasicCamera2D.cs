using Microsoft.Xna.Framework;
using Quasar.Global;

namespace Quasar.Elements.Cameras;

public class BasicCamera2D : Camera2D
{
	private Vector2 position = Vector2.Zero;

	private Vector2 zoom = Vector2.One;

	private Vector2 minBound = new Vector2(float.MinValue, float.MinValue);

	private Vector2 maxBound = new Vector2(float.MaxValue, float.MaxValue);

	public Vector2 Position
	{
		get
		{
			return position;
		}
		set
		{
			position = value;
			checkBounds();
			UpdateMatrix();
		}
	}

	public Vector2 Zoom
	{
		get
		{
			return zoom;
		}
		set
		{
			zoom = value;
			checkBounds();
			UpdateMatrix();
		}
	}

	public Vector2 MinZoomValue => Engine.GUISize / (maxBound - minBound);

	public Vector2 MinBound
	{
		get
		{
			return minBound;
		}
		set
		{
			minBound = value;
		}
	}

	public Vector2 MaxBound
	{
		get
		{
			return maxBound;
		}
		set
		{
			maxBound = value;
		}
	}

	protected override Vector2 Position2D => Position;

	protected void checkBounds()
	{
		position = Vector2.Max(position, MinBound + Engine.GUISize / (2f * Zoom));
		position = Vector2.Min(position, MaxBound - Engine.GUISize / (2f * Zoom));
	}

	protected override void UpdateMatrix()
	{
		base.Transform.Scale = new Vector3(Zoom, 1f);
		base.UpdateMatrix();
	}
}
