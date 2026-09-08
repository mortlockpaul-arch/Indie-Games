using Microsoft.Xna.Framework.GamerServices;
using Quasar.GameUtils.XBLIG.Game;
using Quasar.GameUtils.XBLIG.Network;
using Quasar.Global;
using XnaToFna;

namespace AvatarFarmOnline;

internal static class Program
{
	private static void Main(string[] args)
	{
		XnaToFnaHelper.MainHook(args);
		XNAGame xNAGame = new SimpleXNAGame(1280, 720, fullScreen: true, autoSize: true, vSynch: true, multisampling: false, mouseEnabled: false);
		XBLIGPlatformInterface.Init(xNAGame);
		XBLIGNetworkInterface.Init(xNAGame);
		if (args.Length > 0 && args[0] == "trial")
		{
			Guide.SimulateTrialMode = true;
		}
		xNAGame.Run(new AvatarFarmOnline.AvatarFarmOnlineGame());
	}
}
