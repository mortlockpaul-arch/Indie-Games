using Microsoft.Xna.Framework;

namespace GKEngine.Entities;

public class Base2D
{
	public Vector2 position = new Vector2(0f, 0f);

	public Vector2 size = new Vector2(0f, 0f);

	public Vector2 scale = new Vector2(1f, 1f);

	public float rotation;

	public static Base3D Lerp(Base3D oTracer0, Base3D oTracer1, float xAmount)
	{
		return new Base3D(Vector3.Lerp(oTracer0.position, oTracer1.position, xAmount), Quaternion.Lerp(oTracer0.rotation, oTracer1.rotation, xAmount), Vector3.Lerp(oTracer0.scale, oTracer1.scale, xAmount));
	}

	public override string ToString()
	{
		return base.ToString();
	}
}
