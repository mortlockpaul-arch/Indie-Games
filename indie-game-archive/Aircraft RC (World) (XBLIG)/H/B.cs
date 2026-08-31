using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using SynapseGaming.LightingSystem.Audio;

namespace H;

internal class B
{
	internal int HCB = 200;

	private Vector3 HC_0002;

	private AudioListener HC_0012 = new AudioListener();

	private Dictionary<AudioSource, _0002> HCH = new Dictionary<AudioSource, _0002>();

	private List<AudioSource> HC7 = new List<AudioSource>();

	private List<KeyValuePair<AudioSource, _0002>> HC_0001 = new List<KeyValuePair<AudioSource, _0002>>();

	internal void u()
	{
		foreach (KeyValuePair<AudioSource, _0002> item in HCH)
		{
			item.Value.Dispose();
		}
		HCH.Clear();
		HC7.Clear();
	}

	internal void q(ref Matrix P_0)
	{
		Vector3 translation = P_0.Translation;
		HC_0012.Forward = P_0.Forward;
		HC_0012.Up = P_0.Up;
		HC_0012.Position = translation;
		HC_0012.Velocity = translation - HC_0002;
		HC_0002 = translation;
	}

	internal void R(AudioSource P_0)
	{
		if (HCH.TryGetValue(P_0, out var value))
		{
			if (value.HC_0012 != P_0)
			{
				value.f(P_0);
			}
			value.HCB = true;
			value.HC_0002 = false;
		}
		else
		{
			HC7.Add(P_0);
		}
	}

	internal void N(AudioSource P_0)
	{
		if (HCH.TryGetValue(P_0, out var value))
		{
			value.HC_0002 = true;
		}
	}

	internal void F()
	{
		foreach (KeyValuePair<AudioSource, _0002> item in HCH)
		{
			_0002 value = item.Value;
			if (value.HC_0002)
			{
				value.Dispose();
			}
		}
		int num = HC7.Count + HCH.Count;
		if (num > HCB)
		{
			int num2 = num - HCB;
			HC_0001.Clear();
			foreach (KeyValuePair<AudioSource, _0002> item2 in HCH)
			{
				if (!item2.Value.HCB)
				{
					HC_0001.Add(item2);
				}
			}
			if (num2 > HC_0001.Count)
			{
				throw new Exception("Unable to clear enough unused audio threads to work within maximum thread count.");
			}
			foreach (KeyValuePair<AudioSource, _0002> item3 in HC_0001)
			{
				if (num2 > 0)
				{
					HCH.Remove(item3.Key);
					_0002 value2 = item3.Value;
					int index = HC7.Count - 1;
					AudioSource audioSource = HC7[index];
					HC7.RemoveAt(index);
					value2.f(audioSource);
					HCH.Add(audioSource, value2);
					num2--;
					continue;
				}
				break;
			}
		}
		foreach (AudioSource item4 in HC7)
		{
			_0002 obj = new _0002();
			obj.f(item4);
			HCH.Add(item4, obj);
		}
		HC7.Clear();
		foreach (KeyValuePair<AudioSource, _0002> item5 in HCH)
		{
			item5.Value.F(HC_0012);
		}
	}
}
