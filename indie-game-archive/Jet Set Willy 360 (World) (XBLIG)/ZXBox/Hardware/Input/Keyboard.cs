using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using ZXBox.Hardware.Interfaces;

namespace ZXBox.Hardware.Input;

public class Keyboard : IInput
{
	private KeyboardState ks;

	private KeyboardState virtualks;

	private bool virtalStateAvailible = true;

	private List<Keys> KeyBuffer = new List<Keys>();

	private int NoKeyCounter;

	private bool SymbolShift;

	private int sectionnumber = 1;

	public void SetKeystate(KeyboardState state)
	{
		virtualks = state;
		virtalStateAvailible = true;
	}

	private bool IsNextKey(Keys key)
	{
		return false;
	}

	public Keyboard()
	{
		ks = Microsoft.Xna.Framework.Input.Keyboard.GetState();
	}

	public int Input(int Port, int tstates)
	{
		if ((Port & 0xFF) == 254)
		{
			DateTime now = DateTime.Now;
			if (sectionnumber++ == 1)
			{
				if (virtalStateAvailible)
				{
					ks = virtualks;
				}
				else
				{
					ks = Microsoft.Xna.Framework.Input.Keyboard.GetState();
				}
			}
			if (sectionnumber > 8)
			{
				sectionnumber = 1;
			}
			_ = DateTime.Now - now;
			int num = 255;
			switch ((Port >> 8) & 0xF)
			{
			case 14:
				if (ks.IsKeyDown(Keys.RightShift) || ks.IsKeyDown(Keys.LeftShift) || ks.IsKeyDown(Keys.Back) || IsNextKey(Keys.Back) || IsNextKey(Keys.LeftShift))
				{
					num &= -2;
				}
				if (ks.IsKeyDown(Keys.Z) || IsNextKey(Keys.Z))
				{
					num &= -3;
				}
				if (ks.IsKeyDown(Keys.X) || IsNextKey(Keys.X))
				{
					num &= -5;
				}
				if (ks.IsKeyDown(Keys.C) || IsNextKey(Keys.Z))
				{
					num &= -9;
				}
				if (ks.IsKeyDown(Keys.V) || IsNextKey(Keys.V))
				{
					num &= -17;
				}
				break;
			case 13:
				if (ks.IsKeyDown(Keys.A) || IsNextKey(Keys.A))
				{
					num &= -2;
				}
				if (ks.IsKeyDown(Keys.S) || IsNextKey(Keys.S))
				{
					num &= -3;
				}
				if (ks.IsKeyDown(Keys.D) || IsNextKey(Keys.D))
				{
					num &= -5;
				}
				if (ks.IsKeyDown(Keys.F) || IsNextKey(Keys.F))
				{
					num &= -9;
				}
				if (ks.IsKeyDown(Keys.G) || IsNextKey(Keys.G))
				{
					num &= -17;
				}
				break;
			case 11:
				if (ks.IsKeyDown(Keys.Q) || IsNextKey(Keys.Q))
				{
					num &= -2;
				}
				if (ks.IsKeyDown(Keys.W) || IsNextKey(Keys.W))
				{
					num &= -3;
				}
				if (ks.IsKeyDown(Keys.E) || IsNextKey(Keys.E))
				{
					num &= -5;
				}
				if (ks.IsKeyDown(Keys.R) || IsNextKey(Keys.R))
				{
					num &= -9;
				}
				if (ks.IsKeyDown(Keys.T) || IsNextKey(Keys.T))
				{
					num &= -17;
				}
				break;
			case 7:
				if (ks.IsKeyDown(Keys.D1) || IsNextKey(Keys.D1))
				{
					num &= -2;
				}
				if (ks.IsKeyDown(Keys.D2) || IsNextKey(Keys.D2))
				{
					num &= -3;
				}
				if (ks.IsKeyDown(Keys.D3) || IsNextKey(Keys.D3))
				{
					num &= -5;
				}
				if (ks.IsKeyDown(Keys.D4) || IsNextKey(Keys.D4))
				{
					num &= -9;
				}
				if (ks.IsKeyDown(Keys.D5) || IsNextKey(Keys.D5))
				{
					num &= -17;
				}
				break;
			}
			switch ((Port >> 8) & 0xF0)
			{
			case 224:
				if (ks.IsKeyDown(Keys.D0) || ks.IsKeyDown(Keys.Back) || IsNextKey(Keys.D0) || IsNextKey(Keys.Back))
				{
					num &= -2;
				}
				if (ks.IsKeyDown(Keys.D9) || IsNextKey(Keys.D9))
				{
					num &= -3;
				}
				if (ks.IsKeyDown(Keys.D8) || IsNextKey(Keys.D8))
				{
					num &= -5;
				}
				if (ks.IsKeyDown(Keys.D7) || IsNextKey(Keys.D7))
				{
					num &= -9;
				}
				if (ks.IsKeyDown(Keys.D6) || IsNextKey(Keys.D6))
				{
					num &= -17;
				}
				break;
			case 208:
				if (ks.IsKeyDown(Keys.P) || IsNextKey(Keys.P))
				{
					num &= -2;
				}
				if (ks.IsKeyDown(Keys.O) || IsNextKey(Keys.O))
				{
					num &= -3;
				}
				if (ks.IsKeyDown(Keys.I) || IsNextKey(Keys.I))
				{
					num &= -5;
				}
				if (ks.IsKeyDown(Keys.U) || IsNextKey(Keys.U))
				{
					num &= -9;
				}
				if (ks.IsKeyDown(Keys.Y) || IsNextKey(Keys.Y))
				{
					num &= -17;
				}
				break;
			case 176:
				if (ks.IsKeyDown(Keys.Enter) || ks.IsKeyDown(Keys.Enter) || IsNextKey(Keys.Enter))
				{
					num &= -2;
				}
				if (ks.IsKeyDown(Keys.L) || IsNextKey(Keys.L))
				{
					num &= -3;
				}
				if (ks.IsKeyDown(Keys.K) || IsNextKey(Keys.K))
				{
					num &= -5;
				}
				if (ks.IsKeyDown(Keys.J) || IsNextKey(Keys.J))
				{
					num &= -9;
				}
				if (ks.IsKeyDown(Keys.H) || IsNextKey(Keys.H))
				{
					num &= -17;
				}
				break;
			case 112:
				if (ks.IsKeyDown(Keys.Space) || IsNextKey(Keys.Space))
				{
					num &= -2;
				}
				if (ks.IsKeyDown(Keys.RightAlt) || ks.IsKeyDown(Keys.LeftAlt) || IsNextKey(Keys.LeftAlt) || IsNextKey(Keys.RightAlt))
				{
					SymbolShift = !SymbolShift;
				}
				if (SymbolShift)
				{
					num &= -3;
				}
				if (ks.IsKeyDown(Keys.M) || IsNextKey(Keys.M))
				{
					num &= -5;
				}
				if (ks.IsKeyDown(Keys.N) || IsNextKey(Keys.N))
				{
					num &= -9;
				}
				if (ks.IsKeyDown(Keys.B) || IsNextKey(Keys.B))
				{
					num &= -17;
				}
				break;
			}
			return num;
		}
		return 255;
	}

	public void EnterText(string text)
	{
		if (string.IsNullOrEmpty(text))
		{
			return;
		}
		KeyBuffer.Clear();
		string text2 = text.ToLower();
		for (int i = 0; i < text2.Length; i++)
		{
			switch (text2[i])
			{
			case '1':
				KeyBuffer.Add(Keys.D1);
				break;
			case '2':
				KeyBuffer.Add(Keys.D2);
				break;
			case '3':
				KeyBuffer.Add(Keys.D3);
				break;
			case '4':
				KeyBuffer.Add(Keys.D4);
				break;
			case '5':
				KeyBuffer.Add(Keys.D5);
				break;
			case '6':
				KeyBuffer.Add(Keys.D6);
				break;
			}
			KeyBuffer.Add(Keys.None);
		}
	}
}
