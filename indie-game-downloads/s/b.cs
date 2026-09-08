using System;
using _0002;
using _000E;
using _0014;
using E;
using Microsoft.Xna.Framework;
using N;
using P;
using m;
using r;
using y;

namespace s;

internal abstract class b : h
{
	protected internal E.h entity;

	protected internal N._0006 worldTransform;

	protected internal Vector3 localPosition;

	protected internal _0002.y<b> events;

	public new _000E.b Shape
	{
		get
		{
			return (_000E.b)a5h;
		}
		protected set
		{
			base.Shape = value;
		}
	}

	public E.h Entity
	{
		get
		{
			return entity;
		}
		protected internal set
		{
			entity = value;
			OnEntityChanged();
		}
	}

	public N._0006 WorldTransform
	{
		get
		{
			return worldTransform;
		}
		set
		{
			UpdateWorldTransform(ref value.Position, ref value.Orientation);
		}
	}

	protected internal override bool IsActive
	{
		get
		{
			if (entity == null)
			{
				return false;
			}
			return entity.a5L.IsActive;
		}
	}

	public Vector3 LocalPosition
	{
		get
		{
			return localPosition;
		}
		set
		{
			localPosition = value;
		}
	}

	public _0002.y<b> Events
	{
		get
		{
			return events;
		}
		set
		{
			events = value;
		}
	}

	protected internal override _0002._000E EventTriggerer => events;

	public P.a OverlappedEntities => new P.a(this);

	protected b()
	{
		Events = new _0002.y<b>(this);
	}

	protected b(_000E.b P_0)
		: this()
	{
		base.Shape = P_0;
	}

	protected virtual void OnEntityChanged()
	{
	}

	public override void UpdateBoundingBox()
	{
		UpdateBoundingBox(0f);
	}

	public override void UpdateBoundingBox(float dt)
	{
		UpdateWorldTransform(ref entity.a5h, ref entity.a5b);
		UpdateBoundingBoxInternal(dt);
	}

	public virtual void UpdateWorldTransform(ref Vector3 position, ref Quaternion orientation)
	{
		Vector3.Transform(ref localPosition, ref orientation, out worldTransform.Position);
		Vector3.Add(ref worldTransform.Position, ref position, out worldTransform.Position);
		worldTransform.Orientation = orientation;
	}

	public void UpdateBoundingBoxForTransform(ref N._0006 transform, float dt)
	{
		worldTransform = transform;
		UpdateBoundingBoxInternal(dt);
	}

	protected internal abstract void UpdateBoundingBoxInternal(float dt);

	internal void bh(ref BoundingBox P_0, float P_1)
	{
		if (!(P_1 > 0f))
		{
			return;
		}
		bool flag = _0014._6.UseExtraExpansionForContinuousBoundingBoxes && entity.PositionUpdateMode == m._7.Continuous;
		float num = ((!flag) ? 1 : 2);
		if (entity.a5a.X > 0f)
		{
			P_0.Max.X += entity.a5a.X * P_1 * num;
		}
		else
		{
			P_0.Min.X += entity.a5a.X * P_1 * num;
		}
		if (entity.a5a.Y > 0f)
		{
			P_0.Max.Y += entity.a5a.Y * P_1 * num;
		}
		else
		{
			P_0.Min.Y += entity.a5a.Y * P_1 * num;
		}
		if (entity.a5a.Z > 0f)
		{
			P_0.Max.Z += entity.a5a.Z * P_1 * num;
		}
		else
		{
			P_0.Min.Z += entity.a5a.Z * P_1 * num;
		}
		if (!flag)
		{
			return;
		}
		float num2 = 0f;
		foreach (E.h overlappedEntity in OverlappedEntities)
		{
			float num3 = overlappedEntity.a5a.LengthSquared();
			if (num3 > num2)
			{
				num2 = num3;
			}
		}
		num2 = (float)Math.Sqrt(num2) * P_1;
		P_0.Min.X -= num2;
		P_0.Min.Y -= num2;
		P_0.Min.Z -= num2;
		P_0.Max.X += num2;
		P_0.Max.Y += num2;
		P_0.Max.Z += num2;
	}

	protected override void CollisionRulesUpdated()
	{
		if (entity != null)
		{
			entity.a5L.Activate();
		}
	}
}
internal class B<T> : v where T : y.h
{
	public new T Shape => (T)a5h;

	public B(T shape)
		: base(shape)
	{
	}

	public override bool RayCast(Ray ray, float maximumLength, out r._0006 rayHit)
	{
		T shape = Shape;
		return shape.RayTest(ref ray, ref worldTransform, maximumLength, out rayHit);
	}

	protected internal override void UpdateBoundingBoxInternal(float dt)
	{
		T shape = Shape;
		shape.GetBoundingBox(ref worldTransform, out boundingBox);
		bh(ref boundingBox, dt);
	}
}
