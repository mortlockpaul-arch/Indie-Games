namespace EGEngine;

public class RollingAverage
{
	private float[] sampleValues;

	private int sampleCount;

	private float valueSum;

	private int currentPosition;

	public float AverageValue
	{
		get
		{
			if (sampleCount == 0)
			{
				return 0f;
			}
			return valueSum / (float)sampleCount;
		}
	}

	public RollingAverage(int sampleCount)
	{
		sampleValues = new float[sampleCount];
	}

	public void AddValue(float newValue)
	{
		valueSum -= sampleValues[currentPosition];
		valueSum += newValue;
		sampleValues[currentPosition] = newValue;
		currentPosition++;
		if (currentPosition > sampleCount)
		{
			sampleCount = currentPosition;
		}
		if (currentPosition >= sampleValues.Length)
		{
			currentPosition = 0;
			valueSum = 0f;
			float[] array = sampleValues;
			foreach (float num in array)
			{
				valueSum += num;
			}
		}
	}
}
