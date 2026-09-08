using AvatarFarmOnline.Logic.Mode.Farm;
using AvatarFarmOnline.Template;
using Quasar.GUI;
using Quasar.GameUtils.Game;
using Quasar.GameUtils.Logic.Mode;
using Quasar.GameUtils.Player;
using Quasar.GameUtils.Sections;
using Quasar.Language;

namespace AvatarFarmOnline.Sections;

internal class ReceivingDataSection : GUISection
{
	public ReceivingDataSection()
		: base(26, new Layout("Garage", LanguageManager.Texts["JOIN_GAME_TITLE"], AvatarFarmOnline.Template.ExtendedGameTemplate.Template))
	{
		AvatarFarmOnline.Logic.Mode.Farm.FarmPersistentGameData farmPersistentGameData = GameManager.PersistentData as AvatarFarmOnline.Logic.Mode.Farm.FarmPersistentGameData;
		farmPersistentGameData.Online.OnBeginLoad += OnGameStart;
	}

	private void OnGameStart()
	{
		BaseGame.Instance.NextGameSectionId = 12;
	}

	public override void MainLoop()
	{
		Player.SetPresence(GamerPresenceMode.Multiplayer);
		base.MainLoop();
	}

	public override void Dispose()
	{
		if (GameManager.PersistentData is AvatarFarmOnline.Logic.Mode.Farm.FarmPersistentGameData { IsOnline: not false } farmPersistentGameData)
		{
			farmPersistentGameData.Online.OnBeginLoad -= OnGameStart;
		}
		base.Dispose();
	}
}
