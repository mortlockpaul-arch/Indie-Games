using Microsoft.Xna.Framework;

namespace Quasar.Elements.Cameras;

public class InvertedCamera : Camera
{
	private Camera source;

	private Plane plane;

	private Matrix reflection;

	private Matrix scale;

	public InvertedCamera(Camera source, Plane plane)
	{
		this.source = source;
		projection = source.Projection;
		scale = Matrix.CreateScale(-1f, 1f, 1f);
		SetPlane(plane);
	}

	public InvertedCamera(Camera source)
		: this(source, new Plane(Vector3.Up, 0f))
	{
	}

	public void SetPlane(Plane plane)
	{
		this.plane = plane;
		reflection = Matrix.CreateReflection(plane);
	}

	protected override void DoUpdate()
	{
		transform.Matrix = reflection * source.Transform * scale;
		base.DoUpdate();
	}
}
