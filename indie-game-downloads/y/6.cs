using System;
using Microsoft.Xna.Framework;
using N;
using r;
using s;

namespace Y
{
	internal interface _6
	{
		BoundingBox BoundingBox { get; }
	}
}
namespace y
{
	internal class _6 : h
	{
		internal float a5h;

		internal float a5b;

		internal float a56;

		public float HalfWidth
		{
			get
			{
				return a5h;
			}
			set
			{
				a5h = value;
				OnShapeChanged();
			}
		}

		public float HalfHeight
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

		public float HalfLength
		{
			get
			{
				return a56;
			}
			set
			{
				a56 = value;
				OnShapeChanged();
			}
		}

		public float Width
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

		public float Height
		{
			get
			{
				return a5b * 2f;
			}
			set
			{
				a5b = value / 2f;
				OnShapeChanged();
			}
		}

		public float Length
		{
			get
			{
				return a56 * 2f;
			}
			set
			{
				a56 = value / 2f;
				OnShapeChanged();
			}
		}

		public _6(float width, float height, float length)
		{
			a5h = width * 0.5f;
			a5b = height * 0.5f;
			a56 = length * 0.5f;
			OnShapeChanged();
		}

		public override void GetBoundingBox(ref N._0006 shapeTransform, out BoundingBox boundingBox)
		{
			boundingBox = default(BoundingBox);
			N._7.CreateFromQuaternion(ref shapeTransform.Orientation, out var result);
			Vector3 result2 = new Vector3((float)Math.Sign(result.M11) * a5h, (float)Math.Sign(result.M21) * a5b, (float)Math.Sign(result.M31) * a56);
			Vector3 result3 = new Vector3((float)Math.Sign(result.M12) * a5h, (float)Math.Sign(result.M22) * a5b, (float)Math.Sign(result.M32) * a56);
			Vector3 result4 = new Vector3((float)Math.Sign(result.M13) * a5h, (float)Math.Sign(result.M23) * a5b, (float)Math.Sign(result.M33) * a56);
			N._7.Transform(ref result2, ref result, out result2);
			N._7.Transform(ref result3, ref result, out result3);
			N._7.Transform(ref result4, ref result, out result4);
			boundingBox.Max.X = shapeTransform.Position.X + result2.X;
			boundingBox.Max.Y = shapeTransform.Position.Y + result3.Y;
			boundingBox.Max.Z = shapeTransform.Position.Z + result4.Z;
			boundingBox.Min.X = shapeTransform.Position.X - result2.X;
			boundingBox.Min.Y = shapeTransform.Position.Y - result3.Y;
			boundingBox.Min.Z = shapeTransform.Position.Z - result4.Z;
		}

		public override void GetLocalExtremePointWithoutMargin(ref Vector3 direction, out Vector3 extremePoint)
		{
			extremePoint = new Vector3((float)Math.Sign(direction.X) * (a5h - collisionMargin), (float)Math.Sign(direction.Y) * (a5b - collisionMargin), (float)Math.Sign(direction.Z) * (a56 - collisionMargin));
		}

		public override float ComputeMinimumRadius()
		{
			return Math.Min(a5h, Math.Min(a5b, a56));
		}

		public override float ComputeMaximumRadius()
		{
			return (float)Math.Sqrt(a5h * a5h + a5b * a5b + a56 * a56);
		}

		public override N._7 ComputeVolumeDistribution(out float volume)
		{
			N._7 result = default(N._7);
			float num = a5h * a5h;
			float num2 = a5b * a5b;
			float num3 = a56 * a56;
			result.M11 = (num2 + num3) * (1f / 3f);
			result.M22 = (num + num3) * (1f / 3f);
			result.M33 = (num + num2) * (1f / 3f);
			volume = ComputeVolume();
			return result;
		}

		public override bool RayTest(ref Ray ray, ref N._0006 transform, float maximumLength, out r._0006 hit)
		{
			hit = default(r._0006);
			Quaternion.Conjugate(ref transform.Orientation, out var result);
			Vector3.Subtract(ref ray.Position, ref transform.Position, out var result2);
			Vector3.Transform(ref result2, ref result, out result2);
			Vector3.Transform(ref ray.Direction, ref result, out var result3);
			Vector3 value = r.X.ZeroVector;
			float num = 0f;
			float val = maximumLength;
			if (Math.Abs(result3.X) < 1E-07f && (result2.X < 0f - a5h || result2.X > a5h))
			{
				return false;
			}
			float num2 = 1f / result3.X;
			float num3 = (0f - a5h - result2.X) * num2;
			float num4 = (a5h - result2.X) * num2;
			Vector3 vector = new Vector3(-1f, 0f, 0f);
			float num5;
			if (num3 > num4)
			{
				num5 = num3;
				num3 = num4;
				num4 = num5;
				vector *= -1f;
			}
			num5 = num;
			num = Math.Max(num, num3);
			if (num5 != num)
			{
				value = vector;
			}
			val = Math.Min(val, num4);
			if (num > val)
			{
				return false;
			}
			if (Math.Abs(result3.Y) < 1E-07f && (result2.Y < 0f - a5b || result2.Y > a5b))
			{
				return false;
			}
			num2 = 1f / result3.Y;
			num3 = (0f - a5b - result2.Y) * num2;
			num4 = (a5b - result2.Y) * num2;
			vector = new Vector3(0f, -1f, 0f);
			if (num3 > num4)
			{
				num5 = num3;
				num3 = num4;
				num4 = num5;
				vector *= -1f;
			}
			num5 = num;
			num = Math.Max(num, num3);
			if (num5 != num)
			{
				value = vector;
			}
			val = Math.Min(val, num4);
			if (num > val)
			{
				return false;
			}
			if (Math.Abs(result3.Z) < 1E-07f && (result2.Z < 0f - a56 || result2.Z > a56))
			{
				return false;
			}
			num2 = 1f / result3.Z;
			num3 = (0f - a56 - result2.Z) * num2;
			num4 = (a56 - result2.Z) * num2;
			vector = new Vector3(0f, 0f, -1f);
			if (num3 > num4)
			{
				num5 = num3;
				num3 = num4;
				num4 = num5;
				vector *= -1f;
			}
			num5 = num;
			num = Math.Max(num, num3);
			if (num5 != num)
			{
				value = vector;
			}
			val = Math.Min(val, num4);
			if (num > val)
			{
				return false;
			}
			hit.T = num;
			Vector3.Multiply(ref ray.Direction, num, out hit.Location);
			Vector3.Add(ref hit.Location, ref ray.Position, out hit.Location);
			Vector3.Transform(ref value, ref transform.Orientation, out value);
			hit.Normal = value;
			return true;
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
			return 8f * a5h * a56 * a5b;
		}

		public override s.b GetCollidableInstance()
		{
			return new s.B<_6>(this);
		}
	}
}
