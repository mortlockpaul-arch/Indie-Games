using System;
using System.Collections.Generic;
using _0003;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace X;

internal class I
{
	internal const int _3A_0018 = 1024;

	internal const int _3AL = 4;

	internal const int _3A_0019 = 6;

	private Vector2[] _3A3 = new Vector2[4]
	{
		new Vector2(0f, 0f),
		new Vector2(1f, 0f),
		new Vector2(0f, 1f),
		new Vector2(1f, 1f)
	};

	private Effect _3A6;

	private GraphicsDevice _3AD;

	private int _3A_0017;

	private c[] _3A_0003 = new c[4096];

	private g _3Al;

	private List<g> _3At = new List<g>(32);

	private global::_0003.F<g> _3AF;

	private static ushort[] _3Ac;

	internal Effect Effect => _3A6;

	internal List<g> Buffers => _3At;

	static I()
	{
		_3Ac = new ushort[6144];
		int num = 0;
		for (int i = 0; i < 1024; i++)
		{
			int num2 = i * 4;
			_3Ac[num++] = (ushort)num2;
			_3Ac[num++] = (ushort)(num2 + 1);
			_3Ac[num++] = (ushort)(num2 + 2);
			_3Ac[num++] = (ushort)(num2 + 2);
			_3Ac[num++] = (ushort)(num2 + 1);
			_3Ac[num++] = (ushort)(num2 + 3);
		}
	}

	internal I(GraphicsDevice P_0, global::_0003.F<g> P_1, Effect P_2)
	{
		_3AD = P_0;
		_3AF = P_1;
		_3A6 = P_2;
		for (int i = 0; i < _3A_0003.Length; i++)
		{
			_3A_0003[i].Normal = new Vector3(0f, 0f, -1f);
		}
	}

	internal unsafe void V(ref Vector2 P_0, ref Vector2 P_1, float P_2, ref Vector2 P_3, ref Vector2 P_4, ref Vector2 P_5, float P_6)
	{
		if (_3Al == null || _3A_0017 >= 1024)
		{
			_67();
			_6_0016();
		}
		int num = _3A_0017 * 4;
		if (num + 4 > _3A_0003.Length)
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
		fixed (c* ptr = &_3A_0003[num])
		{
			fixed (Vector2* ptr2 = _3A3)
			{
				c* ptr3 = ptr;
				Vector2* ptr4 = ptr2;
				float num4 = 0.5f - P_3.X;
				float num5 = 0.5f - P_3.Y;
				for (int i = 0; i < 4; i++)
				{
					ptr3->TextureCoordinate.X = ptr4->X * P_4.X + P_5.X;
					ptr3->TextureCoordinate.Y = ptr4->Y * P_4.Y + P_5.Y;
					float num6 = (ptr4->X - num4) * P_0.X;
					float num7 = (ptr4->Y - num5) * P_0.Y;
					if (flag)
					{
						float num8 = num6 * num3 - num7 * num2;
						num7 = num6 * num2 + num7 * num3;
						num6 = num8;
					}
					ptr3->Position.X = num6 + P_1.X;
					ptr3->Position.Y = num7 + P_1.Y;
					ptr3->Position.Z = P_6;
					ptr3->Binormal.X = 0f - num2;
					ptr3->Binormal.Y = num3;
					ptr3->Tangent.X = num3;
					ptr3->Tangent.Y = num2;
					ptr3++;
					ptr4++;
				}
			}
		}
		_3A_0017++;
	}

	internal void _67()
	{
		if (_3Al != null && _3A_0017 >= 1)
		{
			_3Al._67(_3AD, _3A_0003, _3Ac, _3A_0017);
		}
	}

	private void _6_0016()
	{
		_3Al = _3AF.New();
		_3At.Add(_3Al);
		_3A_0017 = 0;
	}

	internal void U()
	{
		foreach (g item in _3At)
		{
			_3AF.Free(item);
		}
		_3Al = null;
		_3At.Clear();
		_3A_0017 = 0;
	}
}
