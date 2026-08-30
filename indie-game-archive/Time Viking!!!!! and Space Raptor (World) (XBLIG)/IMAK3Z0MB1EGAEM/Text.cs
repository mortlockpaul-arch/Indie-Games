using System.Collections.Generic;
using IMAK3Z0MB1EGAEM.text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Viking_x86;

namespace IMAK3Z0MB1EGAEM;

public class Text
{
	public enum Justify
	{
		Left,
		Right,
		Center
	}

	private static Dictionary<char, Shape> shapes;

	private static Texture2D nullTex;

	public static void Init(Texture2D _nullTex)
	{
		nullTex = _nullTex;
		shapes = new Dictionary<char, Shape>();
		shapes.Add('a', new Shape(new int[5] { 0, 0, 1110, 1010, 1111 }));
		shapes.Add('b', new Shape(new int[5] { 100, 100, 111, 101, 111 }));
		shapes.Add('c', new Shape(new int[5] { 0, 0, 111, 100, 111 }));
		shapes.Add('d', new Shape(new int[5] { 1, 1, 111, 101, 111 }));
		shapes.Add('e', new Shape(new int[5] { 0, 110, 1111, 1000, 110 }));
		shapes.Add('f', new Shape(new int[5] { 1, 10, 111, 10, 10 }));
		shapes.Add('g', new Shape(new int[5] { 111, 101, 111, 1, 110 }));
		shapes.Add('h', new Shape(new int[5] { 100, 100, 110, 101, 101 }));
		shapes.Add('i', new Shape(new int[5] { 1, 0, 1, 1, 1 }));
		shapes.Add('j', new Shape(new int[5] { 1, 0, 1, 1, 10 }));
		shapes.Add('k', new Shape(new int[5] { 100, 100, 111, 110, 101 }));
		shapes.Add('l', new Shape(new int[5] { 1, 1, 1, 1, 1 }));
		shapes.Add('m', new Shape(new int[5] { 0, 0, 11110, 10101, 10101 }));
		shapes.Add('n', new Shape(new int[5] { 0, 0, 110, 101, 101 }));
		shapes.Add('o', new Shape(new int[5] { 0, 0, 111, 101, 111 }));
		shapes.Add('p', new Shape(new int[5] { 0, 110, 101, 110, 100 }));
		shapes.Add('q', new Shape(new int[5] { 0, 11, 101, 11, 1 }));
		shapes.Add('r', new Shape(new int[5] { 0, 0, 11, 10, 10 }));
		shapes.Add('s', new Shape(new int[5] { 11, 100, 10, 1, 110 }));
		shapes.Add('t', new Shape(new int[5] { 0, 10, 111, 10, 10 }));
		shapes.Add('u', new Shape(new int[5] { 0, 0, 101, 101, 11 }));
		shapes.Add('v', new Shape(new int[5] { 0, 0, 101, 101, 10 }));
		shapes.Add('w', new Shape(new int[5] { 0, 0, 10101, 10101, 1010 }));
		shapes.Add('x', new Shape(new int[5] { 0, 0, 101, 10, 101 }));
		shapes.Add('y', new Shape(new int[5] { 0, 0, 101, 11, 111 }));
		shapes.Add('z', new Shape(new int[5] { 0, 111, 10, 100, 111 }));
		shapes.Add('A', new Shape(new int[5] { 111, 101, 111, 101, 101 }));
		shapes.Add('B', new Shape(new int[5] { 111, 101, 110, 101, 111 }));
		shapes.Add('C', new Shape(new int[5] { 111, 100, 100, 100, 111 }));
		shapes.Add('D', new Shape(new int[5] { 110, 101, 101, 101, 110 }));
		shapes.Add('E', new Shape(new int[5] { 111, 100, 110, 100, 111 }));
		shapes.Add('F', new Shape(new int[5] { 111, 100, 110, 100, 100 }));
		shapes.Add('G', new Shape(new int[5] { 1111, 1000, 1011, 1001, 1111 }));
		shapes.Add('H', new Shape(new int[5] { 101, 101, 111, 101, 101 }));
		shapes.Add('I', new Shape(new int[5] { 111, 10, 10, 10, 111 }));
		shapes.Add('J', new Shape(new int[5] { 1, 1, 1, 1, 110 }));
		shapes.Add('K', new Shape(new int[5] { 101, 101, 110, 101, 101 }));
		shapes.Add('L', new Shape(new int[5] { 100, 100, 100, 100, 111 }));
		shapes.Add('M', new Shape(new int[5] { 10001, 11011, 10101, 10001, 10001 }));
		shapes.Add('N', new Shape(new int[5] { 10001, 11001, 10101, 10011, 10001 }));
		shapes.Add('O', new Shape(new int[5] { 1110, 10001, 10001, 10001, 1110 }));
		shapes.Add('P', new Shape(new int[5] { 110, 101, 110, 100, 100 }));
		shapes.Add('Q', new Shape(new int[5] { 1110, 10001, 10101, 10011, 1111 }));
		shapes.Add('R', new Shape(new int[5] { 1110, 1001, 1110, 1010, 1001 }));
		shapes.Add('S', new Shape(new int[5] { 111, 1000, 110, 1, 1110 }));
		shapes.Add('T', new Shape(new int[5] { 111, 10, 10, 10, 10 }));
		shapes.Add('U', new Shape(new int[5] { 101, 101, 101, 101, 111 }));
		shapes.Add('V', new Shape(new int[5] { 10001, 10001, 1010, 1010, 100 }));
		shapes.Add('W', new Shape(new int[5] { 10001, 10101, 10101, 10101, 1010 }));
		shapes.Add('X', new Shape(new int[5] { 10001, 1010, 100, 1010, 10001 }));
		shapes.Add('Y', new Shape(new int[5] { 10001, 1010, 100, 100, 100 }));
		shapes.Add('Z', new Shape(new int[5] { 11111, 10, 100, 1000, 11111 }));
		shapes.Add('0', new Shape(new int[5] { 1110, 10011, 10101, 11001, 1110 }));
		shapes.Add('1', new Shape(new int[5] { 10, 110, 10, 10, 111 }));
		shapes.Add('2', new Shape(new int[5] { 110, 1, 10, 100, 111 }));
		shapes.Add('3', new Shape(new int[5] { 1110, 1, 110, 1, 1110 }));
		shapes.Add('4', new Shape(new int[5] { 10, 1010, 1111, 10, 10 }));
		shapes.Add('5', new Shape(new int[5] { 111, 100, 110, 1, 110 }));
		shapes.Add('6', new Shape(new int[5] { 11, 100, 111, 101, 111 }));
		shapes.Add('7', new Shape(new int[5] { 1111, 1, 10, 100, 100 }));
		shapes.Add('8', new Shape(new int[5] { 110, 1001, 110, 1001, 110 }));
		shapes.Add('9', new Shape(new int[5] { 110, 1001, 111, 1, 1 }));
		shapes.Add('.', new Shape(new int[5] { 0, 0, 0, 0, 1 }));
		shapes.Add('\'', new Shape(new int[5] { 1, 1, 0, 0, 0 }));
		shapes.Add('>', new Shape(new int[5] { 1000, 100, 10, 100, 1000 }));
		shapes.Add('"', new Shape(new int[5] { 101, 101, 0, 0, 0 }));
		shapes.Add('!', new Shape(new int[5] { 1, 1, 1, 0, 1 }));
		shapes.Add('?', new Shape(new int[5] { 111, 1, 10, 0, 10 }));
		shapes.Add(',', new Shape(new int[5] { 0, 0, 0, 1, 11 }));
		shapes.Add('-', new Shape(new int[5] { 0, 0, 111, 0, 0 }));
		shapes.Add('+', new Shape(new int[5] { 0, 10, 111, 10, 0 }));
		shapes.Add('#', new Shape(new int[5] { 1010, 11111, 1010, 11111, 1010 }));
		Dictionary<char, Shape> dictionary = shapes;
		int[] src = new int[5];
		dictionary.Add(' ', new Shape(src));
		shapes.Add(':', new Shape(new int[5] { 0, 10, 0, 10, 0 }));
		shapes.Add('*', new Shape(new int[5] { 10101, 1110, 11111, 1110, 10101 }));
		shapes.Add('(', new Shape(new int[5] { 1, 10, 10, 10, 1 }));
		shapes.Add(')', new Shape(new int[5] { 10, 1, 1, 1, 10 }));
	}

	public static void DrawString(char[] str, Vector2 loc, float size, Color c, Justify jus, int flashChar)
	{
		float num = 0f;
		for (int i = 0; i < str.Length; i++)
		{
			if (!shapes.ContainsKey(str[i]))
			{
				shapes.Add(str[i], new Shape(new int[5] { 10101, 1110, 11111, 1110, 10101 }));
			}
			num += (float)(shapes[str[i]].width + 1) * size;
		}
		switch (jus)
		{
		case Justify.Right:
			loc.X -= num;
			break;
		case Justify.Center:
			loc.X -= num / 2f;
			break;
		}
		loc.Y -= 2.5f * size;
		for (int j = 0; j < str.Length; j++)
		{
			shapes[str[j]].Draw(loc, size, (j == flashChar) ? new Color(Rand.GetRandomFloat(0.5f, 1f), Rand.GetRandomFloat(0.5f, 1f), Rand.GetRandomFloat(0.5f, 1f), 1f) : c, nullTex);
			loc.X += (float)(shapes[str[j]].width + 1) * size;
		}
	}

	public static void DrawString(string str, Vector2 loc, float size, Color c, Justify jus)
	{
		float num = 0f;
		for (int i = 0; i < str.Length; i++)
		{
			if (!shapes.ContainsKey(str[i]))
			{
				shapes.Add(str[i], new Shape(new int[5] { 10101, 1110, 11111, 1110, 10101 }));
			}
			num += (float)(shapes[str[i]].width + 1) * size;
		}
		switch (jus)
		{
		case Justify.Right:
			loc.X -= num;
			break;
		case Justify.Center:
			loc.X -= num / 2f;
			break;
		}
		loc.Y -= 2.5f * size;
		for (int j = 0; j < str.Length; j++)
		{
			shapes[str[j]].Draw(loc, size, c, nullTex);
			loc.X += (float)(shapes[str[j]].width + 1) * size;
		}
	}

	public static void DrawString(string str, Vector2 loc, float size, Color c, Justify jus, float angle)
	{
		float num = 0f;
		for (int i = 0; i < str.Length; i++)
		{
			if (!shapes.ContainsKey(str[i]))
			{
				shapes.Add(str[i], new Shape(new int[5] { 10101, 1110, 11111, 1110, 10101 }));
			}
			num += (float)(shapes[str[i]].width + 1) * size;
		}
		switch (jus)
		{
		case Justify.Right:
			loc -= new Vector2(num * VScroll.xVec.X, num * VScroll.xVec.Y);
			break;
		case Justify.Center:
			loc -= new Vector2(num * VScroll.xVec.X, num * VScroll.xVec.Y) / 2f;
			break;
		}
		loc -= 2.5f * new Vector2(size * (0f - VScroll.yVec.X), size * (0f - VScroll.yVec.Y));
		for (int j = 0; j < str.Length; j++)
		{
			shapes[str[j]].Draw(loc, size, c, angle, nullTex);
			loc += new Vector2((float)(shapes[str[j]].width + 1) * size * VScroll.xVec.X, (float)(shapes[str[j]].width + 1) * size * VScroll.xVec.Y);
		}
	}

	private static char ntochar(long n)
	{
		if (n <= 9 && n >= 0)
		{
			switch ((int)n)
			{
			case 0:
				return '0';
			case 1:
				return '1';
			case 2:
				return '2';
			case 3:
				return '3';
			case 4:
				return '4';
			case 5:
				return '5';
			case 6:
				return '6';
			case 7:
				return '7';
			case 8:
				return '8';
			case 9:
				return '9';
			}
		}
		return '0';
	}

	public static void DrawScore(long score, Vector2 loc, float size, Color c, Justify jus)
	{
		float num = 0f;
		for (long num2 = score; num2 > 0; num2 /= 10)
		{
			num += (float)(shapes[ntochar(num2 % 10)].width + 1) * size;
		}
		switch (jus)
		{
		case Justify.Left:
			loc.X += num;
			break;
		case Justify.Center:
			loc.X += num / 2f;
			break;
		}
		for (long num2 = score; num2 > 0; num2 /= 10)
		{
			char key = ntochar(num2 % 10);
			loc.X -= (float)(shapes[key].width + 1) * size;
			shapes[key].Draw(loc, size, c, nullTex);
		}
	}
}
