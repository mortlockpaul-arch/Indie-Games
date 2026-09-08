using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _0003;

internal class c
{
	private static int _3A_0018;

	private static float _3AL;

	private static double _3A_0019;

	private static StringBuilder _3A3 = new StringBuilder();

	private void LB(StringBuilder P_0, int P_1, int P_2)
	{
		if (P_1 <= 0)
		{
			P_0.Append("0");
			return;
		}
		int length = P_0.Length;
		int num = 0;
		while (P_1 > 0)
		{
			int num2 = P_1 / 10;
			switch (P_1 - num2 * 10)
			{
			case 0:
				P_0.Insert(length, "0");
				break;
			case 1:
				P_0.Insert(length, "1");
				break;
			case 2:
				P_0.Insert(length, "2");
				break;
			case 3:
				P_0.Insert(length, "3");
				break;
			case 4:
				P_0.Insert(length, "4");
				break;
			case 5:
				P_0.Insert(length, "5");
				break;
			case 6:
				P_0.Insert(length, "6");
				break;
			case 7:
				P_0.Insert(length, "7");
				break;
			case 8:
				P_0.Insert(length, "8");
				break;
			case 9:
				P_0.Insert(length, "9");
				break;
			}
			num++;
			if (num == P_2 && num != 0 && P_1 > 0)
			{
				P_0.Insert(length, ".");
			}
			P_1 = num2;
		}
		if (num < P_2)
		{
			for (int i = num; i < P_2; i++)
			{
				P_0.Insert(length, "0");
			}
			P_0.Insert(length, ".");
		}
	}

	internal Vector2 e(SpriteBatch P_0, SpriteFont P_1, ref Vector2 P_2, Vector2 P_3, Color P_4)
	{
		P_0.DrawString(P_1, _3A3, P_2, P_4, 0f, Vector2.Zero, P_3, SpriteEffects.None, 0f);
		P_2.Y += 14f * P_3.Y;
		return new Vector2((float)_3A3.Length * 8f * P_3.X, 14f * P_3.Y);
	}

	internal void LV(string P_0)
	{
		_3A3.Length = 0;
		_3A3.Append(P_0);
	}

	internal void Ld(string P_0, int P_1)
	{
		_3A3.Length = 0;
		_3A3.Append(P_0);
		_3A3.Append(": ");
		LB(_3A3, P_1, 0);
	}

	private void Ld(string P_0, float P_1)
	{
		_3A3.Length = 0;
		_3A3.Append(P_0);
		_3A3.Append(": ");
		LB(_3A3, (int)(P_1 * 100f), 2);
	}

	internal void L_0006(string P_0, GameTime P_1, bool P_2)
	{
		double totalSeconds = P_1.TotalGameTime.TotalSeconds;
		if (totalSeconds - _3A_0019 >= 0.5)
		{
			_3AL = (float)_3A_0018 / (float)(totalSeconds - _3A_0019);
			_3A_0018 = 0;
			_3A_0019 = totalSeconds;
		}
		Ld(P_0, _3AL);
		if (P_2)
		{
			_3A_0018++;
		}
	}

	public override string ToString()
	{
		return _3A3.ToString();
	}
}
