using System.Numerics;
using System.Runtime.CompilerServices;

namespace System.Diagnostics.Metrics;

internal sealed class Base2ExponentialHistogramAggregator : Aggregator
{
	private int _scale;

	private double _maxMeasurement;

	private double _minMeasurement;

	private double _sum;

	private long _count;

	private bool _reportDeltas;

	private double _scalingFactor;

	internal CircularBufferBuckets PositiveBuckets { get; }

	internal int Scale
	{
		get
		{
			return _scale;
		}
		set
		{
			_scale = value;
			_scalingFactor = BitConverter.Int64BitsToDouble(0x71547652B82FEL | (1023L + (long)value << 52));
		}
	}

	internal long ZeroCount { get; private set; }

	public Base2ExponentialHistogramAggregator(int maxBuckets = 160, int scale = 20, bool reportDeltas = false)
	{
		if (scale < -11 || scale > 20)
		{
			throw new ArgumentOutOfRangeException("scale", System.SR.Format(System.SR.InvalidHistogramScale, scale, -11, 20));
		}
		if (maxBuckets < 2)
		{
			throw new ArgumentOutOfRangeException("maxBuckets", System.SR.Format(System.SR.InvalidHistogramMaxBuckets, maxBuckets, 2));
		}
		Scale = scale;
		_reportDeltas = reportDeltas;
		_minMeasurement = double.MaxValue;
		_maxMeasurement = double.MinValue;
		PositiveBuckets = new CircularBufferBuckets(maxBuckets);
	}

	public override IAggregationStatistics Collect()
	{
		lock (this)
		{
			Base2ExponentialHistogramStatistics result = new Base2ExponentialHistogramStatistics(Scale, ZeroCount, _sum, _count, _minMeasurement, _maxMeasurement, PositiveBuckets.ToArray());
			if (_reportDeltas)
			{
				_sum = 0.0;
				_minMeasurement = double.MaxValue;
				_maxMeasurement = double.MinValue;
				ZeroCount = 0L;
				_count = 0L;
				PositiveBuckets.Clear();
			}
			return result;
		}
	}

	public override void Update(double measurement)
	{
		if (!IsFinite(measurement))
		{
			return;
		}
		int num = measurement.CompareTo(0.0);
		if (num < 0)
		{
			return;
		}
		lock (this)
		{
			_maxMeasurement = Math.Max(_maxMeasurement, measurement);
			_minMeasurement = Math.Min(_minMeasurement, measurement);
			_count++;
			if (num == 0)
			{
				ZeroCount++;
				return;
			}
			_sum += measurement;
			int num2 = MapToIndex(measurement);
			int num3 = PositiveBuckets.TryIncrement(num2, 1L);
			if (num3 != 0)
			{
				PositiveBuckets.ScaleDown(num3);
				Scale -= num3;
				if (Scale < -11)
				{
					Scale = -11;
				}
				else if (Scale > 20)
				{
					Scale = 20;
				}
				num3 = PositiveBuckets.TryIncrement(num2 >> num3, 1L);
			}
		}
	}

	public int MapToIndex(double value)
	{
		long num = BitConverter.DoubleToInt64Bits(value);
		long num2 = num & 0xFFFFFFFFFFFFFL;
		if (Scale > 0)
		{
			if (num2 == 0L)
			{
				return ((int)((num & 0x7FF0000000000000L) >> 52) - 1023 << Scale) - 1;
			}
			return (int)Math.Ceiling(Math.Log(value) * _scalingFactor) - 1;
		}
		int num3 = (int)((num & 0x7FF0000000000000L) >> 52);
		if (num3 == 0)
		{
			num3 -= LeadingZero64(num2 - 1) - 12;
		}
		else if (num2 == 0L)
		{
			num3--;
		}
		return num3 - 1023 >> -Scale;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static bool IsFinite(double value)
	{
		return double.IsFinite(value);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int LeadingZero64(long value)
	{
		return BitOperations.LeadingZeroCount((ulong)value);
	}
}
