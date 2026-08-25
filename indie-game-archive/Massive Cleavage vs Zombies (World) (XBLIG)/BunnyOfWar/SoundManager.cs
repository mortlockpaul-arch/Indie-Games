using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;

namespace BunnyOfWar;

public static class SoundManager
{
	private static Dictionary<string, SoundEffect> sounds = new Dictionary<string, SoundEffect>();

	private static bool gameHasControl = false;

	private static bool checkedControl = false;

	private static DateTime lastMusicRequestTime = DateTime.MinValue;

	private static string currentSongNamePlaying = "";

	private static SoundEffect[] punches;

	private static SoundEffect[] clangs;

	private static SoundEffect[] splats;

	public static SoundEffect laser1;

	public static SoundEffect laser2;

	public static SoundEffect laser3;

	private static SoundEffect slowWhoosh;

	private static SoundEffect quickWhoosh;

	private static int punchPosition = 0;

	private static int clangPosition = 0;

	private static int splatPosition = 0;

	public static int fartPosition = 0;

	private static int zombieMoansPosition = 0;

	private static string[] zombieMoans = new string[15]
	{
		"zombie moans/00831_monster_in_face_316.wav", "zombie moans/01868_monster_moan16.wav", "zombie moans/02424_monster_moans16.wav", "zombie moans/FF_2FX0036416.wav", "zombie moans/FF_2FX0036516.wav", "zombie moans/FF_2FX0038816.wav", "zombie moans/FF_2FX0042416.wav", "zombie moans/FF_2FX0044316.wav", "zombie moans/FF_2FX0572816.wav", "zombie moans/FF_2FX21438dog16.wav",
		"zombie moans/FF_2FX21441best16.wav", "zombie moans/FF_2FX2144216.wav", "zombie moans/FF_2FX2221316.wav", "zombie moans/PEOP_08416.wav", "zombie moans/PEOP_21216.wav"
	};

	private static int dogGrowlsPosition = 0;

	private static string[] dogGrowls = new string[7] { "doggy/00831_monster_in_face_316.wav", "doggy/04155_slow_animal_whirr16.wav", "doggy/05137_tiger_monster_whirr16.wav", "doggy/05731_zombie_snore_attack16.wav", "doggy/05865_short_monster_whirr16.wav", "doggy/FF_2FX21438dog16.wav", "doggy/Slow Close Zombie Whirr 00831.wav" };

	public static void ClearCache()
	{
		sounds.Clear();
	}

	public static void PlaySoundDirectly(SoundEffect soundin)
	{
		if (soundin == null)
		{
			return;
		}
		try
		{
			soundin.Play(Definitions.Options.MasterVolume * Definitions.Options.SoundsVolume, 0f, 0f);
		}
		catch (Exception ex)
		{
			ex.ToString();
		}
	}

	public static void PlaySound(string soundName)
	{
		PlaySound(soundName, 0f);
	}

	public static void PlaySound(string soundName, float pan)
	{
		if (pan < -1f)
		{
			pan = -1f;
		}
		if (pan > 1f)
		{
			pan = 1f;
		}
		try
		{
			if (sounds.ContainsKey(soundName))
			{
				sounds[soundName].Play(Definitions.Options.MasterVolume * Definitions.Options.SoundsVolume, 0f, pan);
				return;
			}
			try
			{
				sounds[soundName] = LoadSoundEffect("sounds/" + soundName);
				PlaySound(soundName);
			}
			catch (Exception)
			{
				sounds[soundName] = null;
			}
		}
		catch (Exception ex2)
		{
			_ = "sounds/" + soundName;
			_ = ex2.Message;
		}
	}

	public static bool isMusicPlaying()
	{
		if (MediaPlayer.State == MediaState.Playing)
		{
			return true;
		}
		return false;
	}

	public static bool DoesGameHaveControl()
	{
		if (checkedControl)
		{
			return gameHasControl;
		}
		gameHasControl = MediaPlayer.GameHasControl;
		checkedControl = true;
		return gameHasControl;
	}

	public static void PlayMusic(string musicName, bool IsRepeating)
	{
		lastMusicRequestTime = DateTime.Now;
		if (musicName == currentSongNamePlaying)
		{
			return;
		}
		currentSongNamePlaying = musicName;
		if (!DoesGameHaveControl())
		{
			return;
		}
		string assetName = "music/" + musicName;
		try
		{
			MediaPlayer.IsRepeating = IsRepeating;
			MediaPlayer.Volume = Definitions.Options.MasterVolume * Definitions.Options.MusicVolume;
			MediaPlayer.Play(RandomStaticGlobals.Content.Load<Song>(assetName));
		}
		catch (Exception ex)
		{
			_ = ex.Message;
		}
	}

	public static void UpdateVolumes()
	{
		MediaPlayer.Volume = Definitions.Options.MasterVolume * Definitions.Options.MusicVolume;
	}

	public static void StopMusic()
	{
		if (DoesGameHaveControl())
		{
			MediaPlayer.Stop();
		}
	}

	public static void PauseMusic()
	{
		if (DoesGameHaveControl())
		{
			MediaPlayer.Pause();
		}
	}

	public static void ResumeMusic()
	{
		if (DoesGameHaveControl())
		{
			MediaPlayer.Resume();
		}
	}

	public static SoundEffect LoadSoundEffect(string path)
	{
		return RandomStaticGlobals.Content.Load<SoundEffect>(path);
	}

	public static void LoadContent(ContentManager Content)
	{
		punches = new SoundEffect[11];
		clangs = new SoundEffect[12];
		splats = new SoundEffect[2];
		splats[0] = LoadSoundEffect("sounds/squirts/landing");
		splats[1] = LoadSoundEffect("sounds/squirts/landing2");
		punches[0] = LoadSoundEffect("sounds/squirts/squirt1");
		punches[1] = LoadSoundEffect("sounds/squirts/squirt2");
		punches[2] = LoadSoundEffect("sounds/squirts/squirt3");
		punches[3] = LoadSoundEffect("sounds/squirts/squirt4");
		punches[4] = LoadSoundEffect("sounds/squirts/squirt5");
		punches[5] = LoadSoundEffect("sounds/squirts/squirt6");
		punches[6] = LoadSoundEffect("sounds/squirts/squirt7");
		punches[7] = LoadSoundEffect("sounds/squirts/squirt8");
		punches[8] = LoadSoundEffect("sounds/squirts/squirt3a");
		punches[9] = LoadSoundEffect("sounds/squirts/squirt5a");
		punches[10] = LoadSoundEffect("sounds/squirts/squirt5b");
		slowWhoosh = LoadSoundEffect("sounds/SlowWhoosh");
		quickWhoosh = LoadSoundEffect("sounds/QuickWhoosh");
		for (int i = 0; i < zombieMoans.Length; i++)
		{
			LoadSoundEffect("sounds/" + zombieMoans[i].Replace(".wav", ""));
		}
		for (int j = 0; j < dogGrowls.Length; j++)
		{
			LoadSoundEffect("sounds/" + dogGrowls[j].Replace(".wav", ""));
		}
	}

	public static void PlayMenuClick()
	{
	}

	public static void playNextZombieMoan(bool isDog, int pan)
	{
		string text = "";
		if (isDog)
		{
			text = dogGrowls[dogGrowlsPosition];
			dogGrowlsPosition++;
			if (dogGrowlsPosition >= dogGrowls.Length)
			{
				dogGrowlsPosition = 0;
			}
		}
		else
		{
			text = zombieMoans[zombieMoansPosition];
			zombieMoansPosition++;
			if (zombieMoansPosition >= zombieMoans.Length)
			{
				zombieMoansPosition = 0;
			}
		}
		if (pan > 1)
		{
			pan = 1;
		}
		if (pan < -1)
		{
			pan = -1;
		}
		float num = Definitions.Options.MasterVolume * Definitions.Options.SoundsVolume * 0.8f;
		if (num > 1f)
		{
			num = 1f;
		}
		if (num < 0f)
		{
			num = 0f;
		}
		PlaySound(text.Replace(".wav", ""), pan);
	}

	public static void playNextQuickWhoosh(float pan)
	{
		if (quickWhoosh != null)
		{
			if (pan > 1f)
			{
				pan = 1f;
			}
			if (pan < 0f)
			{
				pan = 0f;
			}
			float num = Definitions.Options.MasterVolume * Definitions.Options.SoundsVolume * 0.8f;
			if (num > 1f)
			{
				num = 1f;
			}
			if (num < 0f)
			{
				num = 0f;
			}
			quickWhoosh.Play(num, 0f, pan);
		}
	}

	public static void playNextSlowWhoosh(float pan)
	{
		if (slowWhoosh != null)
		{
			if (punchPosition >= punches.Length)
			{
				punchPosition = 0;
			}
			if (pan > 1f)
			{
				pan = 1f;
			}
			if (pan < 0f)
			{
				pan = 0f;
			}
			float num = Definitions.Options.MasterVolume * Definitions.Options.SoundsVolume * 1.2f;
			if (num > 1f)
			{
				num = 1f;
			}
			if (num < 0f)
			{
				num = 0f;
			}
			slowWhoosh.Play(num, 0f, pan);
		}
	}

	public static void playNextGoreyHitStereo(float pan)
	{
		if (punches != null && punches.Length != 0)
		{
			if (punchPosition >= punches.Length)
			{
				punchPosition = 0;
			}
			if (pan > 1f)
			{
				pan = 1f;
			}
			if (pan < 0f)
			{
				pan = 0f;
			}
			if (punches != null && punches[punchPosition] != null)
			{
				punches[punchPosition].Play(Definitions.Options.MasterVolume * Definitions.Options.SoundsVolume, 0f, pan);
			}
			punchPosition++;
		}
	}

	public static void playNextClangStereo(float pan)
	{
		try
		{
			if (clangPosition >= clangs.Length)
			{
				clangPosition = 0;
			}
			if (pan > 1f)
			{
				pan = 1f;
			}
			if (pan < 0f)
			{
				pan = 0f;
			}
			clangs[clangPosition].Play(Definitions.Options.MasterVolume * Definitions.Options.SoundsVolume, -0.5f, pan);
			clangPosition++;
		}
		catch (Exception)
		{
		}
	}

	public static void playNextSplatStereo(float pan)
	{
		try
		{
			if (splatPosition >= splats.Length)
			{
				splatPosition = 0;
			}
			if (pan > 1f)
			{
				pan = 1f;
			}
			if (pan < 0f)
			{
				pan = 0f;
			}
			splats[splatPosition].Play(Definitions.Options.MasterVolume * Definitions.Options.SoundsVolume * 0.7f, -0.5f, pan);
			splatPosition++;
		}
		catch (Exception)
		{
		}
	}
}
