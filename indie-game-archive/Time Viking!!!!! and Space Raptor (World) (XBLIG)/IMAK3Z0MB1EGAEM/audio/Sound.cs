using System;
using Microsoft.Xna.Framework.Audio;

namespace IMAK3Z0MB1EGAEM.audio;

public class Sound
{
	private static AudioEngine engine;

	private static SoundBank sound;

	private static WaveBank wave;

	public static void Init()
	{
		engine = new AudioEngine("Content/sfx/sfxproj4.xgs");
		wave = new WaveBank(engine, "Content/sfx/fx.xwb");
		sound = new SoundBank(engine, "Content/sfx/fx.xsb");
		engine.GetCategory("Default").SetVolume(0.5f);
	}

	public static void Play(string s)
	{
		try
		{
			sound.PlayCue(s);
		}
		catch (Exception)
		{
		}
	}

	public static void Update()
	{
		engine.Update();
	}
}
