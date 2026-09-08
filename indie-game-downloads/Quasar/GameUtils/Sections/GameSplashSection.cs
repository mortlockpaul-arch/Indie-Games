using Quasar.GameUtils.Game;
using Quasar.Global;
using Quasar.Sections;
using Quasar.Textures;

namespace Quasar.GameUtils.Sections;

public class GameSplashSection : SplashScreenSection
{
	public const string GAME_TEXTURE = "GUI/GameSplash";

	public GameSplashSection()
		: base(3, TextureManager.Textures["GUI/GameSplash"])
	{
	}

	public override void MainLoop()
	{
		base.MainLoop();
	}

	protected override void GoToNextSection()
	{
		((BaseGame)Engine.Game).GoToMainScreen();
	}

	public override void Dispose()
	{
		TextureManager.UnloadTexture("Textures/GUI/GameSplash");
		base.Dispose();
	}
}
