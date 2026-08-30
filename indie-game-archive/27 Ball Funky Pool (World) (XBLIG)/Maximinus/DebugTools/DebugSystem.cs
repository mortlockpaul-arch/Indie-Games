using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Maximinus.DebugTools;

public class DebugSystem
{
	private static DebugSystem singletonInstance;

	public static DebugSystem Instance => singletonInstance;

	public DebugManager DebugManager { get; private set; }

	public DebugCommandUI DebugCommandUI { get; private set; }

	public FpsCounter FpsCounter { get; private set; }

	public TimeRuler TimeRuler { get; private set; }

	public RemoteDebugCommand RemoteDebugCommand { get; private set; }

	public static DebugSystem Initialize(Game game, SpriteFont font)
	{
		return Initialize(game, font, null);
	}

	public static DebugSystem Initialize(Game game, SpriteFont font, SpriteBatch SB)
	{
		if (singletonInstance != null)
		{
			return singletonInstance;
		}
		singletonInstance = new DebugSystem();
		singletonInstance.DebugManager = new DebugManager(game, font, SB);
		game.Components.Add(singletonInstance.DebugManager);
		singletonInstance.DebugCommandUI = new DebugCommandUI(game);
		game.Components.Add(singletonInstance.DebugCommandUI);
		singletonInstance.FpsCounter = new FpsCounter(game);
		game.Components.Add(singletonInstance.FpsCounter);
		singletonInstance.TimeRuler = new TimeRuler(game);
		game.Components.Add(singletonInstance.TimeRuler);
		singletonInstance.RemoteDebugCommand = new RemoteDebugCommand(game);
		game.Components.Add(singletonInstance.RemoteDebugCommand);
		return singletonInstance;
	}

	private DebugSystem()
	{
	}
}
