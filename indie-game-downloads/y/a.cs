using System;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using N;
using l;
using p;
using q;
using r;
using y;

namespace Y
{
	internal abstract class a : q.h, _6
	{
		internal int a5h;

		private Action a5b;

		protected internal BoundingBox boundingBox;

		internal q._7 a56;

		[CompilerGenerated]
		private object a5a;

		public BoundingBox BoundingBox
		{
			get
			{
				return boundingBox;
			}
			set
			{
				boundingBox = value;
			}
		}

		protected internal abstract bool IsActive { get; }

		public q._7 CollisionRules
		{
			get
			{
				return a56;
			}
			set
			{
				if (a56 != value)
				{
					if (a56 != null)
					{
						a56.CollisionRulesChanged -= a5b;
					}
					a56 = value;
					if (a56 != null)
					{
						a56.CollisionRulesChanged += a5b;
					}
					CollisionRulesUpdated();
				}
			}
		}

		public object Tag
		{
			[CompilerGenerated]
			get
			{
				return a5a;
			}
			[CompilerGenerated]
			set
			{
				a5a = value;
			}
		}

		protected a()
		{
			CollisionRules = new q._7();
			a5b = CollisionRulesUpdated;
			a5h = (int)(base.GetHashCode() * 3625334849u);
		}

		public override int GetHashCode()
		{
			return a5h;
		}

		protected abstract void CollisionRulesUpdated();

		public abstract bool RayCast(Ray ray, float maximumLength, out r._0006 rayHit);

		public virtual bool RayCast(Ray ray, float maximumLength, Func<a, bool> filter, out r._0006 rayHit)
		{
			if (filter(this))
			{
				return RayCast(ray, maximumLength, out rayHit);
			}
			rayHit = default(r._0006);
			return false;
		}

		public abstract bool ConvexCast(y.h castShape, ref N._0006 startingTransform, ref Vector3 sweep, out r._0006 hit);

		public abstract void UpdateBoundingBox();
	}
}
namespace y
{
	internal class a
	{
		public static float InertiaTensorScale = 2.5f;

		public static int NumberOfSamplesPerDimension = 10;

		public static Vector3 ComputeCenter(h shape)
		{
			float volume;
			return ComputeCenter(shape, out volume);
		}

		public static Vector3 ComputeCenter(h shape, out float volume)
		{
			l._7<Vector3> vectorList = p._6.GetVectorList();
			GetPoints(shape, out volume, vectorList);
			Vector3 result = AveragePoints(vectorList);
			p._6.GiveBack(vectorList);
			return result;
		}

		public static Vector3 AveragePoints(l._7<Vector3> pointContributions)
		{
			Vector3 vector = default(Vector3);
			for (int i = 0; i < pointContributions.Count; i++)
			{
				vector += pointContributions[i];
			}
			return vector / pointContributions.Count;
		}

		public static N._7 ComputeVolumeDistribution(h shape, out float volume)
		{
			l._7<Vector3> vectorList = p._6.GetVectorList();
			GetPoints(shape, out volume, vectorList);
			Vector3 center = AveragePoints(vectorList);
			N._7 result = ComputeVolumeDistribution(vectorList, ref center);
			p._6.GiveBack(vectorList);
			return result;
		}

		public static N._7 ComputeVolumeDistribution(h shape, ref Vector3 center, out float volume)
		{
			l._7<Vector3> vectorList = p._6.GetVectorList();
			GetPoints(shape, out volume, vectorList);
			N._7 result = ComputeVolumeDistribution(vectorList, ref center);
			p._6.GiveBack(vectorList);
			return result;
		}

		public static N._7 ComputeVolumeDistribution(l._7<Vector3> pointContributions, ref Vector3 center)
		{
			N._7 result = default(N._7);
			float pointWeight = 1f / (float)pointContributions.Count;
			for (int i = 0; i < pointContributions.Count; i++)
			{
				GetPointContribution(pointWeight, ref center, pointContributions[i], out var contribution);
				N._7.Add(ref result, ref contribution, out result);
			}
			return result;
		}

		public static void GetPoints(h shape, out float volume, l._7<Vector3> outputPointContributions)
		{
			N._0006 shapeTransform = N._0006.Identity;
			shape.GetBoundingBox(ref shapeTransform, out var boundingBox);
			float num = boundingBox.Max.X - boundingBox.Min.X;
			float num2 = boundingBox.Max.Y - boundingBox.Min.Y;
			float num3 = boundingBox.Max.Z - boundingBox.Min.Z;
			float num4 = num2 * num3;
			float num5 = num * num3;
			float num6 = num * num2;
			float num7 = 1f / (float)NumberOfSamplesPerDimension;
			Ray ray = default(Ray);
			Vector3 value;
			Vector3 value2;
			float num8;
			float num9;
			if (num4 > num5 && num4 > num6)
			{
				ray.Direction = Vector3.Right;
				ray.Position = new Vector3(boundingBox.Min.X, boundingBox.Min.Y + 0.5f * num7 * num2, boundingBox.Min.Z + 0.5f * num7 * num3);
				value = new Vector3(0f, num7 * num2, 0f);
				value2 = new Vector3(0f, 0f, num7 * num3);
				num8 = num7 * num;
				num9 = num;
			}
			else if (num5 > num6)
			{
				ray.Direction = Vector3.Up;
				ray.Position = new Vector3(boundingBox.Min.X + 0.5f * num7 * num, boundingBox.Min.Y, boundingBox.Min.Z + 0.5f * num7 * num3);
				value = new Vector3(num7 * num, 0f, 0f);
				value2 = new Vector3(0f, 0f, num7 * num2);
				num8 = num7 * num2;
				num9 = num2;
			}
			else
			{
				ray.Direction = Vector3.Backward;
				ray.Position = new Vector3(boundingBox.Min.X + 0.5f * num7 * num, boundingBox.Min.Y + 0.5f * num7 * num2, boundingBox.Min.Z);
				value = new Vector3(num7 * num, 0f, 0f);
				value2 = new Vector3(0f, num7 * num2, 0f);
				num8 = num7 * num3;
				num9 = num3;
			}
			volume = 0f;
			Ray ray2 = default(Ray);
			for (int i = 0; i < NumberOfSamplesPerDimension; i++)
			{
				for (int j = 0; j < NumberOfSamplesPerDimension; j++)
				{
					if (shape.RayTest(ref ray, ref shapeTransform, num9, out var hit))
					{
						Vector3.Multiply(ref ray.Direction, num9, out ray2.Position);
						Vector3.Add(ref ray2.Position, ref ray.Position, out ray2.Position);
						Vector3.Negate(ref ray.Direction, out ray2.Direction);
						if (shape.RayTest(ref ray2, ref shapeTransform, num9, out var hit2))
						{
							b2(num8, num9, ref value, ref value2, ref ray, ref hit, ref hit2, outputPointContributions, out var num10);
							volume += num10;
						}
					}
					Vector3.Add(ref ray.Position, ref value2, out ray.Position);
				}
				Vector3.Add(ref ray.Position, ref value, out ray.Position);
				Vector3.Multiply(ref value2, NumberOfSamplesPerDimension, out var result);
				Vector3.Subtract(ref ray.Position, ref result, out ray.Position);
			}
		}

		private static void b2(float P_0, float P_1, ref Vector3 P_2, ref Vector3 P_3, ref Ray P_4, ref r._0006 P_5, ref r._0006 P_6, l._7<Vector3> P_7, out float P_8)
		{
			Vector3.Multiply(ref P_4.Direction, P_0, out var result);
			Vector3.Add(ref P_2, ref result, out result);
			Vector3.Add(ref P_3, ref result, out result);
			float num = result.X * result.Y * result.Z;
			P_8 = 0f;
			for (int i = (int)(P_5.T / P_0); i <= (int)((P_1 - P_6.T) / P_0); i++)
			{
				Vector3.Multiply(ref P_4.Direction, ((float)i + 0.5f) * P_0, out var result2);
				Vector3.Add(ref result2, ref P_4.Position, out result2);
				P_7.Add(result2);
				P_8 += num;
			}
		}

		public static void GetPointContribution(float pointWeight, ref Vector3 center, Vector3 p, out N._7 contribution)
		{
			Vector3.Subtract(ref p, ref center, out p);
			float num = pointWeight * p.X * p.X;
			float num2 = pointWeight * p.Y * p.Y;
			float num3 = pointWeight * p.Z * p.Z;
			contribution.M11 = num2 + num3;
			contribution.M22 = num + num3;
			contribution.M33 = num + num2;
			contribution.M12 = (0f - pointWeight) * p.X * p.Y;
			contribution.M13 = (0f - pointWeight) * p.X * p.Z;
			contribution.M23 = (0f - pointWeight) * p.Y * p.Z;
			contribution.M21 = contribution.M12;
			contribution.M31 = contribution.M13;
			contribution.M32 = contribution.M23;
		}
	}
}
