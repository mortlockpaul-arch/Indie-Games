using Microsoft.Xna.Framework;

namespace EGEngine;

public class ProfilerComponent : GameComponent
{
	private double totalTime;

	public ProfilerComponent(Game game)
		: base(game)
	{
	}

	public override void Update(GameTime gameTime)
	{
		totalTime += gameTime.ElapsedGameTime.TotalSeconds;
		if (!(totalTime >= 5.0))
		{
			return;
		}
		foreach (Profiler allProfiler in Profiler.AllProfilers)
		{
			allProfiler.Print(totalTime);
		}
		totalTime = 0.0;
	}
}
