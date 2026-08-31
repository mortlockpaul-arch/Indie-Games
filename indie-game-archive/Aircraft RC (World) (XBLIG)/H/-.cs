using System;
using F;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using SynapseGaming.LightingSystem.Audio;

namespace H;

internal class _0002 : IDisposable
{
	internal bool HCB;

	internal bool HC_0002;

	internal AudioSource HC_0012;

	internal bool HCH;

	internal Vector3 HC7;

	internal AudioState HC_0001;

	internal SoundEffectInstance HCw;

	internal SoundState HCZ;

	private static AudioEmitter HC_000F;

	static _0002()
	{
		HC_000F = new AudioEmitter();
		HC_000F.DopplerScale = 1f;
		HC_000F.Forward = Vector3.Forward;
		HC_000F.Up = Vector3.Up;
		HC_000F.Velocity = Vector3.Zero;
	}

	internal void f(AudioSource P_0)
	{
		G();
		HC_0012 = P_0;
		HCH = HC_0012.Loop;
		HC7 = HC_0012.Position;
		HC_0001 = AudioState.Stopped;
		HCw = P_0.SoundEffect.CreateInstance();
		HCw.IsLooped = HC_0012.Loop;
		HCZ = HCw.State;
	}

	private void G()
	{
		HC_0012 = null;
		global::F.B._7_0004(ref HCw);
	}

	public void Dispose()
	{
		G();
	}

	internal void F(AudioListener P_0)
	{
		if (HC_0012 == null)
		{
			HCB = false;
			return;
		}
		bool loop = HC_0012.Loop;
		if (loop != HCH)
		{
			f(HC_0012);
			loop = HC_0012.Loop;
		}
		AudioState audioState = HC_0012.AudioState;
		Vector3 position = HC_0012.Position;
		SoundState state = HCw.State;
		bool flag = audioState == AudioState.Playing;
		bool flag2 = state == SoundState.Playing;
		if (audioState != HC_0001)
		{
			if (flag && !flag2)
			{
				HCw.Apply3D(P_0, HC_000F);
				HCw.Play();
			}
			else if (!flag && state != SoundState.Stopped)
			{
				HCw.Stop();
			}
			state = HCw.State;
		}
		else if (state != HCZ)
		{
			audioState = (flag2 ? AudioState.Playing : AudioState.Stopped);
			HC_0012.AudioState = audioState;
		}
		if (HC_0012.AudioType == AudioType.Point)
		{
			HC_000F.Velocity = position - HC7;
			HC_000F.Position = position;
			SoundEffect.DistanceScale = HC_0012.Radius * 0.1f;
		}
		else
		{
			HC_000F.Velocity = Vector3.Zero;
			HC_000F.Position = P_0.Position;
			SoundEffect.DistanceScale = 1f;
		}
		HCw.Volume = MathHelper.Clamp(HC_0012.Volume, 0f, 1f);
		HCw.Apply3D(P_0, HC_000F);
		HCH = loop;
		HC_0001 = audioState;
		HC7 = position;
		HCZ = state;
		HCB = false;
	}
}
