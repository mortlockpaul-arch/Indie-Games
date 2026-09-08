using Microsoft.Xna.Framework;

namespace PerformanceMeasuring.GameDebugTools;

public class DebugSystem
{
	private static DebugSystem singletonInstance;

	public static DebugSystem Instance => singletonInstance;

	public DebugManager DebugManager { get; private set; }

	public DebugCommandUI DebugCommandUI { get; private set; }

	public FpsCounter FpsCounter { get; private set; }

	public TimeRuler TimeRuler { get; private set; }

	public static DebugSystem Initialize(Game game, string debugFont)
	{
		if (singletonInstance != null)
		{
			return singletonInstance;
		}
		singletonInstance = new DebugSystem();
		singletonInstance.DebugManager = new DebugManager(game, debugFont);
		game.Components.Add(singletonInstance.DebugManager);
		singletonInstance.DebugCommandUI = new DebugCommandUI(game);
		game.Components.Add(singletonInstance.DebugCommandUI);
		singletonInstance.FpsCounter = new FpsCounter(game);
		game.Components.Add(singletonInstance.FpsCounter);
		singletonInstance.TimeRuler = new TimeRuler(game);
		game.Components.Add(singletonInstance.TimeRuler);
		return singletonInstance;
	}

	private DebugSystem()
	{
	}
}
