using System;
using Microsoft.Xna.Framework;
using N;
using q;
using r;
using s;

namespace Y
{
	internal struct _7 : IEquatable<_7>
	{
		internal a a5h;

		internal a a5b;

		internal q.a a56;

		public a EntryA => a5h;

		public a EntryB => a5b;

		public q.a CollisionRule => a56;

		public _7(a entryA, a entryB)
		{
			a5h = entryA;
			a5b = entryB;
			a56 = q._7.DefaultCollisionRule;
		}

		public _7(a entryA, a entryB, q.a collisionRule)
		{
			a5h = entryA;
			a5b = entryB;
			a56 = collisionRule;
		}

		public override int GetHashCode()
		{
			return (int)((a5h.a5h + a5b.a5h) * 3625334849u);
		}

		public bool Equals(_7 other)
		{
			if (other.a5h != a5h || other.a5b != a5b)
			{
				if (other.a5h == a5b)
				{
					return other.a5b == a5h;
				}
				return false;
			}
			return true;
		}

		public override string ToString()
		{
			return string.Concat("{", a5h, ", ", a5b, "}");
		}
	}
}
namespace y
{
	internal class _7 : h
	{
		public float Radius
		{
			get
			{
				return collisionMargin;
			}
			set
			{
				base.CollisionMargin = value;
			}
		}

		public _7(float radius)
		{
			Radius = radius;
		}

		public override void GetBoundingBox(ref N._0006 shapeTransform, out BoundingBox boundingBox)
		{
			boundingBox = default(BoundingBox);
			boundingBox.Min.X = shapeTransform.Position.X - collisionMargin;
			boundingBox.Min.Y = shapeTransform.Position.Y - collisionMargin;
			boundingBox.Min.Z = shapeTransform.Position.Z - collisionMargin;
			boundingBox.Max.X = shapeTransform.Position.X + collisionMargin;
			boundingBox.Max.Y = shapeTransform.Position.Y + collisionMargin;
			boundingBox.Max.Z = shapeTransform.Position.Z + collisionMargin;
		}

		public override void GetLocalExtremePointWithoutMargin(ref Vector3 direction, out Vector3 extremePoint)
		{
			extremePoint = r.X.ZeroVector;
		}

		public override float ComputeMaximumRadius()
		{
			return Radius;
		}

		public override float ComputeMinimumRadius()
		{
			return Radius;
		}

		public override N._7 ComputeVolumeDistribution(out float volume)
		{
			N._7 result = default(N._7);
			result.M33 = (result.M22 = (result.M11 = 0.4f * Radius * Radius));
			volume = ComputeVolume();
			return result;
		}

		public override bool RayTest(ref Ray ray, ref N._0006 transform, float maximumLength, out r._0006 hit)
		{
			return r.X.RayCastSphere(ref ray, ref transform.Position, collisionMargin, maximumLength, out hit);
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
			return (float)(4.18878915758884 * (double)Radius * (double)Radius * (double)Radius);
		}

		public override s.b GetCollidableInstance()
		{
			return new s.B<_7>(this);
		}
	}
}
