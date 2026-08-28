using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ZXBox.Hardware.Interfaces;
using Zilog;

namespace ZXBox.Hardware.Output;

public class Screen : IOutput
{
	private uint[] colours;

	private int bordercounter;

	private Z80 cpu;

	private int bordertop = 48;

	private int borderbottom = 56;

	private int bordersides = 64;

	public int Height;

	private bool RenderBorder;

	public int Width;

	public double tstatesperpixel = 0.58;

	private uint[] screen;

	private List<Border> border = new List<Border>();

	public uint LastBorderColor;

	public void SwitchColors(bool switchColors)
	{
		if (switchColors)
		{
			colours = new uint[16]
			{
				4278190080u, 4278190285u, 4291624960u, 4291625165u, 4278242560u, 4278242765u, 4291677440u, 4291677645u, 4278190080u, 4278190335u,
				4294901760u, 4294902015u, 4278255360u, 4278255615u, 4294967040u, 4294967295u
			};
		}
		else
		{
			colours = new uint[16]
			{
				4278190080u, 4291624960u, 4278190285u, 4291625165u, 4278242560u, 4291677440u, 4278242765u, 4291677645u, 4278190080u, 4294901760u,
				4278190335u, 4294902015u, 4278255360u, 4294967040u, 4278255615u, 4294967295u
			};
		}
	}

	public Screen(Z80 cpu, bool renderBorder, bool switchColors)
	{
		SwitchColors(switchColors);
		RenderBorder = renderBorder;
		if (renderBorder)
		{
			bordertop = 5;
			borderbottom = 5;
			bordersides = 5;
		}
		else
		{
			bordertop = 0;
			borderbottom = 0;
			bordersides = 0;
		}
		Height = 192 + (bordertop + borderbottom);
		Width = 256 + bordersides * 2;
		screen = new uint[Height * Width];
		this.cpu = cpu;
	}

	public Color GetBackgroundColor()
	{
		return new Color((int)(colours[LastBorderColor] & 0xFF), ((int)colours[LastBorderColor] >> 8) & 0xFF, ((int)colours[LastBorderColor] >> 16) & 0xFF);
	}

	private uint GetBorderColor(double tState)
	{
		if (RenderBorder)
		{
			if (tState == 0.0)
			{
				bordercounter = 0;
			}
			if (border.Count - 1 > bordercounter && (double)border[bordercounter + 1].tState <= tState)
			{
				bordercounter++;
			}
			if (bordercounter > border.Count - 1)
			{
				bordercounter = border.Count - 1;
			}
			LastBorderColor = border[bordercounter].ColorByte;
			if (tState == 69887.0)
			{
				border.Clear();
			}
			if (border.Count == 0)
			{
				return LastBorderColor;
			}
			return colours[LastBorderColor];
		}
		return colours[0];
	}

	public void Output(int Port, int ByteValue, int tState)
	{
		if ((Port & 1) == 0)
		{
			border.Add(new Border((uint)(ByteValue & 7), tState));
		}
	}

	public uint[] drawScreen(bool flash)
	{
		double num = 0.0;
		bordercounter = 0;
		int num2 = 0;
		int num3 = 16384;
		uint num4 = colours[0];
		uint num5 = colours[15];
		int num6 = Width * bordertop;
		while (num6-- > 0)
		{
			screen[num2++] = GetBorderColor(num += tstatesperpixel);
		}
		int num7 = num3 + 6144;
		int num8 = num3;
		int num9 = 0;
		while (num9 < 192)
		{
			num6 = bordersides;
			while (num6-- > 0)
			{
				screen[num2] = GetBorderColor(num += tstatesperpixel);
				num2++;
			}
			num8 = num3 + ((((num9 & 7) << 3) | ((num9 & 0x38) >> 3) | (num9 & 0xC0)) << 5);
			num6 = 32;
			while (num6-- > 0)
			{
				if (!flash || (cpu.Memory[num7] & 0x80) == 0)
				{
					num4 = colours[(cpu.Memory[num7] & 7) + (((cpu.Memory[num7] & 0x40) != 0) ? 8 : 0)];
					num5 = colours[((cpu.Memory[num7] >> 3) & 7) + (((cpu.Memory[num7] & 0x40) != 0) ? 8 : 0)];
				}
				else
				{
					num5 = colours[(cpu.Memory[num7] & 7) + (((cpu.Memory[num7] & 0x40) != 0) ? 8 : 0)];
					num4 = colours[((cpu.Memory[num7] >> 3) & 7) + (((cpu.Memory[num7] & 0x40) != 0) ? 8 : 0)];
				}
				num7++;
				int num10 = 128;
				int num11 = cpu.Memory[num8++];
				while (num10 != 0)
				{
					screen[num2++] = (((num10 & num11) != 0) ? num4 : num5);
					num += tstatesperpixel;
					num10 >>= 1;
				}
			}
			num6 = bordersides;
			while (num6-- > 0)
			{
				screen[num2] = GetBorderColor(num += tstatesperpixel);
				num2++;
			}
			if ((++num9 & 7) != 0)
			{
				num7 -= 32;
			}
		}
		num6 = Width * borderbottom;
		while (num6-- > 0)
		{
			screen[num2++] = GetBorderColor(num += tstatesperpixel);
		}
		border.Clear();
		border.Add(new Border(LastBorderColor, 0));
		return screen;
	}

	public void drawScreen(int[] screen, bool flash)
	{
		double num = 0.0;
		bordercounter = 0;
		int num2 = 0;
		int num3 = 16384;
		uint num4 = colours[0];
		uint num5 = colours[15];
		int num6 = Width * bordertop;
		while (num6-- > 0)
		{
			screen[num2++] = (int)GetBorderColor(num += tstatesperpixel);
		}
		int num7 = num3 + 6144;
		int num8 = num3;
		int num9 = 0;
		while (num9 < 192)
		{
			num6 = bordersides;
			while (num6-- > 0)
			{
				screen[num2] = (int)GetBorderColor(num += tstatesperpixel);
				num2++;
			}
			num8 = num3 + ((((num9 & 7) << 3) | ((num9 & 0x38) >> 3) | (num9 & 0xC0)) << 5);
			num6 = 32;
			while (num6-- > 0)
			{
				if (!flash || (cpu.Memory[num7] & 0x80) == 0)
				{
					num4 = colours[(cpu.Memory[num7] & 7) + (((cpu.Memory[num7] & 0x40) != 0) ? 8 : 0)];
					num5 = colours[((cpu.Memory[num7] >> 3) & 7) + (((cpu.Memory[num7] & 0x40) != 0) ? 8 : 0)];
				}
				else
				{
					num5 = colours[(cpu.Memory[num7] & 7) + (((cpu.Memory[num7] & 0x40) != 0) ? 8 : 0)];
					num4 = colours[((cpu.Memory[num7] >> 3) & 7) + (((cpu.Memory[num7] & 0x40) != 0) ? 8 : 0)];
				}
				num7++;
				int num10 = 128;
				int num11 = cpu.Memory[num8++];
				while (num10 != 0)
				{
					screen[num2++] = (int)(((num10 & num11) != 0) ? num4 : num5);
					num += tstatesperpixel;
					num10 >>= 1;
				}
			}
			num6 = bordersides;
			while (num6-- > 0)
			{
				screen[num2] = (int)GetBorderColor(num += tstatesperpixel);
				num2++;
			}
			if ((++num9 & 7) != 0)
			{
				num7 -= 32;
			}
		}
		num6 = Width * borderbottom;
		while (num6-- > 0)
		{
			screen[num2++] = (int)GetBorderColor(num += tstatesperpixel);
		}
		border.Clear();
		border.Add(new Border(LastBorderColor, 0));
	}
}
