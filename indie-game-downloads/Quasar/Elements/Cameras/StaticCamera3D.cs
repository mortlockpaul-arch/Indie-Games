using Microsoft.Xna.Framework;
using Quasar.Global;

namespace Quasar.Elements.Cameras;

public class StaticCamera3D : Camera
{
	private Vector3 Position = new Vector3(0f, 0f, 500f);

	public StaticCamera3D()
	{
		projection = Matrix.CreatePerspective(Engine.GUIWidth / 2f, Engine.GUIHeight / 2f, 250f, 1000f);
		UpdateMatrix();
	}

	protected override void UpdateMatrix()
	{
		base.Transform.LookAt(Position, new Vector3(0f, 0f, 0f), Vector3.UnitY);
		base.UpdateMatrix();
	}
}
