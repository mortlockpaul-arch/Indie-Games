using Microsoft.Xna.Framework;

namespace Quasar.Elements.Cameras;

public class PostprocessCamera : Camera
{
	private Vector3 Position = new Vector3(0f, 0f, 5f);

	public PostprocessCamera()
	{
		projection = Matrix.CreateOrthographicOffCenter(0f, 1f, 0f, 1f, 0.01f, 1000f);
		base.Transform.LookAt(new Vector3(0f, 0f, 5f), Vector3.Zero, Vector3.UnitY);
		UpdateMatrix();
	}
}
