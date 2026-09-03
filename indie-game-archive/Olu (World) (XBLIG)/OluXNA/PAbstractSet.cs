using System;
using System.Collections.Generic;
using System.Xml;
using Microsoft.Xna.Framework;

namespace OluXNA;

internal class PAbstractSet
{
	private double stepTime;

	private double sumTime;

	private double progress;

	private double totalTime;

	private int curBeat;

	public List<PAbstract> pSet;

	public PAbstractSet()
	{
		stepTime = BaseGame.BEAT;
		pSet = new List<PAbstract>();
		totalTime = 0.0;
		sumTime = 0.0;
	}

	public PAbstractSet(Dictionary<string, string> attributes, XmlNode node)
		: this()
	{
		foreach (XmlNode childNode in node.ChildNodes)
		{
			if (childNode.Name != "#comment")
			{
				pSet.Add((PAbstract)LevelLoader.MakeObj(childNode));
			}
		}
	}

	public void Update(GameTime gametime)
	{
		if (totalTime == 0.0 && pSet.Count > 1)
		{
			totalTime = (double)(pSet[1].beat - pSet[0].beat) * (double)BaseGame.BEAT;
		}
		if (!BaseGame.Get().movingToNextZone || pSet.Count <= 1)
		{
			return;
		}
		stepTime += gametime.ElapsedGameTime.TotalSeconds;
		sumTime += gametime.ElapsedGameTime.TotalSeconds;
		if (stepTime >= (double)BaseGame.BEAT)
		{
			stepTime -= BaseGame.BEAT;
			curBeat++;
			if (curBeat >= pSet[1].beat)
			{
				pSet.RemoveAt(0);
				if (pSet.Count > 1)
				{
					sumTime -= totalTime;
					if (sumTime < 0.0)
					{
						sumTime = 0.0;
					}
					totalTime = (double)(pSet[1].beat - pSet[0].beat) * (double)BaseGame.BEAT;
				}
			}
		}
		progress = Math.Min(1.0, sumTime / totalTime);
	}

	public void GetMatrix(ref Vector3 pos, ref Vector3 dir, ref Vector3 up)
	{
		if (pSet.Count > 0)
		{
			GetDifference(ref pos, ref dir, ref up);
		}
	}

	public void GetDifference(ref Vector3 pos, ref Vector3 dir, ref Vector3 up)
	{
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		if (pSet.Count > 1)
		{
			pos = Vector3.Lerp(pSet[0].pos, pSet[1].pos, (float)progress);
			dir = Vector3.Lerp(pSet[0].dir, pSet[1].dir, (float)progress);
			up = Vector3.Lerp(pSet[0].up, pSet[1].up, (float)progress);
		}
		else
		{
			pos = pSet[0].pos;
			dir = pSet[0].dir;
			up = pSet[0].up;
		}
		dir = Vector3.Normalize(dir);
		up = Vector3.Normalize(up);
	}
}
