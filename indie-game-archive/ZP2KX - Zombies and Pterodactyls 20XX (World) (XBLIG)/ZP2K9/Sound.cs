using Microsoft.Xna.Framework.Audio;

namespace ZP2K9;

public class Sound
{
	private const string DEFAULT = "Default";

	private const string MUSIC = "Music";

	public const float MAX_CONFIRM_TIME = 0.15f;

	private const string BRASS_SND = "brass";

	private static AudioEngine engine;

	private static SoundBank sound;

	private static WaveBank wave;

	public static float confirmTime;

	private static float brassFrame;

	public static void Initialize()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Expected O, but got Unknown
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Expected O, but got Unknown
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Expected O, but got Unknown
		engine = new AudioEngine("Content/sfx/sfxproj.xgs");
		wave = new WaveBank(engine, "Content/sfx/wav.xwb");
		sound = new SoundBank(engine, "Content/sfx/snd.xsb");
	}

	public static AudioEngine GetEngine()
	{
		return engine;
	}

	public static void PlayCue(string cue)
	{
		try
		{
			sound.PlayCue(cue);
		}
		catch
		{
		}
	}

	public static Cue GetCue(string cue)
	{
		return sound.GetCue(cue);
	}

	public static void PlayBrass()
	{
		if (brassFrame <= 0f)
		{
			PlayCue("brass");
			brassFrame = 0.1f;
		}
	}

	public static void Update()
	{
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		if (confirmTime > 0f)
		{
			confirmTime -= Game1.frameTime;
		}
		if (brassFrame > 0f)
		{
			brassFrame -= Game1.frameTime;
		}
		engine.Update();
		AudioCategory category = engine.GetCategory("Default");
		((AudioCategory)(ref category)).SetVolume((float)Game1.settings.sfx / 10f);
		AudioCategory category2 = engine.GetCategory("Music");
		((AudioCategory)(ref category2)).SetVolume((float)Game1.settings.bgm / 10f);
	}

	internal static void PlayConfirm()
	{
		if (confirmTime <= 0f)
		{
			PlayCue("confirm");
			confirmTime = 0.15f;
		}
	}

	internal static void DoLevup()
	{
		PlayCue("levup");
		Music.Reset();
	}
}
