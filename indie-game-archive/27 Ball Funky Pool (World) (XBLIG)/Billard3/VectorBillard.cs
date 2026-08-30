using Microsoft.Xna.Framework;

namespace Billard3;

public class VectorBillard
{
	private Vector3 value;

	private Vector2 p2D = Vector2.Zero;

	private float len;

	public float Len => len;

	public Vector3 Value => value;

	public Vector2 Value2D => p2D;

	public VectorBillard(Vector3 p)
	{
		value = p;
		Update();
	}

	public VectorBillard()
		: this(Vector3.Zero)
	{
	}

	public VectorBillard(Vector2 p)
		: this(new Vector3(p.X, 0f, p.Y))
	{
	}

	private void Update()
	{
		p2D.X = value.X;
		p2D.Y = value.Z;
		len = value.Length();
	}

	public void Set(Vector3 p)
	{
		value = p;
		Update();
	}

	public void Set(Vector2 p)
	{
		Set(new Vector3(p.X, value.Y, p.Y));
	}
}
