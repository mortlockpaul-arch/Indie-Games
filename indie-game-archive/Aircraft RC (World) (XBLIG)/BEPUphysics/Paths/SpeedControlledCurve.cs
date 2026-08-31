using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace BEPUphysics.Paths;

/// <summary>
/// Wrapper that controls the speed at which a curve is traversed.
/// </summary>
/// <remarks>
/// <para>
/// Even if a curve is evaluated at linearly increasing positions,
/// the distance between consecutive values can be different.  This
/// has the effect of a curve-following object having variable velocity.
/// </para>
/// <para>
/// To counteract the variable velocity, this wrapper samples the curve
/// and produces a reparameterized, distance-based curve.  Changing the
/// evaluated curve position will linearly change the value.
/// </para>
/// </remarks>
public abstract class SpeedControlledCurve<TValue> : Path<TValue>
{
	private readonly List<Vector2> samples = new List<Vector2>();

	private Curve<TValue> curve;

	private int samplesPerInterval;

	/// <summary>
	/// Gets or sets the curve wrapped by this instance.
	/// </summary>
	public Curve<TValue> Curve
	{
		get
		{
			return curve;
		}
		set
		{
			curve = value;
			if (Curve != null)
			{
				ResampleCurve();
			}
		}
	}

	/// <summary>
	/// Defines how the curve is sampled when the evaluation time exceeds the final control point.
	/// </summary>
	public CurveEndpointBehavior PostLoop { get; set; }

	/// <summary>
	/// Defines how the curve is sampled when the evaluation time exceeds the beginning control point.
	/// </summary>
	public CurveEndpointBehavior PreLoop { get; set; }

	/// <summary>
	/// Gets or sets the number of samples to use per interval in the curve.
	/// </summary>
	public int SamplesPerInterval
	{
		get
		{
			return samplesPerInterval;
		}
		set
		{
			samplesPerInterval = value;
			if (Curve != null)
			{
				ResampleCurve();
			}
		}
	}

	/// <summary>
	/// Constructs a new speed controlled curve.
	/// </summary>
	protected SpeedControlledCurve()
	{
	}

	/// <summary>
	/// Constructs a new speed-controlled curve.
	/// </summary>
	/// <param name="curve">Curve to wrap.</param>
	protected SpeedControlledCurve(Curve<TValue> curve)
	{
		samplesPerInterval = 10;
		this.curve = curve;
	}

	/// <summary>
	/// Constructs a new speed-controlled curve.
	/// </summary>
	/// <param name="curve">Curve to wrap.</param>
	/// <param name="samplesPerInterval">Number of samples to use when constructing the wrapper curve.
	/// More samples increases the accuracy of the speed requirement at the cost of performance.</param>
	protected SpeedControlledCurve(Curve<TValue> curve, int samplesPerInterval)
	{
		this.curve = curve;
		this.samplesPerInterval = samplesPerInterval;
	}

	/// <summary>
	/// Gets the desired speed at a given time.
	/// </summary>
	/// <param name="time">Time to check for speed.</param>
	/// <returns>Speed at the given time.</returns>
	public abstract float GetSpeedAtCurveTime(float time);

	/// <summary>
	/// Gets the time at which the internal curve would be evaluated at the given time.
	/// </summary>
	/// <param name="time">Time to evaluate the speed-controlled curve.</param>
	/// <returns>Time at which the internal curve would be evaluated.</returns>
	public double GetInnerTime(double time)
	{
		if (Curve == null)
		{
			throw new InvalidOperationException("SpeedControlledCurve's internal curve is null; ensure that its curve property is set prior to evaluation.");
		}
		GetPathBoundsInformation(out var startingTime, out var endingTime);
		time = Curve<TValue>.ModifyTime(time, startingTime, endingTime, Curve.PreLoop, Curve.PostLoop);
		int num = 0;
		int num2 = samples.Count;
		if (num2 == 0)
		{
			return 0.0;
		}
		while (num2 - num > 1)
		{
			int num3 = (num + num2) / 2;
			if (time > (double)samples[num3].X)
			{
				num = num3;
				continue;
			}
			if (time < (double)samples[num3].X)
			{
				num2 = num3;
				continue;
			}
			num = num3;
			break;
		}
		if ((double)samples[num].X > time)
		{
			num--;
		}
		double num4 = (time - (double)samples[num].X) / (double)(samples[num + 1].X - samples[num].X);
		return (1.0 - num4) * (double)samples[num].Y + num4 * (double)samples[num + 1].Y;
	}

	/// <summary>
	/// Computes the value of the curve at a given time.
	/// </summary>
	/// <param name="time">Time to evaluate the curve at.</param>
	/// <param name="value">Value of the curve at the given time.</param>
	/// <param name="innerTime">Time at which the internal curve was evaluated to get the value.</param>
	public void Evaluate(double time, out TValue value, out double innerTime)
	{
		Curve.Evaluate(innerTime = GetInnerTime(time), out value);
	}

	/// <summary>
	/// Computes the value of the curve at a given time.
	/// </summary>
	/// <param name="time">Time to evaluate the curve at.</param>
	/// <param name="value">Value of the curve at the given time.</param>
	public override void Evaluate(double time, out TValue value)
	{
		Curve.Evaluate(GetInnerTime(time), out value);
	}

	/// <summary>
	/// Gets the starting and ending times of the path.
	/// </summary>
	/// <param name="startingTime">Beginning time of the path.</param>
	/// <param name="endingTime">Ending time of the path.</param>
	public override void GetPathBoundsInformation(out float startingTime, out float endingTime)
	{
		if (samples.Count > 0)
		{
			startingTime = 0f;
			endingTime = samples[samples.Count - 1].X;
		}
		else
		{
			startingTime = 0f;
			endingTime = 0f;
		}
	}

	/// <summary>
	/// Forces a recalculation of curve samples.
	/// This needs to be called if the wrapped curve
	/// is changed.
	/// </summary>
	public void ResampleCurve()
	{
		samples.Clear();
		curve.GetCurveBoundsInformation(out var _, out var _, out var minIndex, out var maxIndex);
		if (minIndex < 0 || maxIndex < 0)
		{
			return;
		}
		float num = 0f;
		TValue value = Curve.ControlPoints[minIndex].Value;
		TValue start = value;
		float num2 = 1f / (float)(SamplesPerInterval + 1);
		float speedAtCurveTime = GetSpeedAtCurveTime(Curve.ControlPoints[minIndex].Time);
		float num3 = speedAtCurveTime;
		for (int i = minIndex; i < maxIndex; i++)
		{
			start = value;
			value = Curve.ControlPoints[i].Value;
			if (speedAtCurveTime != 0f)
			{
				num += GetDistance(start, value) / speedAtCurveTime;
			}
			num3 = speedAtCurveTime;
			speedAtCurveTime = GetSpeedAtCurveTime(Curve.ControlPoints[i].Time);
			samples.Add(new Vector2(num, Curve.ControlPoints[i].Time));
			float num4 = Curve.ControlPoints[i].Time;
			float num5 = Curve.ControlPoints[i + 1].Time - num4;
			float num6 = num5 / (float)(SamplesPerInterval + 1);
			for (int j = 1; j <= SamplesPerInterval; j++)
			{
				start = value;
				Curve.Evaluate(i, (float)j * num2, out value);
				num4 += num6;
				if (speedAtCurveTime != 0f)
				{
					num += GetDistance(start, value) / speedAtCurveTime;
				}
				num3 = speedAtCurveTime;
				speedAtCurveTime = GetSpeedAtCurveTime(num4);
				samples.Add(new Vector2(num, num4));
			}
		}
		num += GetDistance(start, value) / num3;
		samples.Add(new Vector2(num, Curve.ControlPoints[maxIndex].Time));
	}

	/// <summary>
	/// Computes the distance between the two values.
	/// </summary>
	/// <param name="start">Starting value.</param>
	/// <param name="end">Ending value.</param>
	/// <returns>Distance between the values.</returns>
	protected abstract float GetDistance(TValue start, TValue end);
}
