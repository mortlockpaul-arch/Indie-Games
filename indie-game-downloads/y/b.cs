using System;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using N;
using d;
using l;
using q;
using r;
using s;

namespace y
{
	internal class b : h
	{
		private float a5h;

		private float a5b;

		public float Radius
		{
			get
			{
				return a5b;
			}
			set
			{
				a5b = value;
				OnShapeChanged();
			}
		}

		public float Height
		{
			get
			{
				return a5h * 2f;
			}
			set
			{
				a5h = value / 2f;
				OnShapeChanged();
			}
		}

		public b(float height, float radius)
		{
			a5h = height * 0.5f;
			Radius = radius;
		}

		public override void GetBoundingBox(ref N._0006 shapeTransform, out BoundingBox boundingBox)
		{
			boundingBox = default(BoundingBox);
			N._7.CreateFromQuaternion(ref shapeTransform.Orientation, out var result);
			Vector3 direction = new Vector3(result.M11, result.M21, result.M31);
			GetLocalExtremePointWithoutMargin(ref direction, out var extremePoint);
			direction = new Vector3(result.M12, result.M22, result.M32);
			GetLocalExtremePointWithoutMargin(ref direction, out var extremePoint2);
			direction = new Vector3(result.M13, result.M23, result.M33);
			GetLocalExtremePointWithoutMargin(ref direction, out var extremePoint3);
			N._7.Transform(ref extremePoint, ref result, out extremePoint);
			N._7.Transform(ref extremePoint2, ref result, out extremePoint2);
			N._7.Transform(ref extremePoint3, ref result, out extremePoint3);
			boundingBox.Max.X = shapeTransform.Position.X + collisionMargin + extremePoint.X;
			boundingBox.Max.Y = shapeTransform.Position.Y + collisionMargin + extremePoint2.Y;
			boundingBox.Max.Z = shapeTransform.Position.Z + collisionMargin + extremePoint3.Z;
			boundingBox.Min.X = shapeTransform.Position.X - collisionMargin - extremePoint.X;
			boundingBox.Min.Y = shapeTransform.Position.Y - collisionMargin - extremePoint2.Y;
			boundingBox.Min.Z = shapeTransform.Position.Z - collisionMargin - extremePoint3.Z;
		}

		public override void GetLocalExtremePointWithoutMargin(ref Vector3 direction, out Vector3 extremePoint)
		{
			float num = direction.X * direction.X + direction.Z * direction.Z;
			if (num > 1E-07f)
			{
				float num2 = (a5b - collisionMargin) / (float)Math.Sqrt(num);
				extremePoint = new Vector3(direction.X * num2, (float)Math.Sign(direction.Y) * (a5h - collisionMargin), direction.Z * num2);
			}
			else
			{
				extremePoint = new Vector3(0f, (float)Math.Sign(direction.Y) * (a5h - collisionMargin), 0f);
			}
		}

		public override float ComputeMaximumRadius()
		{
			return (float)Math.Sqrt(a5b * a5b + a5h * a5h);
		}

		public override float ComputeMinimumRadius()
		{
			return Math.Min(a5b, a5h);
		}

		public override N._7 ComputeVolumeDistribution(out float volume)
		{
			volume = ComputeVolume();
			N._7 result = default(N._7);
			float m = (result.M11 = 1f / 12f * Height * Height + 0.25f * Radius * Radius);
			result.M22 = 0.5f * Radius * Radius;
			result.M33 = m;
			return result;
		}

		public override Vector3 ComputeCenter()
		{
			return Vector3.Zero;
		}

		public override Vector3 ComputeCenter(out float volume)
		{
			volume = ComputeVolume();
			return ComputeCenter();
		}

		public override float ComputeVolume()
		{
			return (float)(Math.PI * (double)Radius * (double)Radius * (double)Height);
		}

		public override s.b GetCollidableInstance()
		{
			return new s.B<b>(this);
		}
	}
}
namespace Y
{
	internal abstract class b : r.b
	{
		private readonly d.v a5h = new d.v();

		private readonly l._7<_7> a5b = new l._7<_7>();

		[CompilerGenerated]
		private object a56;

		[CompilerGenerated]
		private _0006 a5a;

		public object Locker
		{
			[CompilerGenerated]
			get
			{
				return a56;
			}
			[CompilerGenerated]
			protected set
			{
				a56 = value;
			}
		}

		public l._7<_7> Overlaps => a5b;

		public _0006 QueryAccelerator
		{
			[CompilerGenerated]
			get
			{
				return a5a;
			}
			[CompilerGenerated]
			protected set
			{
				a5a = value;
			}
		}

		protected b()
		{
			Locker = new object();
			Enabled = true;
		}

		protected b(d.b P_0)
			: this()
		{
			base.ThreadManager = P_0;
			base.AllowMultithreading = true;
		}

		public abstract void Add(a entry);

		public abstract void Remove(a entry);

		protected internal void AddOverlap(_7 overlap)
		{
			a5h.Enter();
			a5b.Add(overlap);
			a5h.Exit();
		}

		protected internal void TryToAddOverlap(a entryA, a entryB)
		{
			q.a collisionRule;
			if ((collisionRule = GetCollisionRule(entryA, entryB)) < q.a.NoBroadPhase)
			{
				a5h.Enter();
				a5b.Add(new _7(entryA, entryB, collisionRule));
				a5h.Exit();
			}
		}

		protected internal q.a GetCollisionRule(a entryA, a entryB)
		{
			if (entryA.IsActive || entryB.IsActive)
			{
				return q._7.a5v(entryA.a56, entryB.a56);
			}
			return q.a.NoBroadPhase;
		}
	}
}
