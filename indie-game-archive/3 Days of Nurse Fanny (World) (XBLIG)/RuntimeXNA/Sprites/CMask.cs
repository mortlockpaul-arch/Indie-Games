using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RuntimeXNA.Banks;

namespace RuntimeXNA.Sprites;

public class CMask
{
	public const int SCMF_FULL = 0;

	public const int SCMF_PLATFORM = 1;

	public const int GCMF_OBSTACLE = 0;

	public const int GCMF_PLATFORM = 1;

	public ushort[] mask;

	public int lineWidth;

	public int height;

	public int width;

	public int xSpot;

	public int ySpot;

	private static ushort[] lMask = new ushort[16]
	{
		65535, 32767, 16383, 8191, 4095, 2047, 1023, 511, 255, 127,
		63, 31, 15, 7, 3, 1
	};

	private static ushort[] rMask = new ushort[17]
	{
		0, 32768, 49152, 57344, 61440, 63488, 64512, 65024, 65280, 65408,
		65472, 65504, 65520, 65528, 65532, 65534, 65535
	};

	public void createMask(CImage image, int nFlags)
	{
		width = image.width;
		height = image.height;
		xSpot = image.xSpot;
		ySpot = image.ySpot;
		int[] array = new int[width * height];
		Texture2D texture2D = image.image;
		Rectangle? rect = null;
		if (image.mosaic != 0)
		{
			texture2D = image.app.imageBank.mosaics[image.mosaic];
			rect = image.mosaicRectangle;
		}
		texture2D.GetData(0, rect, array, 0, width * height);
		int num = (int)(((width + 15) & 0xFFFFFFF0u) / 16);
		mask = new ushort[num * height + 1];
		lineWidth = num;
		for (int i = 0; i < num * height + 1; i++)
		{
			mask[i] = 0;
		}
		if ((nFlags & 1) == 0)
		{
			for (int j = 0; j < height; j++)
			{
				for (int i = 0; i < width; i++)
				{
					int num2 = (int)(j * num + (i & 0xFFFFFFF0u) / 16);
					if ((array[j * width + i] & 0xFF000000u) != 0)
					{
						ushort num3 = (ushort)(32768 >> i % 16);
						mask[num2] |= num3;
					}
				}
			}
			return;
		}
		for (int i = 0; i < width; i++)
		{
			int j;
			for (j = 0; j < height && (array[j * width + i] & 0xFF000000u) == 0; j++)
			{
			}
			if (j >= height)
			{
				continue;
			}
			int num4 = Math.Min(height, j + 6);
			ushort num3 = (ushort)(32768 >> (i & 0xF));
			for (; j < num4; j++)
			{
				if ((array[j * width + i] & 0xFF000000u) != 0)
				{
					int num2 = j * num + i / 16;
					mask[num2] |= num3;
				}
			}
		}
	}

	private void rotateRect(ref int pWidth, ref int pHeight, ref int pHX, ref int pHY, double fAngle)
	{
		double num;
		double num2;
		if (fAngle == 90.0)
		{
			num = 0.0;
			num2 = 1.0;
		}
		else if (fAngle == 180.0)
		{
			num = -1.0;
			num2 = 0.0;
		}
		else if (fAngle == 270.0)
		{
			num = 0.0;
			num2 = -1.0;
		}
		else
		{
			double num3 = fAngle * 0.017453292;
			num = Math.Cos(num3);
			num2 = Math.Sin(num3);
		}
		double num4 = (double)(-pHX) * num;
		double num5 = (double)(-pHX) * num2;
		double num6 = (double)(-pHY) * num;
		double num7 = (double)(-pHY) * num2;
		int num8 = (int)(num4 + num7);
		int num9 = (int)(num6 - num5);
		double num10 = pWidth - pHX;
		num4 = num10 * num;
		num5 = num10 * num2;
		int num11 = (int)(num4 + num7);
		int num12 = (int)(num6 - num5);
		double num13 = pHeight - pHY;
		num6 = num13 * num;
		num7 = num13 * num2;
		int num14 = (int)(num4 + num7);
		int num15 = (int)(num6 - num5);
		int val = num8 + num14 - num11;
		int val2 = num9 + num15 - num12;
		int num16 = Math.Min(num8, Math.Min(num11, Math.Min(num14, val)));
		int num17 = Math.Min(num9, Math.Min(num12, Math.Min(num15, val2)));
		int num18 = Math.Max(num8, Math.Max(num11, Math.Max(num14, val)));
		int num19 = Math.Max(num9, Math.Max(num12, Math.Max(num15, val2)));
		pHX = -num16;
		pHY = -num17;
		pWidth = num18 - num16;
		pHeight = num19 - num17;
	}

	public bool createRotatedMask(CMask pMask, double fAngle, double fScaleX, double fScaleY)
	{
		int num = pMask.width;
		int num2 = pMask.height;
		int pWidth = (int)((double)pMask.width * fScaleX);
		int pHeight = (int)((double)pMask.height * fScaleY);
		int pHX = (int)((double)pMask.xSpot * fScaleX);
		int pHY = (int)((double)pMask.ySpot * fScaleY);
		rotateRect(ref pWidth, ref pHeight, ref pHX, ref pHY, fAngle);
		int num3 = pWidth;
		int num4 = pHeight;
		if (num3 <= 0 || num4 <= 0)
		{
			return false;
		}
		int num5 = pMask.lineWidth;
		int num6 = ((num3 + 15) & 0x7FFFFFF0) / 16;
		mask = new ushort[num6 * num4 + 1];
		lineWidth = num6;
		width = num3;
		height = num4;
		xSpot = pHX;
		ySpot = pHY;
		double num7 = fAngle * 0.017453292;
		double num8 = Math.Cos(num7);
		double num9 = Math.Sin(num7);
		double num10 = (double)num / 2.0 - ((double)num3 / 2.0 * num8 - (double)num4 / 2.0 * num9) / fScaleX;
		double num11 = (double)num2 / 2.0 - ((double)num3 / 2.0 * num9 + (double)num4 / 2.0 * num8) / fScaleY;
		int num12 = 0;
		int num13 = num12;
		int num14 = (int)(num10 * 65536.0);
		int num15 = (int)(num11 * 65536.0);
		int num16 = (int)(num8 * 65536.0 / fScaleX);
		int num17 = (int)(num9 * 65536.0 / fScaleY);
		int num18 = num3 / 16;
		int num19 = num3 % 16;
		int num20 = (int)(num8 * 65536.0 / fScaleY);
		int num21 = (int)(num9 * 65536.0 / fScaleX);
		int num22 = num * 65536;
		int num23 = num2 * 65536;
		for (int i = 0; i < num4; i++)
		{
			int num24 = num14;
			int num25 = num15;
			int num26 = num13;
			for (int j = 0; j < num18; j++)
			{
				ushort num27 = 0;
				if (num24 >= 0 && num24 < num22 && num25 >= 0 && num25 < num23)
				{
					int num28 = num24 / 65536;
					int num29 = num25 / 65536;
					ushort num30 = (ushort)(32768 >> num28 % 16);
					ushort num31 = pMask.mask[num29 * num5 + num28 / 16];
					if ((num31 & num30) != 0)
					{
						num27 |= 0x8000;
					}
				}
				num24 += num16;
				num25 += num17;
				if (num24 >= 0 && num24 < num22 && num25 >= 0 && num25 < num23)
				{
					int num28 = num24 / 65536;
					int num29 = num25 / 65536;
					ushort num30 = (ushort)(32768 >> num28 % 16);
					ushort num31 = pMask.mask[num29 * num5 + num28 / 16];
					if ((num31 & num30) != 0)
					{
						num27 |= 0x4000;
					}
				}
				num24 += num16;
				num25 += num17;
				if (num24 >= 0 && num24 < num22 && num25 >= 0 && num25 < num23)
				{
					int num28 = num24 / 65536;
					int num29 = num25 / 65536;
					ushort num30 = (ushort)(32768 >> num28 % 16);
					ushort num31 = pMask.mask[num29 * num5 + num28 / 16];
					if ((num31 & num30) != 0)
					{
						num27 |= 0x2000;
					}
				}
				num24 += num16;
				num25 += num17;
				if (num24 >= 0 && num24 < num22 && num25 >= 0 && num25 < num23)
				{
					int num28 = num24 / 65536;
					int num29 = num25 / 65536;
					ushort num30 = (ushort)(32768 >> num28 % 16);
					ushort num31 = pMask.mask[num29 * num5 + num28 / 16];
					if ((num31 & num30) != 0)
					{
						num27 |= 0x1000;
					}
				}
				num24 += num16;
				num25 += num17;
				if (num24 >= 0 && num24 < num22 && num25 >= 0 && num25 < num23)
				{
					int num28 = num24 / 65536;
					int num29 = num25 / 65536;
					ushort num30 = (ushort)(32768 >> num28 % 16);
					ushort num31 = pMask.mask[num29 * num5 + num28 / 16];
					if ((num31 & num30) != 0)
					{
						num27 |= 0x800;
					}
				}
				num24 += num16;
				num25 += num17;
				if (num24 >= 0 && num24 < num22 && num25 >= 0 && num25 < num23)
				{
					int num28 = num24 / 65536;
					int num29 = num25 / 65536;
					ushort num30 = (ushort)(32768 >> num28 % 16);
					ushort num31 = pMask.mask[num29 * num5 + num28 / 16];
					if ((num31 & num30) != 0)
					{
						num27 |= 0x400;
					}
				}
				num24 += num16;
				num25 += num17;
				if (num24 >= 0 && num24 < num22 && num25 >= 0 && num25 < num23)
				{
					int num28 = num24 / 65536;
					int num29 = num25 / 65536;
					ushort num30 = (ushort)(32768 >> num28 % 16);
					ushort num31 = pMask.mask[num29 * num5 + num28 / 16];
					if ((num31 & num30) != 0)
					{
						num27 |= 0x200;
					}
				}
				num24 += num16;
				num25 += num17;
				if (num24 >= 0 && num24 < num22 && num25 >= 0 && num25 < num23)
				{
					int num28 = num24 / 65536;
					int num29 = num25 / 65536;
					ushort num30 = (ushort)(32768 >> num28 % 16);
					ushort num31 = pMask.mask[num29 * num5 + num28 / 16];
					if ((num31 & num30) != 0)
					{
						num27 |= 0x100;
					}
				}
				num24 += num16;
				num25 += num17;
				if (num24 >= 0 && num24 < num22 && num25 >= 0 && num25 < num23)
				{
					int num28 = num24 / 65536;
					int num29 = num25 / 65536;
					ushort num30 = (ushort)(32768 >> num28 % 16);
					ushort num31 = pMask.mask[num29 * num5 + num28 / 16];
					if ((num31 & num30) != 0)
					{
						num27 |= 0x80;
					}
				}
				num24 += num16;
				num25 += num17;
				if (num24 >= 0 && num24 < num22 && num25 >= 0 && num25 < num23)
				{
					int num28 = num24 / 65536;
					int num29 = num25 / 65536;
					ushort num30 = (ushort)(32768 >> num28 % 16);
					ushort num31 = pMask.mask[num29 * num5 + num28 / 16];
					if ((num31 & num30) != 0)
					{
						num27 |= 0x40;
					}
				}
				num24 += num16;
				num25 += num17;
				if (num24 >= 0 && num24 < num22 && num25 >= 0 && num25 < num23)
				{
					int num28 = num24 / 65536;
					int num29 = num25 / 65536;
					ushort num30 = (ushort)(32768 >> num28 % 16);
					ushort num31 = pMask.mask[num29 * num5 + num28 / 16];
					if ((num31 & num30) != 0)
					{
						num27 |= 0x20;
					}
				}
				num24 += num16;
				num25 += num17;
				if (num24 >= 0 && num24 < num22 && num25 >= 0 && num25 < num23)
				{
					int num28 = num24 / 65536;
					int num29 = num25 / 65536;
					ushort num30 = (ushort)(32768 >> num28 % 16);
					ushort num31 = pMask.mask[num29 * num5 + num28 / 16];
					if ((num31 & num30) != 0)
					{
						num27 |= 0x10;
					}
				}
				num24 += num16;
				num25 += num17;
				if (num24 >= 0 && num24 < num22 && num25 >= 0 && num25 < num23)
				{
					int num28 = num24 / 65536;
					int num29 = num25 / 65536;
					ushort num30 = (ushort)(32768 >> num28 % 16);
					ushort num31 = pMask.mask[num29 * num5 + num28 / 16];
					if ((num31 & num30) != 0)
					{
						num27 |= 8;
					}
				}
				num24 += num16;
				num25 += num17;
				if (num24 >= 0 && num24 < num22 && num25 >= 0 && num25 < num23)
				{
					int num28 = num24 / 65536;
					int num29 = num25 / 65536;
					ushort num30 = (ushort)(32768 >> num28 % 16);
					ushort num31 = pMask.mask[num29 * num5 + num28 / 16];
					if ((num31 & num30) != 0)
					{
						num27 |= 4;
					}
				}
				num24 += num16;
				num25 += num17;
				if (num24 >= 0 && num24 < num22 && num25 >= 0 && num25 < num23)
				{
					int num28 = num24 / 65536;
					int num29 = num25 / 65536;
					ushort num30 = (ushort)(32768 >> num28 % 16);
					ushort num31 = pMask.mask[num29 * num5 + num28 / 16];
					if ((num31 & num30) != 0)
					{
						num27 |= 2;
					}
				}
				num24 += num16;
				num25 += num17;
				if (num24 >= 0 && num24 < num22 && num25 >= 0 && num25 < num23)
				{
					int num28 = num24 / 65536;
					int num29 = num25 / 65536;
					ushort num30 = (ushort)(32768 >> num28 % 16);
					ushort num31 = pMask.mask[num29 * num5 + num28 / 16];
					if ((num31 & num30) != 0)
					{
						num27 |= 1;
					}
				}
				num24 += num16;
				num25 += num17;
				mask[num26++] = num27;
			}
			if (num19 != 0)
			{
				ushort num32 = 32768;
				ushort num33 = 0;
				int j = 0;
				while (j < num19)
				{
					if (num24 >= 0 && num24 < num22 && num25 >= 0 && num25 < num23)
					{
						int num34 = num24 / 65536;
						int num35 = num25 / 65536;
						ushort num30 = (ushort)(32768 >> num34 % 16);
						ushort num31 = pMask.mask[num35 * num5 + num34 / 16];
						if ((num31 & num30) != 0)
						{
							num33 |= num32;
						}
					}
					num24 += num16;
					num25 += num17;
					j++;
					num32 = (ushort)((num32 >> 1) & 0x7FFF);
				}
				mask[num26] = num33;
			}
			num13 += num6;
			num14 -= num21;
			num15 += num20;
		}
		return true;
	}

	public bool testMask(int yBase1, int x1, int y1, CMask pMask2, int yBase2, int x2, int y2)
	{
		CMask cMask;
		CMask cMask2;
		int num;
		int num2;
		int num3;
		int num4;
		int num5;
		int num6;
		if (x1 <= x2)
		{
			cMask = this;
			cMask2 = pMask2;
			num = yBase1;
			num2 = yBase2;
			num3 = x1;
			num4 = y1;
			num5 = x2;
			num6 = y2;
		}
		else
		{
			cMask = pMask2;
			cMask2 = this;
			num = yBase2;
			num2 = yBase1;
			num3 = x2;
			num4 = y2;
			num5 = x1;
			num6 = y1;
		}
		int num7 = cMask.height - num;
		int num8 = cMask2.height - num2;
		if (num3 >= num5 + cMask2.width || num3 + cMask.width <= num5)
		{
			return false;
		}
		if (num4 >= num6 + num8 || num4 + num7 < num6)
		{
			return false;
		}
		int num9 = num5 - num3;
		int num10 = num9 / 16;
		int num11 = num9 % 16;
		int num12 = Math.Min(num3 + cMask.width - num5, cMask2.width);
		num12 = (num12 + 15) / 16;
		int num13;
		int num14;
		int num15;
		if (num4 <= num6)
		{
			num13 = num6 - num4 + num;
			num14 = num2;
			num15 = Math.Min(num4 + num7, num6 + num8) - num6;
		}
		else
		{
			num13 = num;
			num14 = num4 - num6 + num2;
			num15 = Math.Min(num4 + num7, num6 + num8) - num4;
		}
		if (num11 != 0)
		{
			switch (num12)
			{
			case 1:
			{
				for (int i = 0; i < num15; i++)
				{
					int num16 = (num13 + i) * cMask.lineWidth;
					int num17 = (num14 + i) * cMask2.lineWidth;
					int num18 = cMask.mask[num16 + num10] << num11;
					ushort num19 = (ushort)num18;
					if ((num19 & cMask2.mask[num17]) != 0)
					{
						return true;
					}
					if (num10 * 16 + 16 < cMask.width)
					{
						int num20 = (cMask.mask[num16 + num10 + 1] & 0xFFFF) << num11;
						num19 = (ushort)(num20 >> 16);
						if ((num19 & cMask2.mask[num17]) != 0)
						{
							return true;
						}
					}
				}
				break;
			}
			case 2:
			{
				for (int i = 0; i < num15; i++)
				{
					int num16 = (num13 + i) * cMask.lineWidth;
					int num17 = (num14 + i) * cMask2.lineWidth;
					int num18 = cMask.mask[num16 + num10] << num11;
					ushort num19 = (ushort)num18;
					if ((num19 & cMask2.mask[num17]) != 0)
					{
						return true;
					}
					int num20 = (cMask.mask[num16 + num10 + 1] & 0xFFFF) << num11;
					num19 = (ushort)(num20 >> 16);
					if ((num19 & cMask2.mask[num17]) != 0)
					{
						return true;
					}
					num19 = (ushort)num20;
					if ((num19 & cMask2.mask[num17 + 1]) != 0)
					{
						return true;
					}
					if (num10 + 2 < cMask.lineWidth)
					{
						num20 = cMask.mask[num16 + num10 + 2] << num11;
						num19 = (ushort)(num20 >> 16);
						if ((num19 & cMask2.mask[num17 + 1]) != 0)
						{
							return true;
						}
					}
				}
				break;
			}
			default:
			{
				for (int i = 0; i < num15; i++)
				{
					int num16 = (num13 + i) * cMask.lineWidth;
					int num17 = (num14 + i) * cMask2.lineWidth;
					int num18 = cMask.mask[num16 + num10] << num11;
					ushort num19 = (ushort)num18;
					if ((num19 & cMask2.mask[num17]) != 0)
					{
						return true;
					}
					int j;
					for (j = 0; j < num12 - 1; j++)
					{
						int num20 = (cMask.mask[num16 + num10 + j + 1] & 0xFFFF) << num11;
						num19 = (ushort)(num20 >> 16);
						if ((num19 & cMask2.mask[num17 + j]) != 0)
						{
							return true;
						}
						num19 = (ushort)num20;
						if ((num19 & cMask2.mask[num17 + j + 1]) != 0)
						{
							return true;
						}
					}
					if (num10 + j + 1 < cMask.lineWidth)
					{
						int num20 = cMask.mask[num16 + num10 + j + 1] << num11;
						num19 = (ushort)(num20 >> 16);
						if ((num19 & cMask2.mask[num17 + j]) != 0)
						{
							return true;
						}
					}
				}
				break;
			}
			}
		}
		else
		{
			for (int i = 0; i < num15; i++)
			{
				int num16 = (num13 + i) * cMask.lineWidth;
				int num17 = (num14 + i) * cMask2.lineWidth;
				for (int j = 0; j < num12; j++)
				{
					int num18 = cMask.mask[num16 + num10 + j];
					if ((cMask2.mask[num17 + j] & num18) != 0)
					{
						return true;
					}
				}
			}
		}
		return false;
	}

	public bool testRect(int yBase1, int xx, int yy, int w, int h)
	{
		int num = xx;
		if (num < 0)
		{
			w += num;
			num = 0;
		}
		int num2 = yy;
		if (yBase1 != 0 && num2 >= 0)
		{
			num2 = yBase1 + num2;
			h = height - num2;
		}
		if (num2 < 0)
		{
			h += num2;
			num2 = 0;
		}
		int num3 = num + w;
		if (num3 > width)
		{
			num3 = width;
		}
		int num4 = num2 + h;
		if (num4 > height)
		{
			num4 = height;
		}
		int num5 = num2 * lineWidth;
		int num6 = num4 - num2;
		int num7 = (num3 - num) / 16 + 1;
		int num8 = num / 16;
		for (int i = 0; i < num6; i++)
		{
			int num9 = i * lineWidth + num5;
			ushort num10;
			switch (num7)
			{
			case 1:
				num10 = (ushort)(lMask[num & 0xF] & rMask[(num3 - 1) & 0xF]);
				if ((mask[num9 + num8] & num10) != 0)
				{
					return true;
				}
				continue;
			case 2:
				num10 = lMask[num & 0xF];
				if ((mask[num9 + num8] & num10) != 0)
				{
					return true;
				}
				num10 = rMask[(num3 - 1) & 0xF];
				if ((mask[num9 + num8 + 1] & num10) != 0)
				{
					return true;
				}
				continue;
			}
			num10 = lMask[num & 0xF];
			if ((mask[num9 + num8] & num10) != 0)
			{
				return true;
			}
			int j;
			for (j = 1; j < num7 - 1; j++)
			{
				if (mask[num9 + num8 + 1] != 0)
				{
					return true;
				}
			}
			num10 = rMask[(num3 - 1) & 0xF];
			if ((mask[num9 + num8 + j] & num10) != 0)
			{
				return true;
			}
		}
		return false;
	}

	public bool testPoint(int x1, int y1)
	{
		if (x1 < 0 || x1 >= width || y1 < 0 || y1 >= height)
		{
			return false;
		}
		int num = y1 * lineWidth + x1 / 16;
		ushort num2 = (ushort)(32768 >> (x1 & 0xF));
		if ((mask[num] & num2) != 0)
		{
			return true;
		}
		return false;
	}
}
