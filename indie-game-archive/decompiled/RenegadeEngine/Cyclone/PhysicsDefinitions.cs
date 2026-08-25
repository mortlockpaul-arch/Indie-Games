namespace RenegadeEngine.Cyclone;

public static class PhysicsDefinitions
{
	private static float sleepEpsilon = 0.3f;

	public static float SleepEpsilon
	{
		get
		{
			return sleepEpsilon;
		}
		set
		{
			sleepEpsilon = value;
		}
	}
}
