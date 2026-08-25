using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace RenegadeEngine;

public static class SoundMgr
{
	private static bool initialized = false;

	private static AudioEngine audioEngine;

	private static SoundBank soundBank;

	private static WaveBank waveBank;

	private static Cue variableCue;

	private static Cue cue3D;

	private static AudioListener audioListener;

	private static AudioEmitter audioEmitter;

	private static Cue menuMusicCue;

	private static Cue gameplayMusicCue;

	private static Song song1;

	private static Song song2;

	private static bool start2 = false;

	public static bool MusicMuted { get; private set; }

	public static bool SoundMuted { get; set; }

	public static void MuteMusic(bool muteMusic)
	{
		MusicMuted = muteMusic;
		if (MusicMuted)
		{
			MediaPlayer.Stop();
			start2 = false;
		}
		else
		{
			PlaySong();
		}
	}

	public static void Initialize()
	{
		AssetManager.GetAsset(MusicKeys.TimeToDream, out song1);
		AssetManager.GetAsset(MusicKeys.ThisMachine, out song2);
		initialized = true;
	}

	public static void Dispose()
	{
		if (soundBank != null)
		{
			soundBank.Dispose();
		}
		if (waveBank != null)
		{
			waveBank.Dispose();
		}
		if (audioEngine != null)
		{
			audioEngine.Dispose();
		}
		if (menuMusicCue != null)
		{
			menuMusicCue.Dispose();
		}
		if (gameplayMusicCue != null)
		{
			gameplayMusicCue.Dispose();
		}
		if (song1 != null)
		{
			song1.Dispose();
		}
		if (song2 != null)
		{
			song2.Dispose();
		}
	}

	public static void PlaySound(string sound)
	{
		if (initialized && !SoundMuted)
		{
			soundBank.PlayCue(sound);
		}
	}

	public static void PlaySound(string sound, string variable, float varValue)
	{
		if (initialized)
		{
			variableCue = soundBank.GetCue(sound);
			variableCue.SetVariable(variable, varValue);
			variableCue.Play();
		}
	}

	public static void PlaySound3D(string sound, Vector3 emitterPosition)
	{
		if (initialized)
		{
			audioEmitter.Position = emitterPosition;
			cue3D = soundBank.GetCue(sound);
			cue3D.Apply3D(audioListener, audioEmitter);
			cue3D.Play();
		}
	}

	public static void PlaySound3D(string sound, Vector3 emitterPosition, string variable, float varValue)
	{
		if (initialized)
		{
			audioEmitter.Position = emitterPosition;
			cue3D = soundBank.GetCue(sound);
			cue3D.SetVariable(variable, varValue);
			cue3D.Apply3D(audioListener, audioEmitter);
			cue3D.Play();
		}
	}

	public static void PlaySong()
	{
		MediaPlayer.Play(song1);
	}

	public static void Check()
	{
		if (!MusicMuted && MediaPlayer.State == MediaState.Stopped)
		{
			if (!start2)
			{
				MediaPlayer.Play(song2);
				start2 = true;
			}
			else
			{
				MediaPlayer.Play(song1);
				start2 = false;
			}
		}
	}

	public static void Update()
	{
		audioEngine.Update();
	}

	public static void UpdateListenerPosition(Vector3 listenerPosition)
	{
		audioListener.Position = listenerPosition;
	}
}
