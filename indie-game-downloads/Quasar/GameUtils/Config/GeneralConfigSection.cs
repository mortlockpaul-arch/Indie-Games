using Quasar.GameUtils.Audio;
using Quasar.Input;
using Quasar.Language;

namespace Quasar.GameUtils.Config;

public class GeneralConfigSection : ConfigSection
{
	private const string GENERAL = "General";

	private const string LANGUAGE = "Language";

	private const string SOUND_VOLUME = "Sound";

	private const string MUSIC_VOLUME = "Music";

	private const string VIBRATION_ENABLED = "Vibration";

	public bool VibrationEnabled
	{
		get
		{
			return GetItem<bool>("Vibration").Value;
		}
		set
		{
			GetItem<bool>("Vibration").Value = value;
		}
	}

	public float MusicVolume
	{
		get
		{
			return GetItem<float>("Music").Value;
		}
		set
		{
			GetItem<float>("Music").Value = value;
		}
	}

	public float SoundVolume
	{
		get
		{
			return GetItem<float>("Sound").Value;
		}
		set
		{
			GetItem<float>("Sound").Value = value;
		}
	}

	public GeneralConfigSection(float defaultSoundVolume, float defaultMusicVolume)
		: base("General")
	{
		AddItem(ConfigParameter.CreateFloatParameter("Sound", defaultSoundVolume));
		AddItem(ConfigParameter.CreateFloatParameter("Music", defaultMusicVolume));
		AddItem(ConfigParameter.CreateStringParameter("Language", LanguageManager.Language));
		AddItem(ConfigParameter.CreateBoolParameter("Vibration", defaultValue: true));
	}

	protected override void Gather()
	{
		GetItem<float>("Music").Value = Quasar.Audio.MusicVolume;
		GetItem<float>("Sound").Value = Quasar.Audio.SoundVolume;
		GetItem<string>("Language").Value = LanguageManager.Language;
		GetItem<bool>("Vibration").Value = Gamepad.VibrationEnabled;
	}

	public override void Apply()
	{
		base.Apply();
		Quasar.Audio.MusicVolume = GetItem<float>("Music").Value;
		if (XACTJukebox.HasInstance)
		{
			XACTJukebox.Instance.UpdateVolume();
		}
		Quasar.Audio.SoundVolume = GetItem<float>("Sound").Value;
		LanguageManager.Language = GetItem<string>("Language").Value;
		Gamepad.VibrationEnabled = GetItem<bool>("Vibration").Value;
	}
}
