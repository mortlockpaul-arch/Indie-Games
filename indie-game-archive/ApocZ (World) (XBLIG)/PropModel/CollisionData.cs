using Microsoft.Xna.Framework;

namespace PropModel;

public class CollisionData
{
	public BoundingSphere bSphere;

	public BoundingBox bBox;

	public Matrix transform;

	public short[] indices;

	public Vector3[] vertices;
}
