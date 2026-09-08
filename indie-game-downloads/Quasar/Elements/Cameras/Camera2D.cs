using Microsoft.Xna.Framework;
using Quasar.Global;

namespace Quasar.Elements.Cameras;

public abstract class Camera2D : Camera
{
	protected abstract Vector2 Position2D { get; }

	public Camera2D()
	{
		projection = Matrix.CreateOrthographic(Engine.GUIWidth, Engine.GUIHeight, 0.01f, 1000f);
		UpdateMatrix();
	}

	public Camera2D(Vector2 screenSize)
	{
		projection = Matrix.CreateOrthographic(screenSize.X, screenSize.Y, 0.01f, 1000f);
		UpdateMatrix();
	}

	protected void SetScreenSize(Vector2 screenSize)
	{
		projection = Matrix.CreateOrthographic(screenSize.X, screenSize.Y, 0.01f, 1000f);
	}

	protected override void UpdateMatrix()
	{
		base.Transform.LookAt(new Vector3(Position2D, 500f), new Vector3(Position2D, 0f), Vector3.UnitY);
		base.UpdateMatrix();
	}
}
