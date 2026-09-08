using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using SynapseGaming.LightingSystem.Audio;
using p;

namespace _3;

internal class L : IDisposable
{
	internal bool _3A_0018;

	internal bool _3AL;

	internal AudioSource _3A_0019;

	internal bool _3A3;

	internal Vector3 _3A6;

	internal AudioState _3AD;

	internal SoundEffectInstance _3A_0017;

	internal SoundState _3A_0003;

	private static AudioEmitter _3Al;

	static L()
	{
		_3Al = new AudioEmitter();
		_3Al.DopplerScale = 1f;
		_3Al.Forward = Vector3.Forward;
		_3Al.Up = Vector3.Up;
		_3Al.Velocity = Vector3.Zero;
	}

	internal void u(AudioSource P_0)
	{
		U();
		_3A_0019 = P_0;
		_3A3 = _3A_0019.Loop;
		_3A6 = _3A_0019.Position;
		_3AD = AudioState.Stopped;
		_3A_0017 = P_0.SoundEffect.CreateInstance();
		_3A_0017.IsLooped = _3A_0019.Loop;
		_3A_0003 = _3A_0017.State;
	}

	private void U()
	{
		_3A_0019 = null;
		global::p._0018._6_0006(ref _3A_0017);
	}

	public void Dispose()
	{
		U();
	}

	internal void p(AudioListener P_0, float P_1)
	{
		if (_3A_0019 == null)
		{
			_3A_0018 = false;
			return;
		}
		bool loop = _3A_0019.Loop;
		if (loop != _3A3)
		{
			u(_3A_0019);
			loop = _3A_0019.Loop;
		}
		AudioState audioState = _3A_0019.AudioState;
		Vector3 position = _3A_0019.Position;
		SoundState state = _3A_0017.State;
		bool flag = audioState == AudioState.Playing;
		bool flag2 = state == SoundState.Playing;
		if (audioState != _3AD)
		{
			if (flag && !flag2)
			{
				_3A_0017.Apply3D(P_0, _3Al);
				_3A_0017.Play();
			}
			else if (!flag && state != SoundState.Stopped)
			{
				_3A_0017.Stop();
			}
			state = _3A_0017.State;
		}
		else if (state != _3A_0003)
		{
			audioState = (flag2 ? AudioState.Playing : AudioState.Stopped);
			_3A_0019.AudioState = audioState;
		}
		if (_3A_0019.AudioType == AudioType.Point)
		{
			_3Al.Velocity = position - _3A6;
			_3Al.Position = position;
			_3Al.DopplerScale = P_1;
			SoundEffect.DistanceScale = _3A_0019.Radius * 0.1f;
		}
		else
		{
			_3Al.Velocity = Vector3.Zero;
			_3Al.Position = P_0.Position;
			_3Al.DopplerScale = 1f;
			SoundEffect.DistanceScale = 1f;
		}
		_3A_0017.Volume = MathHelper.Clamp(_3A_0019.Volume, 0f, 1f);
		_3A_0017.Apply3D(P_0, _3Al);
		_3A3 = loop;
		_3AD = audioState;
		_3A6 = position;
		_3A_0003 = state;
		_3A_0018 = false;
	}
}
