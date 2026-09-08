using Quasar.GameUtils.Game;
using Quasar.Global;
using Quasar.Sections;
using Quasar.Textures;

namespace Quasar.GameUtils.Sections;

public class MilkstoneSplashSection : SplashScreenSection
{
	public const string MILKSTONE_TEXTURE = "GUI/Milkstone";

	public MilkstoneSplashSection()
		: base(2, TextureManager.Textures["GUI/Milkstone"])
	{
	}

	public override void MainLoop()
	{
		base.MainLoop();
	}

	protected override void GoToNextSection()
	{
		((BaseGame)Engine.Game).NextGameSectionId = 3;
	}

	public override void Dispose()
	{
		TextureManager.UnloadTexture("Textures/GUI/Milkstone");
		base.Dispose();
	}
}
