using AvatarFarmOnline.Logic.Mode.Farm;
using AvatarFarmOnline.Logic.Mode.Farm.Online;
using AvatarFarmOnline.Template;
using AvatarFarmOnline.Template.Controls;
using Microsoft.Xna.Framework;
using Quasar.GUI;
using Quasar.GameUtils.Game;
using Quasar.GameUtils.Logic.Mode;
using Quasar.GameUtils.Network;
using Quasar.GameUtils.Player;
using Quasar.GameUtils.Sections;
using Quasar.GameUtils.Tasks;
using Quasar.Language;

namespace AvatarFarmOnline.Sections;

internal class JoinedGameSection : GUISection
{
	private AvatarFarmOnline.Template.Controls.JoinGameStatus joinGameStatus;

	private bool success;

	private SessionJoinError error;

	public JoinedGameSection()
		: base(28, new Layout("Garage", LanguageManager.Texts["JOIN_GAME_TITLE"], AvatarFarmOnline.Template.ExtendedGameTemplate.Template))
	{
		joinGameStatus = new AvatarFarmOnline.Template.Controls.JoinGameStatus(base.Layout);
		base.Layout.AddControl(joinGameStatus);
		TaskManager.Post(Search, null, SearchFinished);
	}

	private void Search(object parameters)
	{
		AvatarFarmOnline.Logic.Mode.Farm.FarmPersistentGameData farmPersistentGameData = GameManager.PersistentData as AvatarFarmOnline.Logic.Mode.Farm.FarmPersistentGameData;
		success = farmPersistentGameData.Online.JoinInvited(out error);
	}

	private void OnGameStart()
	{
		BaseGame.Instance.NextGameSectionId = 12;
	}

	private void SearchFinished(object parameters)
	{
		if (success)
		{
			AvatarFarmOnline.Logic.Mode.Farm.FarmPersistentGameData farmPersistentGameData = GameManager.PersistentData as AvatarFarmOnline.Logic.Mode.Farm.FarmPersistentGameData;
			if (farmPersistentGameData.Online.MultiplayerState == AvatarFarmOnline.Logic.Mode.Farm.Online.Online.MultiplayerStates.Loading)
			{
				BaseGame.Instance.NextGameSectionId = 12;
			}
			else
			{
				BaseGame.Instance.NextGameSection = new AvatarFarmOnline.Sections.ReceivingDataSection();
			}
		}
		else
		{
			string message = error switch
			{
				SessionJoinError.SessionFull => "ERROR_SESSION_FULL".Translate(), 
				SessionJoinError.SessionNotJoinable => "ERROR_SESSION_NOT_JOINABLE".Translate(), 
				_ => "ERROR_SESSION_NOT_FOUND".Translate(), 
			};
			base.Layout.ShowDialog("ERROR".Translate(), message, DialogOptions.Ok, OnErrorConfirm);
		}
	}

	private void OnErrorConfirm(DialogResult result, PlayerIndex whoPressed)
	{
		AvatarFarmOnline.Logic.Mode.Farm.FarmPersistentGameData farmPersistentGameData = GameManager.PersistentData as AvatarFarmOnline.Logic.Mode.Farm.FarmPersistentGameData;
		farmPersistentGameData.Online.Reset();
		BaseGame.Instance.NextGameSectionId = 0;
	}

	public override void MainLoop()
	{
		Player.SetPresence(GamerPresenceMode.Multiplayer);
		base.MainLoop();
	}
}
