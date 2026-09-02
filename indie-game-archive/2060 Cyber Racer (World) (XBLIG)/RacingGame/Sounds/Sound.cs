using System;
using System.IO;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using RacingGame.Graphics;
using RacingGame.Helpers;
using RacingGame.Properties;

namespace RacingGame.Sounds;

internal class Sound
{
	public enum Sounds
	{
		ButtonClick,
		ScreenClick,
		ScreenBack,
		Highlight,
		Beep,
		Bleep,
		BrakeCurveMajor,
		BrakeCurveMinor,
		BrakeMajor,
		BrakeMinor,
		CarCrashMinor,
		CarCrashTotal,
		CheckpointBetter,
		CheckpointWorse,
		Victory,
		CarLose,
		MenuMusic,
		GameMusic
	}

	private const int NumberOfGears = 5;

	private const int GearChangeSoundLengthInMs = 1200;

	private const float stayingVol = 0.5f;

	private static AudioEngine audioEngine;

	private static WaveBank waveBank;

	private static SoundBank soundBank;

	private static AudioCategory defaultCategory;

	private static AudioCategory gearsCategory;

	private static AudioCategory musicCategory;

	private static float brakeSoundStillPlayingMs = 1000f;

	private static float crashSoundStillPlayingMs = 2000f;

	private static readonly float[] vol = new float[5] { 1f, 1f, 1f, 1f, 1f };

	private static readonly float[] minPitch = new float[5] { -0.375f, -0.375f, -0.345f, -0.25f, -0.205f };

	private static readonly float[] maxPitch = new float[5] { 0.24f, 0.17f, 0.17f, 0.145f, 0.1f };

	private static int currentGear = 0;

	private static Cue currentGearCue = null;

	private static Cue currentGearChangeCue = null;

	private static float gearChangeSoundInitiatedMs = 0f;

	private static float lastGearVolume = 0.5f;

	private static float lastGearPitch = 0f;

	private Sound()
	{
	}

	public static void Initialize()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Expected O, but got Unknown
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Expected O, but got Unknown
		//IL_00ae: Expected O, but got Unknown
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Expected O, but got Unknown
		try
		{
			string soundsDirectory = Directories.SoundsDirectory;
			audioEngine = new AudioEngine(Path.Combine(soundsDirectory, "RacingGameManager.xgs"));
			waveBank = new WaveBank(audioEngine, Path.Combine(soundsDirectory, "Wave Bank.xwb"));
			if (waveBank != null)
			{
				soundBank = new SoundBank(audioEngine, Path.Combine(soundsDirectory, "Sound Bank.xsb"));
			}
			defaultCategory = audioEngine.GetCategory("Default");
			gearsCategory = audioEngine.GetCategory("Gears");
			musicCategory = audioEngine.GetCategory("Music");
			SetVolumes(GameSettings.Default.SoundVolume, GameSettings.Default.MusicVolume);
		}
		catch (NoAudioHardwareException ex)
		{
			NoAudioHardwareException ex2 = ex;
			Log.Write("Failed to create sound class: " + ((object)ex2).ToString());
		}
	}

	public static void Play(string soundName)
	{
		if (soundBank != null)
		{
			soundBank.PlayCue(soundName);
		}
	}

	public static void Play(Sounds sound)
	{
		Play(sound.ToString());
	}

	public static void StopMusic()
	{
		if (soundBank != null)
		{
			Cue cue = soundBank.GetCue("MenuMusic");
			cue.Play();
			Thread.Sleep(10);
			cue.Stop((AudioStopOptions)1);
		}
	}

	public static void PlayBrakeSound(Sounds soundBrakeType)
	{
		if (brakeSoundStillPlayingMs <= 0f && !RacingGameManager.InMenu)
		{
			Play(soundBrakeType);
			switch (soundBrakeType)
			{
			case Sounds.BrakeMinor:
				brakeSoundStillPlayingMs = 750f;
				break;
			case Sounds.BrakeMajor:
				brakeSoundStillPlayingMs = 2500f;
				break;
			case Sounds.BrakeCurveMinor:
				brakeSoundStillPlayingMs = 1250f;
				break;
			case Sounds.BrakeCurveMajor:
				brakeSoundStillPlayingMs = 3500f;
				break;
			}
		}
	}

	public static Sounds GetBreakSoundType(float speed, float speedChange, float rotationChange)
	{
		bool flag = rotationChange > 0.325f * BaseGame.MoveFactorPerSecond;
		Sounds result = (flag ? Sounds.BrakeCurveMinor : Sounds.BrakeMinor);
		if (speed > 1.5f && Math.Abs(speedChange) > 5f * BaseGame.MoveFactorPerSecond)
		{
			result = (flag ? Sounds.BrakeCurveMajor : Sounds.BrakeMajor);
		}
		return result;
	}

	public static void PlayCrashSound(bool totalCrash)
	{
		if (crashSoundStillPlayingMs <= 0f && !RacingGameManager.InMenu)
		{
			Play(totalCrash ? Sounds.CarCrashTotal : Sounds.CarCrashMinor);
			crashSoundStillPlayingMs = (totalCrash ? 3456 : 2345);
		}
	}

	private static void PlayGearSound(string soundName)
	{
		if (soundBank != null)
		{
			if (soundName.Contains("To"))
			{
				currentGearChangeCue = soundBank.GetCue(soundName);
				currentGearChangeCue.Play();
				gearChangeSoundInitiatedMs = 1200f;
				currentGearCue = null;
			}
			else
			{
				currentGearCue = soundBank.GetCue(soundName);
				currentGearCue.Play();
				currentGearChangeCue = null;
			}
		}
	}

	private static void UpdateGearVolumeAndPitch(string gearSound, float volume, float pitch)
	{
		if (audioEngine == null)
		{
			return;
		}
		if (gearChangeSoundInitiatedMs > 0f)
		{
			gearChangeSoundInitiatedMs -= BaseGame.ElapsedTimeThisFrameInMilliseconds;
			if (gearChangeSoundInitiatedMs <= 0f)
			{
				gearChangeSoundInitiatedMs = 0f;
				PlayGearSound(gearSound);
				volume = (lastGearVolume = 1f);
				pitch = (lastGearPitch = -0.3f);
			}
		}
		((AudioCategory)(ref gearsCategory)).SetVolume(MathHelper.Clamp(volume, 0f, 1f) * GameSettings.Default.SoundVolume);
		if (currentGearCue != null)
		{
			currentGearCue.SetVariable("Pitch", 55f * MathHelper.Clamp(pitch, -1f, 1f));
		}
	}

	public static void StartGearSound()
	{
		currentGear = 0;
		PlayGearSound("Gear1");
		UpdateGearVolumeAndPitch("Gear1", 0.5f, minPitch[0]);
	}

	public static void StopGearSound()
	{
		currentGear = 0;
		if (currentGearChangeCue != null)
		{
			currentGearChangeCue.Stop((AudioStopOptions)1);
		}
		currentGearChangeCue = null;
		if (currentGearCue != null)
		{
			currentGearCue.Stop((AudioStopOptions)1);
		}
		currentGearCue = null;
	}

	public static void UpdateGearSound(float speed, float acceleration)
	{
		int num = (int)(5f * speed / 50.0549f);
		if (num < 0)
		{
			num = 0;
		}
		if (num >= 5)
		{
			num = 4;
		}
		if (gearChangeSoundInitiatedMs <= 0f)
		{
			if (num > currentGear)
			{
				PlayGearSound("Gear" + num + "ToGear" + (num + 1));
				lastGearVolume = 1f;
				lastGearPitch = 0f;
			}
			else if (num < currentGear)
			{
				PlayGearSound("Gear" + (num + 1));
				lastGearVolume = 1f;
				lastGearPitch = maxPitch[num];
			}
			currentGear = num;
		}
		if (speed < 0f)
		{
			speed = MathHelper.Clamp(Math.Abs(speed), 0f, 10.010981f);
		}
		float num2 = (float)((int)(speed / 50.0549f * 499f) % 100) / 100f;
		num2 = MathHelper.Clamp(num2, 0f, 1f);
		float num3 = ((currentGear > 0) ? vol[currentGear - 1] : 0.5f);
		float num4 = vol[currentGear];
		float num5 = MathHelper.Lerp(num3, num4, num2);
		float num6 = MathHelper.Lerp(minPitch[currentGear], maxPitch[currentGear], num2);
		if (gearChangeSoundInitiatedMs > 0f)
		{
			num6 = 0f;
		}
		if (acceleration > 0.25f)
		{
			num5 = 1f;
		}
		else
		{
			num5 /= 1.75f;
			num6 = Math.Min(-0.025f, num6 / 1.25f);
			if (lastGearPitch > num6)
			{
				lastGearPitch = lastGearPitch * 0.9f + num6 * 0.1f;
			}
		}
		lastGearVolume = MathHelper.Lerp(lastGearVolume, num5, 5f * BaseGame.MoveFactorPerSecond);
		lastGearPitch = MathHelper.Lerp(lastGearPitch, num6, 5f * BaseGame.MoveFactorPerSecond);
		UpdateGearVolumeAndPitch("Gear" + (currentGear + 1), lastGearVolume, lastGearPitch);
	}

	public static void Update()
	{
		if (brakeSoundStillPlayingMs > 0f)
		{
			brakeSoundStillPlayingMs -= BaseGame.ElapsedTimeThisFrameInMilliseconds;
		}
		if (crashSoundStillPlayingMs > 0f)
		{
			crashSoundStillPlayingMs -= BaseGame.ElapsedTimeThisFrameInMilliseconds;
		}
		if (audioEngine != null)
		{
			audioEngine.Update();
		}
	}

	public static void SetVolumes(float soundVolume, float musicVolume)
	{
		if (audioEngine != null)
		{
			((AudioCategory)(ref defaultCategory)).SetVolume(soundVolume);
			((AudioCategory)(ref musicCategory)).SetVolume(musicVolume);
		}
	}
}
