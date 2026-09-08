namespace System.Diagnostics.Metrics;

internal sealed class Base2ExponentialHistogramStatistics : IAggregationStatistics
{
	public int Scale { get; }

	public long ZeroCount { get; }

	public double Sum { get; }

	public long Count { get; }

	public double Minimum { get; }

	public double Maximum { get; }

	public long[] PositiveBuckets { get; }

	internal Base2ExponentialHistogramStatistics(int scale, long zeroCount, double sum, long count, double min, double max, long[] buckets)
	{
		Scale = scale;
		ZeroCount = zeroCount;
		Sum = sum;
		Count = count;
		Minimum = min;
		Maximum = max;
		PositiveBuckets = buckets;
	}
}
