using System;
using AvatarFarmOnline.Scores;
using AvatarFarmOnline.Template;
using Microsoft.Xna.Framework;
using Quasar;
using Quasar.GUI;
using Quasar.GUI.Controls;
using Quasar.GUI.Controls.GroupControls;
using Quasar.GameUtils.Audio;
using Quasar.GameUtils.Game;
using Quasar.GameUtils.Player;
using Quasar.GameUtils.Sections;
using Quasar.Global;
using Quasar.Input;
using Quasar.Language;

namespace AvatarFarmOnline.Sections;

internal class SettingsSection : GUISection
{
	private Selector audioSelector;

	private Selector musicSelector;

	private Selector vibrationSelector;

	private Selector p2pSelector;

	private Selector invertYAxisSelector;

	public SettingsSection()
		: base(13, new Layout("", LanguageManager.Texts["SETTINGS_TITLE"], AvatarFarmOnline.Template.ExtendedGameTemplate.Template))
	{
		Group obj = new Group(base.Layout);
		obj.SetId("MainMenuGroup");
		audioSelector = base.Layout.AddSelector(obj, "Audio", "AUDIO_VOLUME".Translate(), loop: false);
		musicSelector = base.Layout.AddSelector(obj, "Music", "MUSIC_VOLUME".Translate(), loop: false);
		vibrationSelector = base.Layout.AddSelector(obj, "Vibration", "VIBRATION".Translate(), loop: true);
		invertYAxisSelector = base.Layout.AddSelector(obj, "InvertYAxis", "INVERT_Y_AXIS".Translate(), loop: true);
		p2pSelector = base.Layout.AddSelector(obj, "P2P", "P2P_SCORES".Translate(), loop: true);
		obj.OnCancel += OnCancel;
		base.Layout.AddControl(obj);
		FillVolumes();
		FillVibration();
		FillInvertAxis();
		FillP2P();
		base.Layout.OnCancel += OnCancel;
		ButtonInstructions buttonInstructions = new ButtonInstructions(base.Layout);
		buttonInstructions.AddInstructions(InputManager.MenuInputCodes.Interact, "OK".Translate());
		buttonInstructions.AddInstructions(InputManager.MenuInputCodes.Cancel, "BACK".Translate());
		base.Layout.AddControl(buttonInstructions);
	}

	private void FillVibration()
	{
		vibrationSelector.OnInteraction += OnCancel;
		vibrationSelector.OnChange += OnVibrationChange;
		vibrationSelector.addOption(0, LanguageManager.Texts["OFF"]);
		vibrationSelector.addOption(1, LanguageManager.Texts["ON"]);
		vibrationSelector.CurrentOption = (AvatarFarmOnline.AvatarFarmOnlineConfig.VibrationEnabled ? 1 : 0);
	}

	private void FillInvertAxis()
	{
		invertYAxisSelector.OnInteraction += OnCancel;
		invertYAxisSelector.addOption(0, LanguageManager.Texts["OFF"]);
		invertYAxisSelector.addOption(1, LanguageManager.Texts["ON"]);
		invertYAxisSelector.CurrentOption = (AvatarFarmOnline.AvatarFarmOnlineConfig.Instance.InvertYAxis ? 1 : 0);
	}

	private void FillP2P()
	{
		p2pSelector.OnInteraction += OnCancel;
		p2pSelector.addOption(0, LanguageManager.Texts["OFF"]);
		p2pSelector.addOption(1, LanguageManager.Texts["ON"]);
		p2pSelector.CurrentOption = (AvatarFarmOnline.AvatarFarmOnlineConfig.Instance.P2PEnabled ? 1 : 0);
	}

	private void OnVibrationChange(Selector selectorChanged, int selection, string value)
	{
		if (selection == 1 && InputManager.PlayerIndices.Count == 1)
		{
			Gamepad.Instance(InputManager.PlayerIndices[0]).vibrateFor(0.4f, 0.4f, 333u);
		}
	}

	private void FillVolumes()
	{
		audioSelector.OnInteraction += OnCancel;
		musicSelector.OnInteraction += OnCancel;
		for (int i = 0; i <= 10; i++)
		{
			audioSelector.addOption(i, i * 10 + "%");
			musicSelector.addOption(i, i * 10 + "%");
		}
		audioSelector.CurrentOption = (int)Math.Round(Audio.SoundVolume * 10f);
		musicSelector.CurrentOption = (int)Math.Round(Audio.MusicVolume * 10f);
		audioSelector.OnChange += SetAudioVolume;
		musicSelector.OnChange += SetMusicVolume;
	}

	private void OnCancel(Button buttonPressed, PlayerIndex whoPressed)
	{
		GoBack();
	}

	private void SetAudioVolume(Selector selectorChanged, int selection, string value)
	{
		Audio.SoundVolume = (float)selection * 0.1f;
	}

	private void SetMusicVolume(Selector selectorChanged, int selection, string value)
	{
		Audio.MusicVolume = (float)selection * 0.1f;
		XACTJukebox.Instance.Volume = Audio.MusicVolume;
	}

	private bool OnCancel(Group g, PlayerIndex whoPressed)
	{
		GoBack();
		return true;
	}

	private bool OnCancel(Layout layout, PlayerIndex whoPressed)
	{
		GoBack();
		return true;
	}

	private void GoBack()
	{
		Gamepad.VibrationEnabled = vibrationSelector.CurrentOption == 1;
		AvatarFarmOnline.AvatarFarmOnlineConfig.Instance.InvertYAxis = invertYAxisSelector.CurrentOption == 1;
		AvatarFarmOnline.Scores.GameScoreManager.EnableP2P = p2pSelector.CurrentOption == 1;
		AvatarFarmOnline.AvatarFarmOnlineConfig.SaveConfig();
		((BaseGame)Engine.Game).NextGameSectionId = 14;
	}

	public override void MainLoop()
	{
		Player.SetPresence(GamerPresenceMode.AtMenu);
		base.MainLoop();
	}
}
