using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace JamSouls;

public class AudioManager
{
	private static AudioEngine m_AudioEngine;

	private static WaveBank m_WaveBank;

	public static SoundBank m_SoundBank;

	private static AudioCategory m_Category;

	private static bool bInit = false;

	private static float SavedvolumeValue = -1f;

	public AudioManager()
	{
		m_AudioEngine = new AudioEngine("Content\\Audio\\JamSound.xgs");
		m_WaveBank = new WaveBank(m_AudioEngine, "Content\\Audio\\Wave Bank.xwb");
		m_SoundBank = new SoundBank(m_AudioEngine, "Content\\Audio\\Sound Bank.xsb");
		m_Category = m_AudioEngine.GetCategory("Default");
		PlayerConfig.JumpSound = new AudioClip("Char_Jump");
		if (SavedvolumeValue != -1f)
		{
			m_Category.SetVolume(SavedvolumeValue / 5f);
		}
		else
		{
			m_Category.SetVolume(MediaPlayer.Volume);
		}
		bInit = true;
	}

	public static void SetSfxVolume(float vol)
	{
		if (bInit)
		{
			m_Category.SetVolume(vol / 4f);
		}
		else
		{
			SavedvolumeValue = vol;
		}
	}

	public static void Update()
	{
		if (bInit)
		{
			m_AudioEngine.Update();
		}
	}
}
