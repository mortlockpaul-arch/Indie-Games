using System;
using _0002;
using _000E;
using I;
using Microsoft.Xna.Framework;
using N;
using T;
using Y;
using i;
using l;
using p;
using q;
using r;
using y;

namespace P;

internal class _0006 : h, r.h, T.h
{
	internal new N.h a5h;

	internal bool a5b = true;

	protected internal _0002.y<_0006> events;

	internal new T._6 a56;

	internal float a5a;

	private readonly Action<T._6> a57;

	private r.a a5_0006;

	public new _000E.v Shape
	{
		get
		{
			return (_000E.v)base.a5h;
		}
		set
		{
			base.Shape = value;
		}
	}

	public N.h WorldTransform
	{
		get
		{
			return a5h;
		}
		set
		{
			a5h = value;
		}
	}

	public bool ImproveBoundaryBehavior
	{
		get
		{
			return a5b;
		}
		set
		{
			a5b = value;
		}
	}

	public _0002.y<_0006> Events => events;

	protected internal override _0002._000E EventTriggerer => events;

	public T._6 Material
	{
		get
		{
			return a56;
		}
		set
		{
			if (a56 != null)
			{
				a56.MaterialChanged -= a57;
			}
			a56 = value;
			if (a56 != null)
			{
				a56.MaterialChanged += a57;
			}
			bn(a56);
		}
	}

	public float Thickness
	{
		get
		{
			return a5a;
		}
		set
		{
			if (value < 0f)
			{
				throw new Exception("Cannot use a negative thickness value.");
			}
			Vector3 vector = Vector3.Normalize(a5h.LinearTransform.Down);
			Vector3 vector2 = vector * (value - a5a);
			if (vector.X < 0f)
			{
				boundingBox.Min.X += vector2.X;
			}
			else
			{
				boundingBox.Max.X += vector2.X;
			}
			if (vector.Y < 0f)
			{
				boundingBox.Min.Y += vector2.Y;
			}
			else
			{
				boundingBox.Max.Y += vector2.Y;
			}
			if (vector.Z < 0f)
			{
				boundingBox.Min.Z += vector2.Z;
			}
			else
			{
				boundingBox.Max.Z += vector2.Z;
			}
			a5a = value;
		}
	}

	protected internal override bool IsActive => false;

	r.a r.h.Space
	{
		get
		{
			return a5_0006;
		}
		set
		{
			a5_0006 = value;
		}
	}

	public r.a Space => a5_0006;

	private void bn(T._6 P_0)
	{
		for (int i = 0; i < base.a56.Count; i++)
		{
			base.a56[i].UpdateMaterialProperties();
		}
	}

	public _0006(_000E.v shape, N.h worldTransform)
	{
		a5h = worldTransform;
		Shape = shape;
		((Y.a)this).a56.a5a = q._7.DefaultKinematicCollisionGroup;
		a56 = new T._6();
		a57 = bn;
		a56.MaterialChanged += a57;
		events = new _0002.y<_0006>(this);
	}

	public _0006(float[,] heights, N.h worldTransform)
		: this(new _000E.v(heights), worldTransform)
	{
	}

	protected override void OnShapeChanged(_000E.h collisionShape)
	{
		UpdateBoundingBox();
	}

	public override void UpdateBoundingBox()
	{
		Shape.GetBoundingBox(ref a5h, out boundingBox);
		Vector3 vector = Vector3.Normalize(a5h.LinearTransform.Down) * a5a;
		if (vector.X < 0f)
		{
			boundingBox.Min.X += vector.X;
		}
		else
		{
			boundingBox.Max.X += vector.X;
		}
		if (vector.Y < 0f)
		{
			boundingBox.Min.Y += vector.Y;
		}
		else
		{
			boundingBox.Max.Y += vector.Y;
		}
		if (vector.Z < 0f)
		{
			boundingBox.Min.Z += vector.Z;
		}
		else
		{
			boundingBox.Max.Z += vector.Z;
		}
	}

	public override bool RayCast(Ray ray, float maximumLength, out r._0006 rayHit)
	{
		return Shape.RayCast(ref ray, maximumLength, ref a5h, out rayHit);
	}

	public override bool ConvexCast(y.h castShape, ref N._0006 startingTransform, ref Vector3 sweep, out r._0006 hit)
	{
		hit = default(r._0006);
		castShape.GetSweptLocalBoundingBox(ref startingTransform, ref a5h, ref sweep, out var localSpaceBoundingBox);
		y.v triangle = p._6.GetTriangle();
		l._7<global::i._0006._00065b> triangleIndicesList = p._6.GetTriangleIndicesList();
		if (Shape.GetOverlaps(localSpaceBoundingBox, triangleIndicesList))
		{
			hit.T = float.MaxValue;
			for (int i = 0; i < triangleIndicesList.a5h; i++)
			{
				Shape.GetTriangle(ref triangleIndicesList.Elements[i], ref a5h, out triangle.a5h, out triangle.a5b, out triangle.a56);
				Vector3.Add(ref triangle.a5h, ref triangle.a5b, out var result);
				Vector3.Add(ref result, ref triangle.a56, out result);
				Vector3.Multiply(ref result, 1f / 3f, out result);
				Vector3.Subtract(ref triangle.a5h, ref result, out triangle.a5h);
				Vector3.Subtract(ref triangle.a5b, ref result, out triangle.a5b);
				Vector3.Subtract(ref triangle.a56, ref result, out triangle.a56);
				triangle.maximumRadius = triangle.a5h.LengthSquared();
				float num = triangle.a5b.LengthSquared();
				if (triangle.maximumRadius < num)
				{
					triangle.maximumRadius = num;
				}
				num = triangle.a56.LengthSquared();
				if (triangle.maximumRadius < num)
				{
					triangle.maximumRadius = num;
				}
				triangle.maximumRadius = (float)Math.Sqrt(triangle.maximumRadius);
				triangle.collisionMargin = 0f;
				N._0006 transformB = new N._0006
				{
					Orientation = Quaternion.Identity,
					Position = result
				};
				if (I.v.Sweep(castShape, triangle, ref sweep, ref r.X.ZeroVector, ref startingTransform, ref transformB, out var hit2) && hit2.T < hit.T)
				{
					hit = hit2;
				}
			}
			triangle.maximumRadius = 0f;
			p._6.GiveBack(triangle);
			p._6.GiveBack(triangleIndicesList);
			return hit.T != float.MaxValue;
		}
		p._6.GiveBack(triangle);
		p._6.GiveBack(triangleIndicesList);
		return false;
	}

	public void GetNormal(int i, int j, out Vector3 normal)
	{
		Shape.GetNormal(i, j, ref a5h, out normal);
	}

	public void GetPosition(int i, int j, out Vector3 position)
	{
		Shape.GetPosition(i, j, ref a5h, out position);
	}

	void r.h.OnAdditionToSpace(r.a newSpace)
	{
	}

	void r.h.OnRemovalFromSpace(r.a oldSpace)
	{
	}
}
