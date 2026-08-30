using Microsoft.Xna.Framework;

namespace Maximinus;

public class MultiMonitorGraphicsDeviceManager : GraphicsDeviceManager
{
	private int MonitorIndex = 1;

	public MultiMonitorGraphicsDeviceManager(Game game, int monitorIndex)
		: base(game)
	{
		MonitorIndex = monitorIndex;
	}

	protected override void OnPreparingDeviceSettings(object sender, PreparingDeviceSettingsEventArgs args)
	{
		base.OnPreparingDeviceSettings(sender, args);
	}
}
