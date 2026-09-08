using System;

namespace Microsoft.Xna.Framework;

[Serializable]
public class CurveKey : IEquatable<CurveKey>, IComparable<CurveKey>
{
	public CurveContinuity Continuity { get; set; }

	public float Position { get; private set; }

	public float TangentIn { get; set; }

	public float TangentOut { get; set; }

	public float Value { get; set; }

	public CurveKey(float position, float value)
		: this(position, value, 0f, 0f, CurveContinuity.Smooth)
	{
	}

	public CurveKey(float position, float value, float tangentIn, float tangentOut)
		: this(position, value, tangentIn, tangentOut, CurveContinuity.Smooth)
	{
	}

	public CurveKey(float position, float value, float tangentIn, float tangentOut, CurveContinuity continuity)
	{
		Position = position;
		Value = value;
		TangentIn = tangentIn;
		TangentOut = tangentOut;
		Continuity = continuity;
	}

	public CurveKey Clone()
	{
		return new CurveKey(Position, Value, TangentIn, TangentOut, Continuity);
	}

	public int CompareTo(CurveKey other)
	{
		return Position.CompareTo(other.Position);
	}

	public bool Equals(CurveKey other)
	{
		return (object)other != null && other.Position == Position && other.Value == Value && other.TangentIn == TangentIn && other.TangentOut == TangentOut && other.Continuity == Continuity;
	}

	public static bool operator !=(CurveKey a, CurveKey b)
	{
		return !(a == b);
	}

	public static bool operator ==(CurveKey a, CurveKey b)
	{
		return a?.Equals(b) ?? ((object)b == null);
	}

	public override bool Equals(object obj)
	{
		return Equals(obj as CurveKey);
	}

	public override int GetHashCode()
	{
		return Position.GetHashCode() + Value.GetHashCode() + TangentIn.GetHashCode() + TangentOut.GetHashCode() + Continuity.GetHashCode();
	}
}
