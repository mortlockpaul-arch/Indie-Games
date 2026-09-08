using FarseerPhysics.Common;
using Microsoft.Xna.Framework;

namespace FarseerPhysics.Collision;

public struct SeparationFunction
{
	private Vector2 _axis;

	private Vector2 _localPoint;

	private DistanceProxy _proxyA;

	private DistanceProxy _proxyB;

	private Sweep _sweepA;

	private Sweep _sweepB;

	private SeparationFunctionType _type;

	public SeparationFunction(ref SimplexCache cache, ref DistanceProxy proxyA, ref Sweep sweepA, ref DistanceProxy proxyB, ref Sweep sweepB, float t1)
	{
		_localPoint = Vector2.Zero;
		_proxyA = proxyA;
		_proxyB = proxyB;
		int count = cache.Count;
		_sweepA = sweepA;
		_sweepB = sweepB;
		_sweepA.GetTransform(out var xf, t1);
		_sweepB.GetTransform(out var xf2, t1);
		if (count == 1)
		{
			_type = SeparationFunctionType.Points;
			Vector2 vertex = _proxyA.GetVertex(cache.IndexA[0]);
			Vector2 vertex2 = _proxyB.GetVertex(cache.IndexB[0]);
			Vector2 vector = MathUtils.Multiply(ref xf, vertex);
			Vector2 vector2 = MathUtils.Multiply(ref xf2, vertex2);
			_axis = vector2 - vector;
			_axis.Normalize();
		}
		else if (cache.IndexA[0] == cache.IndexA[1])
		{
			_type = SeparationFunctionType.FaceB;
			Vector2 vertex3 = proxyB.GetVertex(cache.IndexB[0]);
			Vector2 vertex4 = proxyB.GetVertex(cache.IndexB[1]);
			Vector2 vector3 = vertex4 - vertex3;
			_axis = new Vector2(vector3.Y, 0f - vector3.X);
			_axis.Normalize();
			Vector2 value = MathUtils.Multiply(ref xf2.R, _axis);
			_localPoint = 0.5f * (vertex3 + vertex4);
			Vector2 vector4 = MathUtils.Multiply(ref xf2, _localPoint);
			Vector2 vertex5 = proxyA.GetVertex(cache.IndexA[0]);
			Vector2 vector5 = MathUtils.Multiply(ref xf, vertex5);
			float num = Vector2.Dot(vector5 - vector4, value);
			if (num < 0f)
			{
				_axis = -_axis;
				num = 0f - num;
			}
		}
		else
		{
			_type = SeparationFunctionType.FaceA;
			Vector2 vertex6 = _proxyA.GetVertex(cache.IndexA[0]);
			Vector2 vertex7 = _proxyA.GetVertex(cache.IndexA[1]);
			Vector2 vector6 = vertex7 - vertex6;
			_axis = new Vector2(vector6.Y, 0f - vector6.X);
			_axis.Normalize();
			Vector2 value2 = MathUtils.Multiply(ref xf.R, _axis);
			_localPoint = 0.5f * (vertex6 + vertex7);
			Vector2 vector7 = MathUtils.Multiply(ref xf, _localPoint);
			Vector2 vertex8 = _proxyB.GetVertex(cache.IndexB[0]);
			Vector2 vector8 = MathUtils.Multiply(ref xf2, vertex8);
			float num2 = Vector2.Dot(vector8 - vector7, value2);
			if (num2 < 0f)
			{
				_axis = -_axis;
				num2 = 0f - num2;
			}
		}
	}

	public float FindMinSeparation(out int indexA, out int indexB, float t)
	{
		_sweepA.GetTransform(out var xf, t);
		_sweepB.GetTransform(out var xf2, t);
		switch (_type)
		{
		case SeparationFunctionType.Points:
		{
			Vector2 direction3 = MathUtils.MultiplyT(ref xf.R, _axis);
			Vector2 direction4 = MathUtils.MultiplyT(ref xf2.R, -_axis);
			indexA = _proxyA.GetSupport(direction3);
			indexB = _proxyB.GetSupport(direction4);
			Vector2 vertex3 = _proxyA.GetVertex(indexA);
			Vector2 vertex4 = _proxyB.GetVertex(indexB);
			Vector2 vector7 = MathUtils.Multiply(ref xf, vertex3);
			Vector2 vector8 = MathUtils.Multiply(ref xf2, vertex4);
			return Vector2.Dot(vector8 - vector7, _axis);
		}
		case SeparationFunctionType.FaceA:
		{
			Vector2 vector4 = MathUtils.Multiply(ref xf.R, _axis);
			Vector2 vector5 = MathUtils.Multiply(ref xf, _localPoint);
			Vector2 direction2 = MathUtils.MultiplyT(ref xf2.R, -vector4);
			indexA = -1;
			indexB = _proxyB.GetSupport(direction2);
			Vector2 vertex2 = _proxyB.GetVertex(indexB);
			Vector2 vector6 = MathUtils.Multiply(ref xf2, vertex2);
			return Vector2.Dot(vector6 - vector5, vector4);
		}
		case SeparationFunctionType.FaceB:
		{
			Vector2 vector = MathUtils.Multiply(ref xf2.R, _axis);
			Vector2 vector2 = MathUtils.Multiply(ref xf2, _localPoint);
			Vector2 direction = MathUtils.MultiplyT(ref xf.R, -vector);
			indexB = -1;
			indexA = _proxyA.GetSupport(direction);
			Vector2 vertex = _proxyA.GetVertex(indexA);
			Vector2 vector3 = MathUtils.Multiply(ref xf, vertex);
			return Vector2.Dot(vector3 - vector2, vector);
		}
		default:
			indexA = -1;
			indexB = -1;
			return 0f;
		}
	}

	public float Evaluate(int indexA, int indexB, float t)
	{
		_sweepA.GetTransform(out var xf, t);
		_sweepB.GetTransform(out var xf2, t);
		switch (_type)
		{
		case SeparationFunctionType.Points:
		{
			MathUtils.MultiplyT(ref xf.R, _axis);
			MathUtils.MultiplyT(ref xf2.R, -_axis);
			Vector2 vertex3 = _proxyA.GetVertex(indexA);
			Vector2 vertex4 = _proxyB.GetVertex(indexB);
			Vector2 vector7 = MathUtils.Multiply(ref xf, vertex3);
			Vector2 vector8 = MathUtils.Multiply(ref xf2, vertex4);
			return Vector2.Dot(vector8 - vector7, _axis);
		}
		case SeparationFunctionType.FaceA:
		{
			Vector2 vector4 = MathUtils.Multiply(ref xf.R, _axis);
			Vector2 vector5 = MathUtils.Multiply(ref xf, _localPoint);
			MathUtils.MultiplyT(ref xf2.R, -vector4);
			Vector2 vertex2 = _proxyB.GetVertex(indexB);
			Vector2 vector6 = MathUtils.Multiply(ref xf2, vertex2);
			return Vector2.Dot(vector6 - vector5, vector4);
		}
		case SeparationFunctionType.FaceB:
		{
			Vector2 vector = MathUtils.Multiply(ref xf2.R, _axis);
			Vector2 vector2 = MathUtils.Multiply(ref xf2, _localPoint);
			MathUtils.MultiplyT(ref xf.R, -vector);
			Vector2 vertex = _proxyA.GetVertex(indexA);
			Vector2 vector3 = MathUtils.Multiply(ref xf, vertex);
			return Vector2.Dot(vector3 - vector2, vector);
		}
		default:
			return 0f;
		}
	}
}
