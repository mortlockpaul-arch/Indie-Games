using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Z;

namespace u;

internal class K
{
	internal const int HCB = 1024;

	internal const int HC_0002 = 4;

	internal const int HC_0012 = 6;

	private Vector2[] HCH = new Vector2[4]
	{
		new Vector2(0f, 0f),
		new Vector2(1f, 0f),
		new Vector2(0f, 1f),
		new Vector2(1f, 1f)
	};

	private Effect HC7;

	private GraphicsDevice HC_0001;

	private int HCw;

	private D[] HCZ = new D[4096];

	private _0011 HC_000F;

	private List<_0011> HCy = new List<_0011>(32);

	private global::Z._6<_0011> HC6;

	private static ushort[] HCD;

	internal Effect Effect => HC7;

	internal List<_0011> Buffers => HCy;

	static K()
	{
		HCD = new ushort[6144];
		int num = 0;
		for (int i = 0; i < 1024; i++)
		{
			int num2 = i * 4;
			HCD[num++] = (ushort)num2;
			HCD[num++] = (ushort)(num2 + 1);
			HCD[num++] = (ushort)(num2 + 2);
			HCD[num++] = (ushort)(num2 + 2);
			HCD[num++] = (ushort)(num2 + 1);
			HCD[num++] = (ushort)(num2 + 3);
		}
	}

	internal K(GraphicsDevice P_0, global::Z._6<_0011> P_1, Effect P_2)
	{
		HC_0001 = P_0;
		HC6 = P_1;
		HC7 = P_2;
		for (int i = 0; i < HCZ.Length; i++)
		{
			HCZ[i].Normal = new Vector3(0f, 0f, -1f);
		}
	}

	internal unsafe void _5(ref Vector2 P_0, ref Vector2 P_1, float P_2, ref Vector2 P_3, ref Vector2 P_4, ref Vector2 P_5, float P_6)
	{
		if (HC_000F == null || HCw >= 1024)
		{
			_7u();
			_7N();
		}
		int num = HCw * 4;
		if (num + 4 > HCZ.Length)
		{
			throw new Exception("Unable to build sprite, vertex array to small for all vertices.");
		}
		bool flag = P_2 != 0f;
		float num2;
		float num3;
		if (flag)
		{
			num2 = (float)Math.Sin(P_2);
			num3 = (float)Math.Cos(P_2);
		}
		else
		{
			num2 = 0f;
			num3 = 1f;
		}
		fixed (D* ptr = &HCZ[num])
		{
			fixed (Vector2* hCH = HCH)
			{
				D* ptr2 = ptr;
				Vector2* ptr3 = hCH;
				float num4 = 0.5f - P_3.X;
				float num5 = 0.5f - P_3.Y;
				for (int i = 0; i < 4; i++)
				{
					ptr2->TextureCoordinate.X = ptr3->X * P_4.X + P_5.X;
					ptr2->TextureCoordinate.Y = ptr3->Y * P_4.Y + P_5.Y;
					float num6 = (ptr3->X - num4) * P_0.X;
					float num7 = (ptr3->Y - num5) * P_0.Y;
					if (flag)
					{
						float num8 = num6 * num3 - num7 * num2;
						num7 = num6 * num2 + num7 * num3;
						num6 = num8;
					}
					ptr2->Position.X = num6 + P_1.X;
					ptr2->Position.Y = num7 + P_1.Y;
					ptr2->Position.Z = P_6;
					ptr2->Binormal.X = 0f - num2;
					ptr2->Binormal.Y = num3;
					ptr2->Tangent.X = num3;
					ptr2->Tangent.Y = num2;
					ptr2++;
					ptr3++;
				}
			}
		}
		HCw++;
	}

	internal void _7u()
	{
		if (HC_000F != null && HCw >= 1)
		{
			HC_000F._7u(HC_0001, HCZ, HCD, HCw);
		}
	}

	private void _7N()
	{
		HC_000F = HC6.New();
		HCy.Add(HC_000F);
		HCw = 0;
	}

	internal void G()
	{
		foreach (_0011 item in HCy)
		{
			HC6.Free(item);
		}
		HC_000F = null;
		HCy.Clear();
		HCw = 0;
	}
}
