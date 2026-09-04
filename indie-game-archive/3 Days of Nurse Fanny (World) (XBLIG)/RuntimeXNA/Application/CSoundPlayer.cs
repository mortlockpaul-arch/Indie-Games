using Microsoft.Xna.Framework.Audio;
using RuntimeXNA.Banks;

namespace RuntimeXNA.Application;

public class CSoundPlayer
{
	private const int NCHANNELS = 32;

	private CRunApp app;

	private CSound[] channels;

	private bool bMultipleSounds;

	private bool bOn = true;

	private int[] volumes;

	private bool[] bLocked;

	private int[] pans;

	private int mainVolume;

	private int mainPan;

	public CSoundPlayer(CRunApp a)
	{
		app = a;
		channels = new CSound[32];
		volumes = new int[32];
		pans = new int[32];
		bLocked = new bool[32];
		bOn = true;
		bMultipleSounds = true;
		for (int i = 0; i < 32; i++)
		{
			channels[i] = null;
			volumes[i] = 100;
			pans[i] = 0;
		}
		mainVolume = 100;
		mainPan = 0;
	}

	public void reset()
	{
		for (int i = 0; i < 32; i++)
		{
			bLocked[i] = false;
		}
	}

	public void lockChannel(int channel)
	{
		if (channel >= 0 && channel < 32)
		{
			bLocked[channel] = true;
		}
	}

	public void unlockChannel(int channel)
	{
		if (channel >= 0 && channel < 32)
		{
			bLocked[channel] = false;
		}
	}

	public void play(short handle, int nLoops, int channel, bool bPrio)
	{
		if (!bOn)
		{
			return;
		}
		CSound cSound = app.soundBank.getSoundFromHandle(handle);
		if (cSound == null)
		{
			return;
		}
		if (!bMultipleSounds)
		{
			channel = 0;
		}
		else
		{
			for (int i = 0; i < 32; i++)
			{
				if (channels[i] == cSound)
				{
					cSound = CSound.createFromSound(cSound);
					break;
				}
			}
		}
		if (channel < 0)
		{
			int i;
			for (i = 0; i < 32 && (channels[i] != null || bLocked[i]); i++)
			{
			}
			if (i == 32)
			{
				for (i = 0; i < 32 && (bLocked[i] || channels[i] == null || channels[i].bUninterruptible); i++)
				{
				}
			}
			channel = i;
			if (channel >= 0 && channel < 32)
			{
				volumes[channel] = mainVolume;
			}
		}
		if (channel < 0 || channel >= 32)
		{
			return;
		}
		if (channels[channel] != null)
		{
			if (channels[channel].bUninterruptible)
			{
				return;
			}
			if (channels[channel] != cSound)
			{
				channels[channel].stop();
			}
		}
		channels[channel] = cSound;
		cSound.play(nLoops, bPrio, volumes[channel], pans[channel]);
	}

	public void setMultipleSounds(bool bMultiple)
	{
		bMultipleSounds = bMultiple;
	}

	public void keepCurrentSounds()
	{
		for (int i = 0; i < 32; i++)
		{
			if (channels[i] != null && channels[i].isPlaying())
			{
				app.soundBank.setToLoad(channels[i].handle);
			}
		}
	}

	public void setOnOff(bool bState)
	{
		if (bState != bOn)
		{
			bOn = bState;
			if (!bOn)
			{
				stopAllSounds();
			}
		}
	}

	public bool getOnOff()
	{
		return bOn;
	}

	public void stopAllSounds()
	{
		for (int i = 0; i < 32; i++)
		{
			if (channels[i] != null)
			{
				channels[i].stop();
				channels[i] = null;
			}
		}
	}

	public void stopSample(short handle)
	{
		for (int i = 0; i < 32; i++)
		{
			if (channels[i] != null && channels[i].handle == handle)
			{
				channels[i].stop();
				channels[i] = null;
			}
		}
	}

	public void stopChannel(int channel)
	{
		if (channel >= 0 && channel < 32 && channels[channel] != null)
		{
			channels[channel].stop();
			channels[channel] = null;
		}
	}

	public bool isSamplePaused(short handle)
	{
		for (int i = 0; i < 32; i++)
		{
			if (channels[i] != null && channels[i].handle == handle)
			{
				return channels[i].isPaused();
			}
		}
		return false;
	}

	public bool isSoundPlaying()
	{
		for (int i = 0; i < 32; i++)
		{
			if (channels[i] != null)
			{
				return channels[i].isPlaying();
			}
		}
		return false;
	}

	public bool isSamplePlaying(short handle)
	{
		for (int i = 0; i < 32; i++)
		{
			if (channels[i] != null && channels[i].handle == handle)
			{
				return channels[i].isPlaying();
			}
		}
		return false;
	}

	public bool isChannelPlaying(int channel)
	{
		if (channel >= 0 && channel < 32 && channels[channel] != null)
		{
			return channels[channel].isPlaying();
		}
		return false;
	}

	public bool isChannelPaused(int channel)
	{
		if (channel >= 0 && channel < 32 && channels[channel] != null)
		{
			return channels[channel].isPaused();
		}
		return false;
	}

	public void pause()
	{
		for (int i = 0; i < 32; i++)
		{
			if (channels[i] != null)
			{
				channels[i].pause();
			}
		}
	}

	public void pauseChannel(int channel)
	{
		if (channel >= 0 && channel < 32 && channels[channel] != null)
		{
			channels[channel].pause();
		}
	}

	public void pauseSample(short handle)
	{
		for (int i = 0; i < 32; i++)
		{
			if (channels[i] != null && channels[i].handle == handle)
			{
				channels[i].pause();
			}
		}
	}

	public void resume()
	{
		for (int i = 0; i < 32; i++)
		{
			if (channels[i] != null)
			{
				channels[i].resume();
			}
		}
	}

	public void resumeChannel(int channel)
	{
		if (channel >= 0 && channel < 32 && channels[channel] != null)
		{
			channels[channel].resume();
		}
	}

	public void resumeSample(short handle)
	{
		for (int i = 0; i < 32; i++)
		{
			if (channels[i] != null && channels[i].handle == handle)
			{
				channels[i].resume();
			}
		}
	}

	public void setVolumeChannel(int channel, int volume)
	{
		if (volume < 0)
		{
			volume = 0;
		}
		if (volume > 100)
		{
			volume = 100;
		}
		if (channel >= 0 && channel < 32)
		{
			volumes[channel] = volume;
			if (channels[channel] != null)
			{
				channels[channel].setVolume(volume);
			}
		}
	}

	public void setFrequencyChannel(int channel, int frequency)
	{
		if (frequency < 0)
		{
			frequency = 100;
		}
		if (channel >= 0 && channel < 32 && channels[channel] != null)
		{
			channels[channel].setFrequency(frequency);
		}
	}

	public int getVolumeChannel(int channel)
	{
		if (channel >= 0 && channel < 32 && channels[channel] != null)
		{
			return volumes[channel];
		}
		return 0;
	}

	public int getFrequencyChannel(int channel)
	{
		if (channel >= 0 && channel < 32 && channels[channel] != null)
		{
			return channels[channel].getFrequency();
		}
		return 0;
	}

	public void setVolumeSample(short handle, int volume)
	{
		if (volume < 0)
		{
			volume = 0;
		}
		if (volume > 100)
		{
			volume = 100;
		}
		for (int i = 0; i < 32; i++)
		{
			if (channels[i] != null && channels[i].handle == handle)
			{
				volumes[i] = volume;
				channels[i].setVolume(volume);
			}
		}
	}

	public void setFrequencySample(short handle, int frequency)
	{
		if (frequency < 0)
		{
			frequency = 100;
		}
		for (int i = 0; i < 32; i++)
		{
			if (channels[i] != null && channels[i].handle == handle)
			{
				channels[i].setFrequency(frequency);
			}
		}
	}

	public void setMainVolume(int volume)
	{
		if (volume < 0)
		{
			volume = 0;
		}
		if (volume > 100)
		{
			volume = 100;
		}
		mainVolume = volume;
		SoundEffect.MasterVolume = (float)((double)volume / 100.0);
	}

	public int getMainVolume()
	{
		return mainVolume;
	}

	public int getMainPan()
	{
		return mainPan;
	}

	public void setPanChannel(int channel, int pan)
	{
		if (pan < -100)
		{
			pan = -100;
		}
		if (pan > 100)
		{
			pan = 100;
		}
		if (channel >= 0 && channel < 32)
		{
			pans[channel] = pan;
			if (channels[channel] != null)
			{
				channels[channel].setPan(pan);
			}
		}
	}

	public int getPanChannel(int channel)
	{
		if (channel >= 0 && channel < 32 && channels[channel] != null)
		{
			return pans[channel];
		}
		return 0;
	}

	public void setPanSample(short handle, int pan)
	{
		if (pan < -100)
		{
			pan = -100;
		}
		if (pan > 100)
		{
			pan = 100;
		}
		for (int i = 0; i < 32; i++)
		{
			if (channels[i] != null && channels[i].handle == handle)
			{
				pans[i] = pan;
				channels[i].setPan(pan);
			}
		}
	}

	public void setMainPan(int pan)
	{
		if (pan < -100)
		{
			pan = -100;
		}
		if (pan > 100)
		{
			pan = 100;
		}
		mainPan = pan;
		for (int i = 0; i < 32; i++)
		{
			pans[i] = pan;
			if (channels[i] != null)
			{
				channels[i].setPan(pan);
			}
		}
	}

	private int getChannel(string name)
	{
		for (int i = 0; i < 32; i++)
		{
			if (channels[i] != null && string.Compare(channels[i].name, name) == 0)
			{
				return i;
			}
		}
		return -1;
	}

	public int getDurationChannel(int channel)
	{
		if (channel >= 0 && channel < 32 && channels[channel] != null)
		{
			return channels[channel].getDuration();
		}
		return 0;
	}

	public int getVolumeSample(string name)
	{
		int channel = getChannel(name);
		if (channel >= 0)
		{
			return volumes[channel];
		}
		return 0;
	}

	public int getDurationSample(string name)
	{
		int channel = getChannel(name);
		if (channel >= 0)
		{
			return channels[channel].getDuration();
		}
		return 0;
	}

	public int getPanSample(string name)
	{
		int channel = getChannel(name);
		if (channel >= 0)
		{
			return pans[channel];
		}
		return 0;
	}

	public int getFrequencySample(string name)
	{
		int channel = getChannel(name);
		if (channel >= 0)
		{
			return channels[channel].getFrequency();
		}
		return 0;
	}

	public void checkSounds()
	{
		for (int i = 0; i < 32; i++)
		{
			if (channels[i] != null && channels[i].checkSound())
			{
				channels[i] = null;
			}
		}
	}
}
