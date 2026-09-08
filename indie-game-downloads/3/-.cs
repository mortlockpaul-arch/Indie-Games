using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using SynapseGaming.LightingSystem.Audio;

namespace _3;

internal class _0018
{
	internal int _3A_0018 = 200;

	private Vector3 _3AL;

	private AudioListener _3A_0019 = new AudioListener();

	private Dictionary<AudioSource, L> _3A3 = new Dictionary<AudioSource, L>();

	private List<AudioSource> _3A6 = new List<AudioSource>();

	private List<KeyValuePair<AudioSource, L>> _3AD = new List<KeyValuePair<AudioSource, L>>();

	internal void X()
	{
		foreach (KeyValuePair<AudioSource, L> item in _3A3)
		{
			item.Value.Dispose();
		}
		_3A3.Clear();
		_3A6.Clear();
	}

	internal void _0010(ref Matrix P_0)
	{
		Vector3 translation = P_0.Translation;
		_3A_0019.Forward = P_0.Forward;
		_3A_0019.Up = P_0.Up;
		_3A_0019.Position = translation;
		_3A_0019.Velocity = translation - _3AL;
		_3AL = translation;
	}

	internal void _0016(AudioSource P_0)
	{
		if (_3A3.TryGetValue(P_0, out var value))
		{
			if (value._3A_0019 != P_0)
			{
				value.u(P_0);
			}
			value._3A_0018 = true;
			value._3AL = false;
		}
		else
		{
			_3A6.Add(P_0);
		}
	}

	internal void k(AudioSource P_0)
	{
		if (_3A3.TryGetValue(P_0, out var value))
		{
			value._3AL = true;
		}
	}

	internal void p()
	{
		foreach (KeyValuePair<AudioSource, L> item in _3A3)
		{
			L value = item.Value;
			if (value._3AL)
			{
				value.Dispose();
			}
		}
		int num = _3A6.Count + _3A3.Count;
		if (num > _3A_0018)
		{
			int num2 = num - _3A_0018;
			_3AD.Clear();
			foreach (KeyValuePair<AudioSource, L> item2 in _3A3)
			{
				if (!item2.Value._3A_0018)
				{
					_3AD.Add(item2);
				}
			}
			if (num2 > _3AD.Count)
			{
				throw new Exception("Unable to clear enough unused audio threads to work within maximum thread count.");
			}
			foreach (KeyValuePair<AudioSource, L> item3 in _3AD)
			{
				if (num2 > 0)
				{
					_3A3.Remove(item3.Key);
					L value2 = item3.Value;
					int index = _3A6.Count - 1;
					AudioSource audioSource = _3A6[index];
					_3A6.RemoveAt(index);
					value2.u(audioSource);
					_3A3.Add(audioSource, value2);
					num2--;
					continue;
				}
				break;
			}
		}
		foreach (AudioSource item4 in _3A6)
		{
			L l = new L();
			l.u(item4);
			_3A3.Add(item4, l);
		}
		_3A6.Clear();
		foreach (KeyValuePair<AudioSource, L> item5 in _3A3)
		{
			item5.Value.p(_3A_0019, item5.Key.DopplerScale);
		}
	}
}
