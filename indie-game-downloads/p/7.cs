using System;
using _0002;
using _000E;
using I;
using Microsoft.Xna.Framework;
using N;
using T;
using Y;
using l;
using p;
using q;
using r;
using y;

namespace P;

internal class _7 : h, r.h, T.h
{
	private new l.y a5h;

	internal y._0006 a5b;

	internal new bool a56 = true;

	protected internal _0002.y<_7> events;

	internal T._6 a5a;

	private Action<T._6> a57;

	private r.a a5_0006;

	public l.y Mesh => a5h;

	public N.h WorldTransform
	{
		get
		{
			return ((l._0002)a5h.Data).a5h;
		}
		set
		{
			((l._0002)a5h.Data).WorldTransform = value;
			a5h.Tree.Refit();
			UpdateBoundingBox();
		}
	}

	public new _000E._0006 Shape => (_000E._0006)base.a5h;

	public y._0006 Sidedness
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

	public bool ImproveBoundaryBehavior
	{
		get
		{
			return a56;
		}
		set
		{
			a56 = value;
		}
	}

	public _0002.y<_7> Events => events;

	protected internal override _0002._000E EventTriggerer => events;

	public T._6 Material
	{
		get
		{
			return a5a;
		}
		set
		{
			if (a5a != null)
			{
				a5a.MaterialChanged -= a57;
			}
			a5a = value;
			if (a5a != null)
			{
				a5a.MaterialChanged += a57;
			}
			bn(a5a);
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

	public _7(Vector3[] vertices, int[] indices)
	{
		base.Shape = new _000E._0006(vertices, indices);
		((Y.a)this).a56.a5a = q._7.DefaultKinematicCollisionGroup;
		events = new _0002.y<_7>(this);
		a5a = new T._6();
		a57 = bn;
		a5a.MaterialChanged += a57;
	}

	public _7(Vector3[] vertices, int[] indices, N.h worldTransform)
	{
		base.Shape = new _000E._0006(vertices, indices, worldTransform);
		((Y.a)this).a56.a5a = q._7.DefaultKinematicCollisionGroup;
		events = new _0002.y<_7>(this);
		a5a = new T._6();
		a57 = bn;
		a5a.MaterialChanged += a57;
	}

	protected override void OnShapeChanged(_000E.h collisionShape)
	{
		a5h = new l.y(Shape.TriangleMeshData);
		UpdateBoundingBox();
	}

	public override void UpdateBoundingBox()
	{
		boundingBox = a5h.Tree.BoundingBox;
	}

	private void bn(T._6 P_0)
	{
		for (int i = 0; i < base.a56.Count; i++)
		{
			base.a56[i].UpdateMaterialProperties();
		}
	}

	public override bool RayCast(Ray ray, float maximumLength, out r._0006 rayHit)
	{
		return a5h.RayCast(ray, maximumLength, a5b, out rayHit);
	}

	public override bool ConvexCast(y.h castShape, ref N._0006 startingTransform, ref Vector3 sweep, out r._0006 hit)
	{
		hit = default(r._0006);
		r.X.GetExpandedBoundingBox(ref castShape, ref startingTransform, ref sweep, out var boundingBox);
		y.v triangle = p._6.GetTriangle();
		l._7<int> intList = p._6.GetIntList();
		if (Mesh.Tree.GetOverlaps(boundingBox, intList))
		{
			hit.T = float.MaxValue;
			for (int i = 0; i < intList.Count; i++)
			{
				a5h.Data.GetTriangle(intList[i], out triangle.a5h, out triangle.a5b, out triangle.a56);
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
			p._6.GiveBack(intList);
			return hit.T != float.MaxValue;
		}
		p._6.GiveBack(triangle);
		p._6.GiveBack(intList);
		return false;
	}

	public bool RayCast(Ray ray, float maximumLength, y._0006 sidedness, out r._0006 rayHit)
	{
		return a5h.RayCast(ray, maximumLength, sidedness, out rayHit);
	}

	void r.h.OnAdditionToSpace(r.a newSpace)
	{
	}

	void r.h.OnRemovalFromSpace(r.a oldSpace)
	{
	}
}
