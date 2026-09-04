using System;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using RuntimeXNA.Application;

namespace RuntimeXNA.Banks;

public class CSound
{
	public short handle;

	public SoundEffect sound;

	public SoundEffectInstance soundInstance;

	public int useCount;

	public bool bUninterruptible;

	public int nLoops;

	public int numSound;

	public string name;

	public int type;

	public Song song;

	public bool bSongPlaying;

	public bool bPaused;

	public long timer;

	public int frequency;

	public CRunApp application;

	public CSound(CRunApp app)
	{
		application = app;
	}

	public void loadHandle()
	{
		handle = application.file.readAShort();
		application.file.skipBytes(5);
		int num = application.file.readAShort();
		if (!application.file.bUnicode)
		{
			application.file.skipBytes(num);
		}
		else
		{
			application.file.skipBytes(num * 2);
		}
	}

	public static CSound createFromSound(CSound source)
	{
		CSound cSound = new CSound(source.application);
		cSound.handle = source.handle;
		cSound.sound = source.sound;
		cSound.name = source.name;
		cSound.type = source.type;
		cSound.song = source.song;
		return cSound;
	}

	public void load()
	{
		handle = application.file.readAShort();
		type = application.file.readAByte();
		frequency = application.file.readAInt();
		int size = application.file.readAShort();
		name = application.file.readAString(size);
		string text = handle.ToString("D4");
		text = "Snd" + text;
		if (type == 0)
		{
			sound = application.content.Load<SoundEffect>(text);
			LoadUpfront.BuildLoadInfo_SoundEffect(text);
		}
		else
		{
			song = application.content.Load<Song>(text);
			LoadUpfront.BuildLoadInfo_Song(text);
		}
	}

	public void play(int nl, bool bPrio, float v, float p)
	{
		nLoops = nl;
		if (nLoops == 0)
		{
			nLoops = 10000000;
		}
		if (type == 0)
		{
			if (soundInstance != null)
			{
				soundInstance.Stop();
				soundInstance.Dispose();
				soundInstance = null;
			}
			if (soundInstance == null)
			{
				soundInstance = sound.CreateInstance();
			}
			if (soundInstance != null)
			{
				soundInstance.Volume = (float)((double)v / 100.0);
				soundInstance.Pan = (float)((double)p / 100.0);
				soundInstance.Play();
				bUninterruptible = bPrio;
			}
		}
		else if (MediaPlayer.GameHasControl)
		{
			MediaPlayer.Stop();
			MediaPlayer.Play(song);
			bSongPlaying = true;
			bPaused = false;
			timer = application.timer + getDuration();
		}
	}

	public void stop()
	{
		if (type == 0)
		{
			if (soundInstance != null)
			{
				soundInstance.Stop();
				soundInstance.Dispose();
				soundInstance = null;
				bUninterruptible = false;
			}
		}
		else if (MediaPlayer.GameHasControl)
		{
			MediaPlayer.Stop();
			bSongPlaying = false;
			bUninterruptible = false;
		}
	}

	public void setVolume(int v)
	{
		if (type == 0)
		{
			if (soundInstance != null)
			{
				soundInstance.Volume = (float)((double)v / 100.0);
			}
		}
		else if (MediaPlayer.GameHasControl)
		{
			MediaPlayer.Volume = (float)((double)v / 100.0);
		}
	}

	public void setPan(int p)
	{
		if (type == 0 && soundInstance != null)
		{
			soundInstance.Pan = (float)((double)p / 100.0);
		}
	}

	public void setFrequency(int newFrequency)
	{
		double num = (double)newFrequency / (double)frequency;
		num = ((!(num >= 1.0)) ? (num * 2.0 - 2.0) : (num - 1.0));
		num = Math.Max(Math.Min(num, 1.0), -1.0);
		if (soundInstance != null)
		{
			soundInstance.Pitch = (float)num;
		}
	}

	public int getFrequency()
	{
		return frequency;
	}

	public void pause()
	{
		if (type == 0)
		{
			if (soundInstance != null)
			{
				soundInstance.Pause();
			}
		}
		else if (MediaPlayer.GameHasControl)
		{
			MediaPlayer.Pause();
			bPaused = true;
		}
	}

	public void resume()
	{
		if (type == 0)
		{
			if (soundInstance != null)
			{
				soundInstance.Resume();
			}
		}
		else if (MediaPlayer.GameHasControl)
		{
			MediaPlayer.Resume();
			bPaused = false;
		}
	}

	public bool isPaused()
	{
		if (type == 0)
		{
			if (soundInstance != null && soundInstance.State == SoundState.Paused)
			{
				return true;
			}
			return false;
		}
		return bPaused;
	}

	public bool isPlaying()
	{
		if (type == 0)
		{
			if (soundInstance != null && soundInstance.State == SoundState.Playing)
			{
				return true;
			}
		}
		else if (MediaPlayer.GameHasControl && MediaPlayer.State == MediaState.Playing)
		{
			return true;
		}
		return false;
	}

	public int getDuration()
	{
		TimeSpan timeSpan = ((type != 0) ? song.Duration : sound.Duration);
		return timeSpan.Hours * 60 * 60 * 1000 + timeSpan.Minutes * 60 * 1000 + timeSpan.Seconds * 1000 + timeSpan.Milliseconds;
	}

	public bool checkSound()
	{
		if (type == 0)
		{
			if (soundInstance != null && soundInstance.State == SoundState.Stopped)
			{
				if (nLoops > 0)
				{
					nLoops--;
					if (nLoops > 0)
					{
						soundInstance.Play();
						return false;
					}
				}
				bUninterruptible = false;
				soundInstance.Dispose();
				soundInstance = null;
				return true;
			}
		}
		else if (bSongPlaying && application.timer >= timer && MediaPlayer.State != MediaState.Playing && !bPaused)
		{
			if (nLoops > 0)
			{
				nLoops--;
				if (nLoops > 0)
				{
					MediaPlayer.Play(song);
					timer = application.timer + getDuration();
					return false;
				}
			}
			bUninterruptible = false;
			return true;
		}
		return false;
	}
}
