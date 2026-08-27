using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EGEngine;

public class CameraBase
{
	public float Scroll;

	public float Yaw;

	public float Pitch;

	public Vector3 vecDirection = Vector3.UnitZ;

	public Vector3 vecRight;

	public Vector3 vecUp;

	public Vector3 vecPosition;

	public Matrix matYaw;

	public Matrix matPitch;

	public Matrix matView;

	private static Vector3 VecUnitX = Vector3.UnitX;

	private static Vector3 VecUnitY = Vector3.UnitY;

	private static Vector3 VecUnitZ = Vector3.UnitZ;

	public virtual void LoadContent()
	{
	}

	public virtual void Update(GameTime gameTime)
	{
		float num = (float)gameTime.ElapsedGameTime.Milliseconds * 0.001f;
		float num2 = 0f;
		float num3 = 0f;
		num2 = InputBase.CurrentState(EndGameEngine.controllingPlayer.Value).ThumbSticks.Right.X;
		num3 = InputBase.CurrentState(EndGameEngine.controllingPlayer.Value).ThumbSticks.Right.Y;
		Matrix.CreateRotationY(MathHelper.ToRadians(0f - num2), out matYaw);
		Vector3.Transform(ref vecDirection, ref matYaw, out vecDirection);
		Vector3.Cross(ref VecUnitY, ref vecDirection, out vecRight);
		Matrix.CreateFromAxisAngle(ref vecRight, MathHelper.ToRadians(0f - num3), out matPitch);
		Vector3.Transform(ref vecDirection, ref matPitch, out vecDirection);
		Vector3.Cross(ref vecRight, ref vecDirection, out vecUp);
		vecPosition += vecDirection * (InputBase.CurrentState(EndGameEngine.controllingPlayer.Value).ThumbSticks.Left.Y * 100f * num);
		matView = Matrix.CreateLookAt(vecPosition, vecPosition + vecDirection * 1000f, Vector3.Up);
	}

	public virtual void Draw(Model m)
	{
	}
}
