using System;
using _000E;
using _0014;
using _0017;
using Microsoft.Xna.Framework;
using N;
using r;

namespace y
{
	internal abstract class h : _000E.b
	{
		protected internal float collisionMargin = _0014.h.DefaultMargin;

		protected internal float minimumRadius;

		protected internal float maximumRadius;

		public float CollisionMargin
		{
			get
			{
				return collisionMargin;
			}
			set
			{
				if (value < 0f)
				{
					throw new Exception("Collision margin must be nonnegative..");
				}
				collisionMargin = value;
				OnShapeChanged();
			}
		}

		public float MinimumRadius
		{
			get
			{
				return minimumRadius;
			}
			set
			{
				minimumRadius = value;
			}
		}

		public float MaximumRadius => maximumRadius;

		public abstract void GetLocalExtremePointWithoutMargin(ref Vector3 direction, out Vector3 extremePoint);

		public void GetExtremePointWithoutMargin(Vector3 direction, ref N._0006 shapeTransform, out Vector3 extremePoint)
		{
			Quaternion.Conjugate(ref shapeTransform.Orientation, out var result);
			Vector3.Transform(ref direction, ref result, out direction);
			GetLocalExtremePointWithoutMargin(ref direction, out extremePoint);
			Vector3.Transform(ref extremePoint, ref shapeTransform.Orientation, out extremePoint);
			Vector3.Add(ref extremePoint, ref shapeTransform.Position, out extremePoint);
		}

		public void GetExtremePoint(Vector3 direction, ref N._0006 shapeTransform, out Vector3 extremePoint)
		{
			GetExtremePointWithoutMargin(direction, ref shapeTransform, out extremePoint);
			float num = direction.LengthSquared();
			if (num > 1E-07f)
			{
				Vector3.Multiply(ref direction, collisionMargin / (float)Math.Sqrt(num), out direction);
				Vector3.Add(ref extremePoint, ref direction, out extremePoint);
			}
		}

		public void GetLocalExtremePoint(Vector3 direction, out Vector3 extremePoint)
		{
			GetLocalExtremePointWithoutMargin(ref direction, out extremePoint);
			float num = direction.LengthSquared();
			if (num > 1E-07f)
			{
				Vector3.Multiply(ref direction, collisionMargin / (float)Math.Sqrt(num), out direction);
				Vector3.Add(ref extremePoint, ref direction, out extremePoint);
			}
		}

		public virtual void GetBoundingBox(ref N._0006 shapeTransform, out BoundingBox boundingBox)
		{
			boundingBox = default(BoundingBox);
			N._7.CreateFromQuaternion(ref shapeTransform.Orientation, out var result);
			Vector3 direction = new Vector3(result.M11, result.M21, result.M31);
			GetLocalExtremePointWithoutMargin(ref direction, out var extremePoint);
			direction = new Vector3(0f - result.M11, 0f - result.M21, 0f - result.M31);
			GetLocalExtremePointWithoutMargin(ref direction, out var extremePoint2);
			direction = new Vector3(result.M12, result.M22, result.M32);
			GetLocalExtremePointWithoutMargin(ref direction, out var extremePoint3);
			direction = new Vector3(0f - result.M12, 0f - result.M22, 0f - result.M32);
			GetLocalExtremePointWithoutMargin(ref direction, out var extremePoint4);
			direction = new Vector3(result.M13, result.M23, result.M33);
			GetLocalExtremePointWithoutMargin(ref direction, out var extremePoint5);
			direction = new Vector3(0f - result.M13, 0f - result.M23, 0f - result.M33);
			GetLocalExtremePointWithoutMargin(ref direction, out var extremePoint6);
			N._7.Transform(ref extremePoint, ref result, out extremePoint);
			N._7.Transform(ref extremePoint2, ref result, out extremePoint2);
			N._7.Transform(ref extremePoint3, ref result, out extremePoint3);
			N._7.Transform(ref extremePoint4, ref result, out extremePoint4);
			N._7.Transform(ref extremePoint5, ref result, out extremePoint5);
			N._7.Transform(ref extremePoint6, ref result, out extremePoint6);
			boundingBox.Max.X = shapeTransform.Position.X + collisionMargin + extremePoint.X;
			boundingBox.Max.Y = shapeTransform.Position.Y + collisionMargin + extremePoint3.Y;
			boundingBox.Max.Z = shapeTransform.Position.Z + collisionMargin + extremePoint5.Z;
			boundingBox.Min.X = shapeTransform.Position.X - collisionMargin + extremePoint2.X;
			boundingBox.Min.Y = shapeTransform.Position.Y - collisionMargin + extremePoint4.Y;
			boundingBox.Min.Z = shapeTransform.Position.Z - collisionMargin + extremePoint6.Z;
		}

		public virtual bool RayTest(ref Ray ray, ref N._0006 transform, float maximumLength, out r._0006 hit)
		{
			return _0017.h.RayCast(ray, this, ref transform, maximumLength, out hit);
		}

		public override Vector3 ComputeCenter()
		{
			return a.ComputeCenter(this);
		}

		public override Vector3 ComputeCenter(out float volume)
		{
			return a.ComputeCenter(this, out volume);
		}

		public override float ComputeVolume()
		{
			ComputeVolumeDistribution(out var volume);
			return volume;
		}

		public override N._7 ComputeVolumeDistribution(out float volume)
		{
			return a.ComputeVolumeDistribution(this, out volume);
		}

		protected override void OnShapeChanged()
		{
			base.OnShapeChanged();
			minimumRadius = ComputeMinimumRadius();
			maximumRadius = ComputeMaximumRadius();
		}

		public override N._7 ComputeVolumeDistribution()
		{
			float volume;
			return ComputeVolumeDistribution(out volume);
		}

		public override void ComputeDistributionInformation(out _000E._7 shapeInfo)
		{
			shapeInfo.VolumeDistribution = ComputeVolumeDistribution(out shapeInfo.Volume);
			shapeInfo.Center = ComputeCenter();
		}

		public void GetSweptLocalBoundingBox(ref N._0006 shapeTransform, ref N.h spaceTransform, ref Vector3 sweep, out BoundingBox boundingBox)
		{
			GetLocalBoundingBox(ref shapeTransform, ref spaceTransform, out boundingBox);
			N._7.TransformTranspose(ref sweep, ref spaceTransform.LinearTransform, out var result);
			r.X.ExpandBoundingBox(ref boundingBox, ref result);
		}

		public void GetLocalBoundingBox(ref N._0006 shapeTransform, ref N.h spaceTransform, out BoundingBox boundingBox)
		{
			boundingBox = default(BoundingBox);
			N.h.Invert(ref spaceTransform, out var inverse);
			N.h.Multiply(ref shapeTransform, ref inverse, out inverse);
			Vector3 direction = new Vector3(inverse.LinearTransform.M11, inverse.LinearTransform.M21, inverse.LinearTransform.M31);
			GetLocalExtremePoint(direction, out var extremePoint);
			direction = new Vector3(0f - inverse.LinearTransform.M11, 0f - inverse.LinearTransform.M21, 0f - inverse.LinearTransform.M31);
			GetLocalExtremePoint(direction, out var extremePoint2);
			direction = new Vector3(inverse.LinearTransform.M12, inverse.LinearTransform.M22, inverse.LinearTransform.M32);
			GetLocalExtremePoint(direction, out var extremePoint3);
			direction = new Vector3(0f - inverse.LinearTransform.M12, 0f - inverse.LinearTransform.M22, 0f - inverse.LinearTransform.M32);
			GetLocalExtremePoint(direction, out var extremePoint4);
			direction = new Vector3(inverse.LinearTransform.M13, inverse.LinearTransform.M23, inverse.LinearTransform.M33);
			GetLocalExtremePoint(direction, out var extremePoint5);
			direction = new Vector3(0f - inverse.LinearTransform.M13, 0f - inverse.LinearTransform.M23, 0f - inverse.LinearTransform.M33);
			GetLocalExtremePoint(direction, out var extremePoint6);
			N._7.Transform(ref extremePoint, ref inverse.LinearTransform, out extremePoint);
			N._7.Transform(ref extremePoint2, ref inverse.LinearTransform, out extremePoint2);
			N._7.Transform(ref extremePoint3, ref inverse.LinearTransform, out extremePoint3);
			N._7.Transform(ref extremePoint4, ref inverse.LinearTransform, out extremePoint4);
			N._7.Transform(ref extremePoint5, ref inverse.LinearTransform, out extremePoint5);
			N._7.Transform(ref extremePoint6, ref inverse.LinearTransform, out extremePoint6);
			boundingBox.Max.X = inverse.Translation.X + extremePoint.X;
			boundingBox.Max.Y = inverse.Translation.Y + extremePoint3.Y;
			boundingBox.Max.Z = inverse.Translation.Z + extremePoint5.Z;
			boundingBox.Min.X = inverse.Translation.X + extremePoint2.X;
			boundingBox.Min.Y = inverse.Translation.Y + extremePoint4.Y;
			boundingBox.Min.Z = inverse.Translation.Z + extremePoint6.Z;
		}

		public abstract float ComputeMinimumRadius();

		public abstract float ComputeMaximumRadius();
	}
}
namespace Y
{
	internal interface h
	{
		a Entry { get; }
	}
}
