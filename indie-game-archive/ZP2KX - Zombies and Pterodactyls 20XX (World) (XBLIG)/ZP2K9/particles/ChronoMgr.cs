using Microsoft.Xna.Framework;

namespace ZP2K9.particles;

public class ChronoMgr
{
	private struct ChronoList
	{
		public Vector2[] chronosVec;

		public int chronos;

		public void AddChrono(Vector2 loc)
		{
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			if (chronos < chronosVec.Length)
			{
				chronosVec[chronos] = loc;
				chronos++;
			}
		}

		public void ResetChronos()
		{
			chronos = 0;
		}

		public bool GetChronod(Vector2 loc)
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			if (chronos <= 0)
			{
				return false;
			}
			for (int i = 0; i < chronos; i++)
			{
				Vector2 val = loc - chronosVec[i];
				if (((Vector2)(ref val)).LengthSquared() < 90000f)
				{
					return true;
				}
			}
			return false;
		}
	}

	private ChronoList[] chronos;

	private int curDic;

	public ChronoMgr()
	{
		chronos = new ChronoList[2];
		for (int i = 0; i < chronos.Length; i++)
		{
			chronos[i].chronosVec = (Vector2[])(object)new Vector2[10];
		}
	}

	internal void AddChrono(Vector2 vector2)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		chronos[1 - curDic].AddChrono(vector2);
	}

	internal void ResetChronos()
	{
		chronos[curDic].ResetChronos();
		curDic = 1 - curDic;
	}

	internal bool GetChronod(Vector2 loc)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		return chronos[curDic].GetChronod(loc);
	}
}
