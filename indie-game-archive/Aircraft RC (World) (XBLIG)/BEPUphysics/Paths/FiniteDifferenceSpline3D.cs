using Microsoft.Xna.Framework;

namespace BEPUphysics.Paths;

/// <summary>
/// 3D hermite curve that uses the finite difference method to compute tangents.
/// </summary>
public class FiniteDifferenceSpline3D : HermiteCurve3D
{
	/// <summary>
	/// Gets the curve's bounding index information.
	/// </summary>
	/// <param name="minIndex">Index of the minimum control point in the active curve segment.</param>
	/// <param name="maxIndex">Index of the maximum control point in the active curve segment.</param>
	public override void GetCurveIndexBoundsInformation(out int minIndex, out int maxIndex)
	{
		if (base.ControlPoints.Count > 0)
		{
			minIndex = 0;
			maxIndex = base.ControlPoints.Count - 1;
		}
		else
		{
			minIndex = -1;
			maxIndex = -1;
		}
	}

	protected override void ComputeTangents()
	{
		if (base.ControlPoints.Count == 1)
		{
			tangents.Add(Vector3.Zero);
			return;
		}
		if (base.ControlPoints.Count == 2)
		{
			Vector3 item = base.ControlPoints[1].Value - base.ControlPoints[0].Value;
			tangents.Add(item);
			tangents.Add(item);
			return;
		}
		Vector3 value = base.ControlPoints[0].Value;
		Vector3 value2 = base.ControlPoints[1].Value;
		Vector3.Subtract(ref value2, ref value, out var result);
		Vector3.Multiply(ref result, 0.5f / (base.ControlPoints[1].Time - base.ControlPoints[0].Time), out result);
		tangents.Add(result);
		Vector3 result2;
		Vector3 value3;
		for (int i = 1; i < base.ControlPoints.Count - 1; i++)
		{
			value3 = value;
			value = value2;
			value2 = base.ControlPoints[i + 1].Value;
			Vector3.Subtract(ref value2, ref value, out result);
			Vector3.Subtract(ref value, ref value3, out result2);
			Vector3.Multiply(ref result, 0.5f / (base.ControlPoints[i + 1].Time - base.ControlPoints[i].Time), out result);
			Vector3.Multiply(ref result2, 0.5f / (base.ControlPoints[i].Time - base.ControlPoints[i - 1].Time), out result2);
			Vector3.Add(ref result, ref result2, out result);
			tangents.Add(result);
		}
		value3 = value;
		value = value2;
		Vector3.Negate(ref value, out result);
		Vector3.Subtract(ref value, ref value3, out result2);
		int num = base.ControlPoints.Count - 1;
		int index = num - 1;
		Vector3.Multiply(ref result2, 0.5f / (base.ControlPoints[num].Time - base.ControlPoints[index].Time), out result2);
		tangents.Add(result2);
	}
}
