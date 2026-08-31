using System.Collections;
using System.Collections.Generic;

namespace BEPUphysics.Paths;

/// <summary>
/// Collection of control points in a curve.
/// </summary>
/// <typeparam name="TValue">Type of values in the curve.</typeparam>
public class CurveControlPointList<TValue> : IEnumerable<CurveControlPoint<TValue>>, IEnumerable
{
	internal List<CurveControlPoint<TValue>> list = new List<CurveControlPoint<TValue>>();

	/// <summary>
	/// Gets the control point at the given index.
	/// </summary>
	/// <param name="index">Index into the list.</param>
	/// <returns>Control point at the index.</returns>
	public CurveControlPoint<TValue> this[int index] => list[index];

	/// <summary>
	/// Gets the number of elements in the list.
	/// </summary>
	public int Count => list.Count;

	/// <summary>
	/// Gets the curve associated with this control point list.
	/// </summary>
	public Curve<TValue> Curve { get; private set; }

	internal CurveControlPointList(Curve<TValue> curve)
	{
		Curve = curve;
	}

	/// <summary>
	///  Gets an enumerator for the list.
	/// </summary>
	/// <returns>Enumerator for the list.</returns>
	public List<CurveControlPoint<TValue>>.Enumerator GetEnumerator()
	{
		return list.GetEnumerator();
	}

	IEnumerator<CurveControlPoint<TValue>> IEnumerable<CurveControlPoint<TValue>>.GetEnumerator()
	{
		return list.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return list.GetEnumerator();
	}

	/// <summary>
	/// Adds a control point to the curve.
	/// </summary>
	/// <param name="point">New control point to add to the curve.</param>
	public void Add(CurveControlPoint<TValue> point)
	{
		int index = Curve.GetPreviousIndex(point.Time) + 1;
		list.Insert(index, point);
		Curve.ControlPointAdded(point, index);
	}

	/// <summary>
	/// Adds a new control point to the curve.
	/// </summary>
	/// <param name="time">Time of the new control point.</param>
	/// <param name="value">Value of the new control point.</param>
	/// <returns>Newly created control point.</returns>
	public CurveControlPoint<TValue> Add(float time, TValue value)
	{
		CurveControlPoint<TValue> curveControlPoint = new CurveControlPoint<TValue>(time, value, Curve);
		Add(curveControlPoint);
		return curveControlPoint;
	}

	/// <summary>
	/// Removes the control point from the curve.
	/// </summary>
	/// <param name="controlPoint">Control point to remove.</param>
	public void Remove(CurveControlPoint<TValue> controlPoint)
	{
		RemoveAt(list.IndexOf(controlPoint));
	}

	/// <summary>
	/// Removes the control point from the curve.
	/// </summary>
	/// <param name="index">Index to remove at.</param>
	public void RemoveAt(int index)
	{
		CurveControlPoint<TValue> curveControlPoint = list[index];
		list.RemoveAt(index);
		Curve.ControlPointRemoved(curveControlPoint, index);
	}
}
