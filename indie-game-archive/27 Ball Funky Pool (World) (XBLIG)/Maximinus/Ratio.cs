using Microsoft.Xna.Framework;

namespace Maximinus;

public class Ratio
{
	public static float TimerRatio(GameTime currentGameTime, double startTimeSecs, double waitTimeSecs)
	{
		return (float)Utils.clampRatio((currentGameTime.TotalGameTime.TotalSeconds - startTimeSecs) / waitTimeSecs);
	}
}
