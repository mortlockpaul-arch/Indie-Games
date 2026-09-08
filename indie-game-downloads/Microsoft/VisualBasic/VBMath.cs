using System;
using Microsoft.VisualBasic.CompilerServices;

namespace Microsoft.VisualBasic;

[StandardModule]
public sealed class VBMath
{
	public static float Rnd()
	{
		return Rnd(1f);
	}

	public static float Rnd(float Number)
	{
		ProjectData projectData = ProjectData.GetProjectData();
		int num = projectData.m_rndSeed;
		checked
		{
			if ((double)Number != 0.0)
			{
				if ((double)Number < 0.0)
				{
					num = BitConverter.ToInt32(BitConverter.GetBytes(Number), 0);
					long num2 = num & 0xFFFFFFFFu;
					num = (int)((num2 + (num2 >> 24)) & 0xFFFFFF);
				}
				num = (int)((unchecked((long)num) * 1140671485L + 12820163) & 0xFFFFFF);
			}
			projectData.m_rndSeed = num;
			return (float)num / 16777216f;
		}
	}

	public static void Randomize()
	{
		ProjectData projectData = ProjectData.GetProjectData();
		float timer = GetTimer();
		int rndSeed = projectData.m_rndSeed;
		int num = BitConverter.ToInt32(BitConverter.GetBytes(timer), 0);
		num = ((num & 0xFFFF) ^ (num >> 16)) << 8;
		rndSeed = (rndSeed & -16776961) | num;
		projectData.m_rndSeed = rndSeed;
	}

	public static void Randomize(double Number)
	{
		ProjectData projectData = ProjectData.GetProjectData();
		int rndSeed = projectData.m_rndSeed;
		int num = ((!BitConverter.IsLittleEndian) ? BitConverter.ToInt32(BitConverter.GetBytes(Number), 0) : BitConverter.ToInt32(BitConverter.GetBytes(Number), 4));
		num = ((num & 0xFFFF) ^ (num >> 16)) << 8;
		rndSeed = (rndSeed & -16776961) | num;
		projectData.m_rndSeed = rndSeed;
	}

	private static float GetTimer()
	{
		DateTime now = DateTime.Now;
		return (float)((double)checked((60 * now.Hour + now.Minute) * 60 + now.Second) + (double)now.Millisecond / 1000.0);
	}
}
