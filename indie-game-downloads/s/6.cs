using System;
using System.Collections.Generic;
using _000E;
using Microsoft.Xna.Framework;
using N;
using T;
using Y;
using l;
using p;
using r;
using y;

namespace s;

internal class _6 : b
{
	internal new l._7<_7> a5h = new l._7<_7>();

	internal _0006 a5b;

	public new _000E.a Shape
	{
		get
		{
			return (_000E.a)base.a5h;
		}
		protected internal set
		{
			base.Shape = value;
		}
	}

	public l.X<_7> Children => new l.X<_7>(a5h);

	public _0006 Hierarchy => a5b;

	protected override void OnEntityChanged()
	{
		for (int i = 0; i < a5h.a5h; i++)
		{
			a5h.Elements[i].CollisionInformation.Entity = entity;
			if (a5h.Elements[i].Material == null)
			{
				a5h.Elements[i].Material = entity.a5Z;
			}
		}
		base.OnEntityChanged();
	}

	private _7 bM(a P_0, int P_1)
	{
		b collidableInstance = P_0.Entry.Shape.GetCollidableInstance();
		if (P_0.Events != null)
		{
			collidableInstance.events = P_0.Events;
		}
		if (P_0.CollisionRules != null)
		{
			((Y.a)collidableInstance).a56 = P_0.CollisionRules;
		}
		collidableInstance.Tag = P_0.Tag;
		if (P_0.Material == null)
		{
			P_0.Material = new T._6();
		}
		return new _7(Shape, collidableInstance, P_0.Material, P_1);
	}

	private _7 bM(_000E._6 P_0, int P_1)
	{
		b collidableInstance = P_0.Shape.GetCollidableInstance();
		return new _7(Shape, collidableInstance, P_1);
	}

	internal _6()
	{
		a5b = new _0006(this);
	}

	public _6(IList<a> children)
	{
		l._7<_000E._6> obj = new l._7<_000E._6>();
		for (int i = 0; i < children.Count; i++)
		{
			obj.Add(children[i].Entry);
		}
		base.Shape = new _000E.a(obj);
		for (int j = 0; j < children.Count; j++)
		{
			a5h.Add(bM(children[j], j));
		}
		a5b = new _0006(this);
	}

	public _6(IList<a> children, out Vector3 center)
	{
		l._7<_000E._6> obj = new l._7<_000E._6>();
		for (int i = 0; i < children.Count; i++)
		{
			obj.Add(children[i].Entry);
		}
		base.Shape = new _000E.a(obj, out center);
		for (int j = 0; j < children.Count; j++)
		{
			a5h.Add(bM(children[j], j));
		}
		a5b = new _0006(this);
	}

	public _6(_000E.a compoundShape)
		: base(compoundShape)
	{
		for (int i = 0; i < compoundShape.a5h.a5h; i++)
		{
			_7 item = bM(compoundShape.a5h.Elements[i], i);
			a5h.Add(item);
		}
		a5b = new _0006(this);
	}

	public override void UpdateWorldTransform(ref Vector3 position, ref Quaternion orientation)
	{
		base.UpdateWorldTransform(ref position, ref orientation);
		l._7<_000E._6> obj = Shape.a5h;
		for (int i = 0; i < a5h.a5h; i++)
		{
			N._0006.Transform(ref obj.Elements[a5h.Elements[i].a5b].LocalTransform, ref worldTransform, out var combined);
			a5h.Elements[i].CollisionInformation.UpdateWorldTransform(ref combined.Position, ref combined.Orientation);
		}
	}

	protected internal override void UpdateBoundingBoxInternal(float dt)
	{
		for (int i = 0; i < a5h.a5h; i++)
		{
			a5h.Elements[i].CollisionInformation.UpdateBoundingBoxInternal(dt);
		}
		a5b.Tree.Refit();
		boundingBox = a5b.Tree.BoundingBox;
	}

	public override bool RayCast(Ray ray, float maximumLength, out r._0006 rayHit)
	{
		rayHit = default(r._0006);
		l._7<_7> compoundChildList = p._6.GetCompoundChildList();
		if (a5b.Tree.GetOverlaps(ray, maximumLength, compoundChildList))
		{
			rayHit.T = float.MaxValue;
			for (int i = 0; i < compoundChildList.a5h; i++)
			{
				if (compoundChildList.Elements[i].CollisionInformation.RayCast(ray, maximumLength, out var rayHit2) && rayHit2.T < rayHit.T)
				{
					rayHit = rayHit2;
				}
			}
			p._6.GiveBack(compoundChildList);
			return rayHit.T != float.MaxValue;
		}
		p._6.GiveBack(compoundChildList);
		return false;
	}

	public override bool RayCast(Ray ray, float maximumLength, Func<Y.a, bool> filter, out r._0006 rayHit)
	{
		rayHit = default(r._0006);
		if (filter(this))
		{
			l._7<_7> compoundChildList = p._6.GetCompoundChildList();
			if (a5b.Tree.GetOverlaps(ray, maximumLength, compoundChildList))
			{
				rayHit.T = float.MaxValue;
				for (int i = 0; i < compoundChildList.a5h; i++)
				{
					if (compoundChildList.Elements[i].CollisionInformation.RayCast(ray, maximumLength, filter, out var rayHit2) && rayHit2.T < rayHit.T)
					{
						rayHit = rayHit2;
					}
				}
				p._6.GiveBack(compoundChildList);
				return rayHit.T != float.MaxValue;
			}
			p._6.GiveBack(compoundChildList);
		}
		return false;
	}

	public bool RayCast(Ray ray, float maximumLength, out r._7 rayHit)
	{
		rayHit = default(r._7);
		l._7<_7> compoundChildList = p._6.GetCompoundChildList();
		if (a5b.Tree.GetOverlaps(ray, maximumLength, compoundChildList))
		{
			rayHit.HitData.T = float.MaxValue;
			for (int i = 0; i < compoundChildList.a5h; i++)
			{
				b collisionInformation = compoundChildList.Elements[i].CollisionInformation;
				if (collisionInformation.RayCast(ray, maximumLength, out var rayHit2) && rayHit2.T < rayHit.HitData.T)
				{
					rayHit.HitData = rayHit2;
					rayHit.HitObject = collisionInformation;
				}
			}
			p._6.GiveBack(compoundChildList);
			return rayHit.HitData.T != float.MaxValue;
		}
		p._6.GiveBack(compoundChildList);
		return false;
	}

	public bool RayCast(Ray ray, float maximumLength, out r._0006 rayHit, out _7 hitChild)
	{
		rayHit = default(r._0006);
		hitChild = null;
		l._7<_7> compoundChildList = p._6.GetCompoundChildList();
		if (a5b.Tree.GetOverlaps(ray, maximumLength, compoundChildList))
		{
			rayHit.T = float.MaxValue;
			for (int i = 0; i < compoundChildList.a5h; i++)
			{
				b collisionInformation = compoundChildList.Elements[i].CollisionInformation;
				if (collisionInformation.RayCast(ray, maximumLength, out var rayHit2) && rayHit2.T < rayHit.T)
				{
					rayHit = rayHit2;
					hitChild = compoundChildList.Elements[i];
				}
			}
			p._6.GiveBack(compoundChildList);
			return rayHit.T != float.MaxValue;
		}
		p._6.GiveBack(compoundChildList);
		return false;
	}

	public override bool ConvexCast(y.h castShape, ref N._0006 startingTransform, ref Vector3 sweep, out r._0006 hit)
	{
		hit = default(r._0006);
		r.X.GetExpandedBoundingBox(ref castShape, ref startingTransform, ref sweep, out var boundingBox);
		l._7<_7> compoundChildList = p._6.GetCompoundChildList();
		if (a5b.Tree.GetOverlaps(boundingBox, compoundChildList))
		{
			hit.T = float.MaxValue;
			for (int i = 0; i < compoundChildList.a5h; i++)
			{
				b collisionInformation = compoundChildList.Elements[i].CollisionInformation;
				if (collisionInformation.ConvexCast(castShape, ref startingTransform, ref sweep, out var hit2) && hit2.T < hit.T)
				{
					hit = hit2;
				}
			}
			p._6.GiveBack(compoundChildList);
			return hit.T != float.MaxValue;
		}
		p._6.GiveBack(compoundChildList);
		return false;
	}
}
