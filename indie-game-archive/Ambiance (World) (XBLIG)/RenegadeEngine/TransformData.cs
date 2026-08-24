using Microsoft.Xna.Framework;

namespace RenegadeEngine;

public class TransformData
{
	public Matrix Transform = Matrix.Identity;

	public Vector3 Position = Vector3.Zero;

	public Vector3 Rotation = Vector3.Zero;

	public Vector3 Velocity = Vector3.Zero;

	public Vector3 Scale = Vector3.One;

	public float Speed;

	private Quaternion Orientation = Quaternion.Identity;

	public TransformData()
	{
	}

	public TransformData(Matrix world, Vector3 position, Vector3 rotation, Vector3 scale)
	{
		Transform = world;
		Position = position;
		Rotation = rotation;
		Scale = scale;
	}

	public void UpdateOrientation()
	{
		Quaternion.CreateFromYawPitchRoll(Rotation.Y, Rotation.X, Rotation.Z, out Orientation);
	}

	public void UpdateTransform()
	{
		Transform = Matrix.CreateScale(Scale) * Matrix.CreateFromQuaternion(Orientation) * Matrix.CreateTranslation(Position);
	}
}
