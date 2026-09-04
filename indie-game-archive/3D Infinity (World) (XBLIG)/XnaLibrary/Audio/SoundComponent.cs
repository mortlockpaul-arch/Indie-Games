using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace XnaLibrary.Audio;

public class SoundComponent : GameComponent
{
	private const string DefaultKeyName = "Default";

	private const string DefaultAudioEnginePath = "Content/Audio/Audio.xgs";

	private const string DefaultWaveBankPath = "Content/Audio/Wave Bank.xwb";

	private const string DefaultSoundBankPath = "Content/Audio/Sound Bank.xsb";

	private const string VolumeVariableName = "Volume";

	private const string PitchVariableName = "Pitch";

	private const string ReverbVariableName = "SpeedOfSound";

	private Dictionary<string, WaveBank> waveBankList;

	private Dictionary<string, SoundBank> soundBankList;

	public string AudioEnginePath { get; private set; }

	public AudioEngine AudioEngine { get; private set; }

	public SoundComponent(Game game)
		: this(game, "Content/Audio/Audio.xgs")
	{
	}

	public SoundComponent(Game game, string audioEnginePath)
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Expected O, but got Unknown
		waveBankList = new Dictionary<string, WaveBank>();
		soundBankList = new Dictionary<string, SoundBank>();
		((GameComponent)this)._002Ector(game);
		AudioEnginePath = audioEnginePath;
		if (File.Exists(AudioEnginePath))
		{
			AudioEngine = new AudioEngine(AudioEnginePath);
		}
	}

	public override void Initialize()
	{
		if (File.Exists("Content/Audio/Wave Bank.xwb") && File.Exists("Content/Audio/Sound Bank.xsb"))
		{
			EntryBank("Default", "Content/Audio/Wave Bank.xwb", "Content/Audio/Sound Bank.xsb", isStream: false);
		}
		((GameComponent)this).Initialize();
	}

	public void EntryBank(string name, string waveBankPath, string soundBankPath, bool isStream)
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Expected O, but got Unknown
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Expected O, but got Unknown
		if (!waveBankList.ContainsKey(name) && !soundBankList.ContainsKey(name))
		{
			if (AudioEngine != null)
			{
				WaveBank value = (isStream ? new WaveBank(AudioEngine, waveBankPath, 0, (short)4) : new WaveBank(AudioEngine, waveBankPath));
				SoundBank value2 = new SoundBank(AudioEngine, soundBankPath);
				waveBankList.Add(name, value);
				soundBankList.Add(name, value2);
				AudioEngine.Update();
			}
			return;
		}
		throw new ArgumentException("既に同一のキーが含まれています。", name);
	}

	public void RemoveBank(string name)
	{
		if (waveBankList.ContainsKey(name))
		{
			waveBankList[name].Dispose();
			waveBankList.Remove(name);
		}
		if (soundBankList.ContainsKey(name))
		{
			soundBankList[name].Dispose();
			soundBankList.Remove(name);
		}
	}

	protected override void Dispose(bool disposing)
	{
		foreach (KeyValuePair<string, WaveBank> waveBank in waveBankList)
		{
			if (!waveBank.Value.IsDisposed)
			{
				waveBank.Value.Dispose();
			}
		}
		foreach (KeyValuePair<string, SoundBank> soundBank in soundBankList)
		{
			if (!soundBank.Value.IsDisposed)
			{
				soundBank.Value.Dispose();
			}
		}
		if (AudioEngine != null && !AudioEngine.IsDisposed)
		{
			AudioEngine.Dispose();
		}
		((GameComponent)this).Dispose(disposing);
	}

	public override void Update(GameTime gameTime)
	{
		if (AudioEngine != null && !AudioEngine.IsDisposed)
		{
			AudioEngine.Update();
		}
		((GameComponent)this).Update(gameTime);
	}

	public Cue GetCue(string key, string cueName)
	{
		if (soundBankList.ContainsKey(key))
		{
			return soundBankList[key].GetCue(cueName);
		}
		throw new KeyNotFoundException();
	}

	public Cue GetCue(string cueName)
	{
		return GetCue("Default", cueName);
	}

	public Cue PlayBGM(string key, string cueName)
	{
		Cue cue = GetCue(key, cueName);
		PlayBGM(cue);
		return cue;
	}

	public Cue PlayBGM(string cueName)
	{
		return PlayBGM("Default", cueName);
	}

	public void PlayBGM(Cue cue)
	{
		if (cue != null && !cue.IsDisposed)
		{
			cue.Play();
		}
	}

	public void PlaySE(string key, string cueName)
	{
		if (soundBankList.ContainsKey(key))
		{
			soundBankList[key].PlayCue(cueName);
			return;
		}
		throw new KeyNotFoundException();
	}

	public void PlaySE(string cueName)
	{
		PlaySE("Default", cueName);
	}

	public void SetVariable(Cue cue, string name, float value)
	{
		if (cue != null && !cue.IsDisposed)
		{
			cue.SetVariable(name, value);
		}
	}

	public void SetVolume(Cue cue, float value)
	{
		float value2 = 100f * MathHelper.Clamp(value, 0f, 1f);
		SetVariable(cue, "Volume", value2);
	}

	public void SetPitch(Cue cue, float value)
	{
		float value2 = 12f * MathHelper.Clamp(value, -1f, 1f);
		SetVariable(cue, "Pitch", value2);
	}

	public void SetReverb(float value)
	{
		AudioEngine.SetGlobalVariable("SpeedOfSound", value);
	}

	public void Stop(Cue cue, AudioStopOptions options)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		if (cue != null && !cue.IsDisposed && cue.IsPlaying)
		{
			cue.Stop(options);
		}
	}

	public void Stop(Cue cue)
	{
		Stop(cue, (AudioStopOptions)1);
	}
}
