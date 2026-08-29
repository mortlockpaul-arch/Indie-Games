using Microsoft.Xna.Framework;

namespace GKEngine.Entities;

public class Poly
{
	public Vector3 normal = default(Vector3);

	public Vector3[] vertex = new Vector3[3];

	public Vector3 center;

	public Poly(Vector3 xVectorA, Vector3 xVectorB, Vector3 xVectorC, Vector3 xNormal)
	{
		vertex[0] = xVectorA;
		vertex[1] = xVectorB;
		vertex[2] = xVectorC;
		normal = xNormal;
		center = (xVectorA + xVectorB + xVectorC) / 3f;
	}

	public void Dispose()
	{
		vertex = null;
	}
}
