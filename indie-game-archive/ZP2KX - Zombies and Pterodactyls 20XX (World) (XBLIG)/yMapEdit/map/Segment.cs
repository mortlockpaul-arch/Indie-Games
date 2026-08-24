using System;
using Microsoft.Xna.Framework;
using yMapEdit.segdef;

namespace yMapEdit.map;

public class Segment
{
	public Vector2 loc;

	public float rotation;

	public int idx;

	public Rectangle rect;

	public Segment()
	{
		idx = -1;
	}

	public void CalculateRect(SegDef segDef)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c5: Unknown result type (might be due to invalid IL or missing references)
		Vector2 val = loc;
		Vector2 val2 = default(Vector2);
		((Vector2)(ref val2))._002Ector((float)Math.Cos(rotation) * ((float)((Rectangle)(ref segDef.sRect)).Right - segDef.origLoc.X), (float)Math.Sin(rotation) * ((float)((Rectangle)(ref segDef.sRect)).Right - segDef.origLoc.X));
		Vector2 val3 = default(Vector2);
		((Vector2)(ref val3))._002Ector((float)Math.Cos(rotation + 1.57f) * ((float)((Rectangle)(ref segDef.sRect)).Bottom - segDef.origLoc.Y), (float)Math.Sin(rotation + 1.57f) * ((float)((Rectangle)(ref segDef.sRect)).Bottom - segDef.origLoc.Y));
		Vector2 val4 = default(Vector2);
		((Vector2)(ref val4))._002Ector((float)Math.Cos(rotation) * (segDef.origLoc.X - (float)segDef.sRect.X), (float)Math.Sin(rotation) * (segDef.origLoc.X - (float)segDef.sRect.X));
		Vector2 val5 = default(Vector2);
		((Vector2)(ref val5))._002Ector((float)Math.Cos(rotation + 1.57f) * (segDef.origLoc.Y - (float)segDef.sRect.Y), (float)Math.Sin(rotation + 1.57f) * (segDef.origLoc.Y - (float)segDef.sRect.Y));
		Vector2 val6 = val - val4 - val5;
		Vector2 val7 = val + val2 - val5;
		Vector2 val8 = val - val4 + val3;
		Vector2 val9 = val + val2 + val3;
		Vector2 val10 = val;
		Vector2 val11 = val;
		if (val7.X < val10.X)
		{
			val10.X = val7.X;
		}
		if (val7.Y < val10.Y)
		{
			val10.Y = val7.Y;
		}
		if (val8.X < val10.X)
		{
			val10.X = val8.X;
		}
		if (val8.Y < val10.Y)
		{
			val10.Y = val8.Y;
		}
		if (val9.X < val10.X)
		{
			val10.X = val9.X;
		}
		if (val9.Y < val10.Y)
		{
			val10.Y = val9.Y;
		}
		if (val6.X < val10.X)
		{
			val10.X = val6.X;
		}
		if (val6.Y < val10.Y)
		{
			val10.Y = val6.Y;
		}
		if (val7.X > val11.X)
		{
			val11.X = val7.X;
		}
		if (val7.Y > val11.Y)
		{
			val11.Y = val7.Y;
		}
		if (val8.X > val11.X)
		{
			val11.X = val8.X;
		}
		if (val8.Y > val11.Y)
		{
			val11.Y = val8.Y;
		}
		if (val9.X > val11.X)
		{
			val11.X = val9.X;
		}
		if (val9.Y > val11.Y)
		{
			val11.Y = val9.Y;
		}
		if (val6.X > val11.X)
		{
			val11.X = val6.X;
		}
		if (val6.Y > val11.Y)
		{
			val11.Y = val6.Y;
		}
		rect = new Rectangle((int)val10.X, (int)val10.Y, (int)(val11.X - val10.X), (int)(val11.Y - val10.Y));
	}
}
