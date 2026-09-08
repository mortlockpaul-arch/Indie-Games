using Microsoft.Xna.Framework;

namespace Deep_waters;

public class Camera
{
	public Matrix Projection;

	public Matrix View;

	public Vector3 position = new Vector3(-300f, 0f, 0f);

	public Vector3 lookat = new Vector3(0f, 0f, 0f);

	public float vid;

	public float ar;

	public Vector3 speed;

	public BoundingFrustum frustum;

	public Camera(float visibledistance, float AspectRatio)
	{
		vid = visibledistance;
		ar = AspectRatio;
		View = Matrix.CreateLookAt(position, lookat, Vector3.Up);
		Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45f), AspectRatio, 0.1f, visibledistance);
		frustum = new BoundingFrustum(View * Projection);
		speed = new Vector3(0f, 0f, 0f);
	}

	public virtual void update(GameTime gameTime)
	{
		View = Matrix.CreateLookAt(position, lookat, Vector3.Up);
		Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45f), ar, 0.1f, vid);
		frustum = new BoundingFrustum(View * Projection);
	}
}
