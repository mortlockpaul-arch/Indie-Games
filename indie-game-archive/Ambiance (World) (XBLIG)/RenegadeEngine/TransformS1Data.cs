using Microsoft.Xna.Framework;

namespace RenegadeEngine;

public class TransformS1Data
{
	public Matrix Transform = Matrix.Identity;

	public Vector3 Position = Vector3.Zero;

	public Vector3 Rotation = Vector3.Zero;

	public Vector3 Velocity = Vector3.Zero;

	public float Speed;

	private Quaternion orientation = Quaternion.Identity;

	public TransformS1Data()
	{
	}

	public TransformS1Data(Matrix world, Vector3 position, Vector3 rotation, Vector3 scale)
	{
		Transform = world;
		Position = position;
		Rotation = rotation;
	}

	public void UpdateOrientation()
	{
		Quaternion.CreateFromYawPitchRoll(Rotation.Y, Rotation.X, Rotation.Z, out orientation);
	}

	public void UpdateTransform()
	{
		Transform = Matrix.CreateFromQuaternion(orientation) * Matrix.CreateTranslation(Position);
	}
}
