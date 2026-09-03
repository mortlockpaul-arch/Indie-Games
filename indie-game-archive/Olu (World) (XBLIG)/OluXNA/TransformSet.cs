using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace OluXNA;

internal class TransformSet
{
	private double stepTime;

	public List<Transform> tSet;

	public bool usePath;

	private Matrix curTransform;

	private Matrix curScaleTransform;

	private Matrix curAllTransform;

	public TransformSet()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		base._002Ector();
		stepTime = BaseGame.BEAT;
		tSet = new List<Transform>();
		curTransform = Matrix.Identity;
		curScaleTransform = Matrix.Identity;
		curAllTransform = Matrix.Identity;
	}

	public void Update(GameTime gametime)
	{
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		bool flag = false;
		if (BaseGame.Get().movingToNextZone)
		{
			stepTime += gametime.ElapsedGameTime.TotalMilliseconds / 1000.0;
			if (stepTime >= (double)BaseGame.BEAT)
			{
				stepTime -= BaseGame.BEAT;
			}
			int num = tSet.Count;
			for (int i = 0; i < num; i++)
			{
				bool flag2 = tSet[i].Update(gametime.ElapsedGameTime.TotalMilliseconds / 1000.0);
				flag |= flag2;
				if (flag && !flag2)
				{
					break;
				}
				if (!flag)
				{
					curTransform = tSet[i].GetMatrix(1f);
					curScaleTransform = tSet[i].GetScaleMatrix(1f);
					tSet.RemoveAt(i);
					i--;
					num--;
				}
			}
		}
		if (tSet.Count > 0)
		{
			curTransform = tSet[0].GetMatrix();
			curScaleTransform = tSet[0].GetScaleMatrix();
			curAllTransform = tSet[0].GetAllMatrix();
		}
	}

	public Matrix GetAllMatrix()
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		if (tSet.Count > 0)
		{
			curAllTransform = tSet[0].GetAllMatrix();
		}
		return curAllTransform;
	}

	public Matrix GetAllMatrix(float progress)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		if (tSet.Count > 0)
		{
			curAllTransform = tSet[0].GetAllMatrix(progress);
		}
		return curAllTransform;
	}

	public Matrix GetMatrix()
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		if (tSet.Count > 0)
		{
			curTransform = tSet[0].GetMatrix();
		}
		return curTransform;
	}

	public Matrix GetMatrix(float progress)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		if (tSet.Count > 0)
		{
			curTransform = tSet[0].GetMatrix(progress);
		}
		return curTransform;
	}

	public Matrix GetScaleMatrix()
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		if (tSet.Count > 0)
		{
			curScaleTransform = tSet[0].GetScaleMatrix();
		}
		return curScaleTransform;
	}

	public Matrix GetScaleMatrix(float progress)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		if (tSet.Count > 0)
		{
			curScaleTransform = tSet[0].GetScaleMatrix(progress);
		}
		return curScaleTransform;
	}
}
