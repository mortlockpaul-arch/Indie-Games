namespace Viking_x86.vikinggame;

public class VikingQuake
{
	public static float quake;

	public static void SetQuake(float val)
	{
		if (val > quake)
		{
			quake = val;
		}
	}

	public static void Update()
	{
		if (quake > 0f)
		{
			quake -= Game1.frameTime;
			VScroll.scroll += Rand.GetRandomVec2(-1f, 1f, -1f, 1f) * quake * 10f;
		}
	}
}
