using System;

namespace BEPUphysics.Paths;

/// <summary>
/// Manages a curve in 3D space that supports interpolation.
/// </summary>
/// <typeparam name="TValue">Type of values in the curve.</typeparam>
public abstract class Curve<TValue> : Path<TValue>
{
	/// <summary>
	/// Gets the list of control points in the curve.
	/// </summary>
	public CurveControlPointList<TValue> ControlPoints { get; private set; }

	/// <summary>
	/// Defines how the curve is sampled when the evaluation time exceeds the final control point.
	/// </summary>
	public CurveEndpointBehavior PostLoop { get; set; }

	/// <summary>
	/// Defines how the curve is sampled when the evaluation time exceeds the beginning control point.
	/// </summary>
	public CurveEndpointBehavior PreLoop { get; set; }

	/// <summary>
	/// Constructs a new 3D curve.
	/// </summary>
	protected Curve()
	{
		ControlPoints = new CurveControlPointList<TValue>(this);
	}

	/// <summary>
	/// Converts an unbounded time to a time within the curve's interval using the 
	/// endpoint behavior.
	/// </summary>
	/// <param name="time">Time to convert.</param>
	/// <param name="intervalBegin">Beginning of the curve's interval.</param>
	/// <param name="intervalEnd">End of the curve's interval.</param>
	/// <param name="preLoop">Looping behavior of the curve before the first endpoint's time.</param>
	/// <param name="postLoop">Looping behavior of the curve after the last endpoint's time.</param>
	/// <returns>Time within the curve's interval.</returns>
	public static double ModifyTime(double time, float intervalBegin, float intervalEnd, CurveEndpointBehavior preLoop, CurveEndpointBehavior postLoop)
	{
		if (time < (double)intervalBegin)
		{
			switch (preLoop)
			{
			case CurveEndpointBehavior.Wrap:
			{
				double num = time - (double)intervalBegin;
				double num2 = intervalEnd - intervalBegin;
				num %= num2;
				return (double)intervalEnd + num;
			}
			case CurveEndpointBehavior.Clamp:
				return Math.Max(intervalBegin, time);
			case CurveEndpointBehavior.Mirror:
			{
				double num = time - (double)intervalBegin;
				double num2 = intervalEnd - intervalBegin;
				int num3 = (int)(num / num2);
				if (num3 % 2 == 0)
				{
					return (double)intervalBegin - num % num2;
				}
				return (double)intervalEnd + num % num2;
			}
			}
		}
		else if (time >= (double)intervalEnd)
		{
			switch (postLoop)
			{
			case CurveEndpointBehavior.Wrap:
			{
				double num4 = time - (double)intervalEnd;
				double num5 = intervalEnd - intervalBegin;
				num4 %= num5;
				return (double)intervalBegin + num4;
			}
			case CurveEndpointBehavior.Clamp:
				return Math.Min(intervalEnd, time);
			case CurveEndpointBehavior.Mirror:
			{
				double num4 = time - (double)intervalEnd;
				double num5 = intervalEnd - intervalBegin;
				int num6 = (int)(num4 / num5);
				if (num6 % 2 == 0)
				{
					return (double)intervalEnd - num4 % num5;
				}
				return (double)intervalBegin + num4 % num5;
			}
			}
		}
		return time;
	}

	/// <summary>
	/// Evaluates the curve section starting at the control point index using
	/// the weight value.
	/// </summary>
	/// <param name="controlPointIndex">Index of the starting control point of the subinterval.</param>
	/// <param name="weight">Location to evaluate on the subinterval from 0 to 1.</param>
	/// <param name="value">Value at the given location.</param>
	public abstract void Evaluate(int controlPointIndex, float weight, out TValue value);

	/// <summary>
	/// Gets the curve's bounding index information.
	/// </summary>
	/// <param name="minIndex">Index of the minimum control point in the active curve segment.</param>
	/// <param name="maxIndex">Index of the maximum control point in the active curve segment.</param>
	public abstract void GetCurveIndexBoundsInformation(out int minIndex, out int maxIndex);

	/// <summary>
	/// Computes the value of the curve at a given time.
	/// </summary>
	/// <param name="time">Time at which to evaluate the curve.</param>
	/// <param name="value">Curve value at the given time.</param>
	public override void Evaluate(double time, out TValue value)
	{
		GetCurveBoundsInformation(out var firstIndexTime, out var lastIndexTime, out var minIndex, out var maxIndex);
		if (minIndex < 0 || maxIndex < 0)
		{
			value = default(TValue);
			return;
		}
		if (minIndex == maxIndex)
		{
			value = ControlPoints[minIndex].Value;
			return;
		}
		time = ModifyTime(time, firstIndexTime, lastIndexTime, PreLoop, PostLoop);
		int previousIndex = GetPreviousIndex(time);
		if (previousIndex == maxIndex)
		{
			value = ControlPoints[maxIndex].Value;
			return;
		}
		float num = ControlPoints[previousIndex + 1].Time - ControlPoints[previousIndex].Time;
		float weight = ((!(num < 1E-07f)) ? ((float)(time - (double)ControlPoints[previousIndex].Time) / num) : 0f);
		Evaluate(previousIndex, weight, out value);
	}

	/// <summary>
	/// Gets the starting and ending times of the path.
	/// </summary>
	/// <param name="startingTime">Beginning time of the path.</param>
	/// <param name="endingTime">Ending time of the path.</param>
	public override void GetPathBoundsInformation(out float startingTime, out float endingTime)
	{
		GetCurveBoundsInformation(out startingTime, out endingTime, out int minIndex, out minIndex);
	}

	/// <summary>
	/// Gets information about the curve's total active interval.
	/// These are not always the first and last endpoints in a curve.
	/// </summary>
	/// <param name="firstIndexTime">Time of the first index.</param>
	/// <param name="lastIndexTime">Time of the last index.</param>
	/// <param name="minIndex">First index in the reachable curve.</param>
	/// <param name="maxIndex">Last index in the reachable curve.</param>
	public void GetCurveBoundsInformation(out float firstIndexTime, out float lastIndexTime, out int minIndex, out int maxIndex)
	{
		GetCurveIndexBoundsInformation(out minIndex, out maxIndex);
		if (minIndex >= 0 && maxIndex < ControlPoints.Count && minIndex <= maxIndex)
		{
			firstIndexTime = ControlPoints[minIndex].Time;
			lastIndexTime = ControlPoints[maxIndex].Time;
		}
		else
		{
			firstIndexTime = 0f;
			lastIndexTime = 0f;
		}
	}

	/// <summary>
	/// Computes the indices of control points surrounding the time.
	/// If the time is equal to a control point's time, indexA will
	/// be that control point's index.
	/// </summary>
	/// <param name="time">Time to index.</param>
	/// <returns>Index prior to or equal to the given time.</returns>
	public int GetPreviousIndex(double time)
	{
		int num = 0;
		int num2 = ControlPoints.Count;
		if (num2 == 0)
		{
			return -1;
		}
		while (num2 - num > 1)
		{
			int num3 = (num + num2) / 2;
			if (time > (double)ControlPoints[num3].Time)
			{
				num = num3;
				continue;
			}
			if (time < (double)ControlPoints[num3].Time)
			{
				num2 = num3;
				continue;
			}
			num = num3;
			break;
		}
		if ((double)ControlPoints[num].Time <= time)
		{
			return num;
		}
		return num - 1;
	}

	internal void InternalControlPointTimeChanged(CurveControlPoint<TValue> controlPoint)
	{
		int num = ControlPoints.list.IndexOf(controlPoint);
		ControlPoints.list.RemoveAt(num);
		int num2 = GetPreviousIndex(controlPoint.Time) + 1;
		ControlPoints.list.Insert(num2, controlPoint);
		ControlPointTimeChanged(controlPoint, num, num2);
	}

	/// <summary>
	/// Called when a control point is added.
	/// </summary>
	/// <param name="curveControlPoint">New control point.</param>
	/// <param name="index">Index of the control point.</param>
	protected internal abstract void ControlPointAdded(CurveControlPoint<TValue> curveControlPoint, int index);

	/// <summary>
	/// Called when a control point is removed.
	/// </summary>
	/// <param name="curveControlPoint">Removed control point.</param>
	/// <param name="oldIndex">Index of the control point before it was removed.</param>
	protected internal abstract void ControlPointRemoved(CurveControlPoint<TValue> curveControlPoint, int oldIndex);

	/// <summary>
	/// Called when a control point belonging to the curve has its time changed.
	/// </summary>
	/// <param name="curveControlPoint">Changed control point.</param>
	/// <param name="oldIndex">Old index of the control point.</param>
	/// <param name="newIndex">New index of the control point.</param>
	protected internal abstract void ControlPointTimeChanged(CurveControlPoint<TValue> curveControlPoint, int oldIndex, int newIndex);

	/// <summary>
	/// Called when a control point belonging to the curve has its value changed.
	/// </summary>
	/// <param name="curveControlPoint">Changed control point.</param>
	protected internal abstract void ControlPointValueChanged(CurveControlPoint<TValue> curveControlPoint);
}
