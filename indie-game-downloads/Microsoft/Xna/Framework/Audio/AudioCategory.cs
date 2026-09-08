using System;

namespace Microsoft.Xna.Framework.Audio;

public struct AudioCategory : IEquatable<AudioCategory>
{
	internal AudioEngine parent;

	internal ushort index;

	internal string name;

	public string Name => name;

	public void Pause()
	{
		lock (parent.gcSync)
		{
			if (parent.IsDisposed)
			{
				throw new ArgumentException();
			}
			FAudio.FACTAudioEngine_Pause(parent.handle, index, 1);
		}
	}

	public void Resume()
	{
		lock (parent.gcSync)
		{
			if (parent.IsDisposed)
			{
				throw new ArgumentException();
			}
			FAudio.FACTAudioEngine_Pause(parent.handle, index, 0);
		}
	}

	public void SetVolume(float volume)
	{
		if (volume < 0f)
		{
			throw new ArgumentException("Volume must be a positive float value.");
		}
		if (volume > 16777216f)
		{
			throw new ArgumentException();
		}
		lock (parent.gcSync)
		{
			if (parent.IsDisposed)
			{
				throw new ArgumentException();
			}
			FAudio.FACTAudioEngine_SetVolume(parent.handle, index, volume);
		}
	}

	public void Stop(AudioStopOptions options)
	{
		if ((uint)options > 1u)
		{
			throw new ArgumentException();
		}
		lock (parent.gcSync)
		{
			if (parent.IsDisposed)
			{
				throw new ArgumentException();
			}
			FAudio.FACTAudioEngine_Stop(parent.handle, index, (uint)options);
		}
	}

	public override int GetHashCode()
	{
		int num = index;
		if (parent != null)
		{
			num ^= parent.GetHashCode();
		}
		return num;
	}

	public bool Equals(AudioCategory other)
	{
		return other.parent == parent && other.index == index;
	}

	public override bool Equals(object obj)
	{
		return obj is AudioCategory && Equals((AudioCategory)obj);
	}

	public static bool operator ==(AudioCategory value1, AudioCategory value2)
	{
		return value1.Equals(value2);
	}

	public static bool operator !=(AudioCategory value1, AudioCategory value2)
	{
		return !value1.Equals(value2);
	}

	public override string ToString()
	{
		return name ?? string.Empty;
	}
}
