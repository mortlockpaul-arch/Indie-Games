using System;

namespace GKEngine.Utils;

public class GUID : IEquatable<GUID>
{
	public static char DELIMITER = ':';

	public static int RADIX = 16;

	private static int counter = 0;

	public long time;

	public int random;

	public int count;

	public string value;

	public GUID()
	{
		Set(DateTime.Now.Ticks, GameEngine.random.Next(int.MaxValue), counter++);
	}

	public void Set(long xTime, int xRandom, int xCount)
	{
		time = xTime;
		random = xRandom;
		count = xCount;
		value = Convert.ToString(time, RADIX).ToUpper() + DELIMITER + Convert.ToString(random, RADIX).ToUpper() + DELIMITER + Convert.ToString(count, RADIX).ToUpper();
	}

	public GUID Copy()
	{
		GUID gUID = new GUID();
		gUID.Set(time, random, count);
		return gUID;
	}

	public override string ToString()
	{
		return "GUID: " + value;
	}

	public bool Equals(GUID other)
	{
		return value == other.value;
	}

	public static GUID FromString(string xString)
	{
		GUID gUID = new GUID();
		if (xString != null)
		{
			string[] array = xString.Split(DELIMITER);
			if (array.Length == 3)
			{
				gUID.Set(Convert.ToInt64(array[0], RADIX), Convert.ToInt32(array[1], RADIX), Convert.ToInt32(array[2], RADIX));
			}
		}
		return gUID;
	}
}
