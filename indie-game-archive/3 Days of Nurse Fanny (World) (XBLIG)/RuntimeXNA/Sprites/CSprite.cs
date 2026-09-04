using System;
using Microsoft.Xna.Framework;
using RuntimeXNA.Banks;
using RuntimeXNA.Objects;
using RuntimeXNA.Services;

namespace RuntimeXNA.Sprites;

public class CSprite
{
	public const uint SF_RAMBO = 1u;

	public const uint SF_RECALCSURF = 2u;

	public const uint SF_PRIVATE = 4u;

	public const uint SF_INACTIF = 8u;

	public const uint SF_TOHIDE = 16u;

	public const uint SF_TOKILL = 32u;

	public const uint SF_REAF = 64u;

	public const uint SF_HIDDEN = 128u;

	public const uint SF_COLBOX = 256u;

	public const uint SF_NOSAVE = 512u;

	public const uint SF_FILLBACK = 1024u;

	public const uint SF_DISABLED = 2048u;

	public const uint SF_REAFINT = 4096u;

	public const uint SF_OWNERDRAW = 8192u;

	public const uint SF_OWNERSAVE = 16384u;

	public const uint SF_FADE = 32768u;

	public const uint SF_OBSTACLE = 65536u;

	public const uint SF_PLATFORM = 131072u;

	public const uint SF_BACKGROUND = 524288u;

	public const uint SF_SCALE_RESAMPLE = 1048576u;

	public const uint SF_ROTATE_ANTIA = 2097152u;

	public const uint SF_NOHOTSPOT = 4194304u;

	public const uint SF_OWNERCOLMASK = 8388608u;

	public const uint SF_UPDATECOLLIST = 268435456u;

	public const uint SF_TRUEOBJECT = 536870912u;

	public const int EFFECTFLAG_TRANSPARENT = 268435456;

	public const int EFFECTFLAG_ANTIALIAS = 536870912;

	public const int EFFECT_SEMITRANSP = 1;

	public CSprite objPrev;

	public CSprite objNext;

	public CImageBank bank;

	public uint sprFlags;

	public short sprLayer;

	public short sprAngle;

	public short sprAnglenew;

	public int sprZOrder;

	public int sprX;

	public int sprY;

	public int sprX1;

	public int sprY1;

	public int sprX2;

	public int sprY2;

	public int sprXnew;

	public int sprYnew;

	public int sprX1new;

	public int sprY1new;

	public int sprX2new;

	public int sprY2new;

	public int sprX1z;

	public int sprY1z;

	public int sprX2z;

	public int sprY2z;

	public float sprScaleX;

	public float sprScaleY;

	public float sprScaleXnew;

	public float sprScaleYnew;

	public short sprImg;

	public short sprImgNew;

	public IDrawing sprRout;

	public int sprEffect;

	public int sprEffectParam;

	public Color rgb = Color.White;

	public int sprBackColor;

	public CObject sprExtraInfo;

	public CSprite()
	{
	}

	public CSprite(CImageBank b)
	{
		bank = b;
	}

	public int getSpriteLayer()
	{
		return sprLayer / 2;
	}

	public uint getSpriteFlags()
	{
		return sprFlags;
	}

	public uint setSpriteFlags(uint dwNewFlags)
	{
		uint result = sprFlags;
		sprFlags = dwNewFlags;
		return result;
	}

	public uint setSpriteColFlag(uint colMode)
	{
		uint result = sprFlags & 1;
		sprFlags = (sprFlags & 0xFFFFFFFEu) | colMode;
		return result;
	}

	public float getSpriteScaleX()
	{
		return sprScaleX;
	}

	public float getSpriteScaleY()
	{
		return sprScaleY;
	}

	public bool getSpriteScaleResample()
	{
		return (sprFlags & 0x100000) != 0;
	}

	public int getSpriteAngle()
	{
		return sprAngle;
	}

	public bool getSpriteAngleAntiA()
	{
		return (sprFlags & 0x200000) != 0;
	}

	public CRect getSpriteRect()
	{
		CRect cRect = new CRect();
		cRect.left = sprX1new;
		cRect.right = sprX2new;
		cRect.top = sprY1new;
		cRect.bottom = sprY2new;
		return cRect;
	}

	public void updateBoundingBox()
	{
		CImage imageFromHandle = bank.getImageFromHandle(sprImgNew);
		if (imageFromHandle == null)
		{
			sprX1new = sprXnew;
			sprX2new = sprXnew + 1;
			sprY1new = sprYnew;
			sprY2new = sprYnew + 1;
			return;
		}
		int num = imageFromHandle.width;
		int num2 = imageFromHandle.height;
		int num3 = 0;
		int num4 = 0;
		if ((sprFlags & 0x400000) == 0)
		{
			num4 = imageFromHandle.ySpot;
			num3 = imageFromHandle.xSpot;
		}
		if (sprAngle == 0)
		{
			if (sprScaleXnew == 1f)
			{
				sprX1new = sprXnew - num3;
				sprX2new = sprX1new + num;
			}
			else
			{
				sprX1new = sprXnew - (int)((float)num3 * sprScaleXnew);
				sprX2new = sprX1new + (int)((float)num * sprScaleXnew);
			}
			if (sprScaleYnew == 1f)
			{
				sprY1new = sprYnew - num4;
				sprY2new = sprY1new + num2;
			}
			else
			{
				sprY1new = sprYnew - (int)((float)num4 * sprScaleYnew);
				sprY2new = sprY1new + (int)((float)num2 * sprScaleYnew);
			}
			return;
		}
		if (sprScaleXnew != 1f)
		{
			num3 = (int)((float)num3 * sprScaleXnew);
			num = (int)((float)num * sprScaleXnew);
		}
		if (sprScaleYnew != 1f)
		{
			num4 = (int)((float)num4 * sprScaleYnew);
			num2 = (int)((float)num2 * sprScaleYnew);
		}
		num--;
		num2--;
		int num9;
		int num10;
		int num5;
		int num7;
		int num8;
		int num6;
		if (sprAnglenew == 90)
		{
			num5 = num2;
			num6 = -num;
			num7 = 0;
			num8 = 0;
			num9 = num4;
			num10 = -num3;
		}
		else if (sprAnglenew == 180)
		{
			num5 = 0;
			num6 = 0;
			num7 = -num2;
			num8 = -num;
			num9 = -num3;
			num10 = -num4;
		}
		else if (sprAnglenew == 270)
		{
			num5 = -num2;
			num6 = num;
			num7 = 0;
			num8 = 0;
			num9 = -num4;
			num10 = num3;
		}
		else
		{
			double num11 = (double)sprAnglenew * Math.PI / 180.0;
			float num12 = (float)Math.Cos(num11);
			float num13 = (float)Math.Sin(num11);
			num9 = (int)((float)num3 * num12 + (float)num4 * num13);
			num10 = (int)((float)num4 * num12 - (float)num3 * num13);
			num5 = (int)((float)num2 * num13);
			num6 = -(int)((float)num * num13);
			num7 = (int)((float)num2 * num12);
			num8 = (int)((float)num * num12);
		}
		int num14 = num5 + num8;
		int num15 = num7 + num6;
		int val = sprXnew - num9;
		int val2 = sprYnew - num10;
		num5 += sprXnew - num9;
		num7 += sprYnew - num10;
		num14 += sprXnew - num9;
		num15 += sprYnew - num10;
		num8 += sprXnew - num9;
		num6 += sprYnew - num10;
		sprX1new = Math.Min(val, num5);
		sprX1new = Math.Min(sprX1new, num14);
		sprX1new = Math.Min(sprX1new, num8);
		sprX2new = Math.Max(val, num5);
		sprX2new = Math.Max(sprX2new, num14);
		sprX2new = Math.Max(sprX2new, num8);
		sprX2new++;
		sprY1new = Math.Min(val2, num7);
		sprY1new = Math.Min(sprY1new, num15);
		sprY1new = Math.Min(sprY1new, num6);
		sprY2new = Math.Max(val2, num7);
		sprY2new = Math.Max(sprY2new, num15);
		sprY2new = Math.Max(sprY2new, num6);
		sprY2new++;
	}

	public void calcBoundingBox(short newImg, int newX, int newY, int newAngle, float newScaleX, float newScaleY, CRect prc)
	{
		prc.left = (prc.top = (prc.right = (prc.bottom = 0)));
		CImage imageFromHandle = bank.getImageFromHandle(newImg);
		if (imageFromHandle == null)
		{
			return;
		}
		int num = imageFromHandle.width;
		int num2 = imageFromHandle.height;
		int num3 = 0;
		int num4 = 0;
		if ((sprFlags & 0x400000) == 0)
		{
			num4 = imageFromHandle.ySpot;
			num3 = imageFromHandle.xSpot;
		}
		if (newAngle == 0)
		{
			if (newScaleX == 1f)
			{
				prc.left = newX - num3;
				prc.right = prc.left + num;
			}
			else
			{
				prc.left = newX - (int)((float)num3 * newScaleX);
				prc.right = prc.left + (int)((float)num * newScaleX);
			}
			if (newScaleY == 1f)
			{
				prc.top = newY - num4;
				prc.bottom = prc.top + num2;
			}
			else
			{
				prc.top = newY - (int)((float)num4 * newScaleY);
				prc.bottom = prc.top + (int)((float)num2 * newScaleY);
			}
			return;
		}
		if (newScaleX != 1f)
		{
			num3 = (int)((float)num3 * newScaleX);
			num = (int)((float)num * newScaleX);
		}
		if (newScaleY != 1f)
		{
			num4 = (int)((float)num4 * newScaleY);
			num2 = (int)((float)num2 * newScaleY);
		}
		num--;
		num2--;
		int num8;
		int num9;
		int num10;
		int num12;
		int num13;
		int num11;
		switch (newAngle)
		{
		case 90:
			num10 = num2;
			num11 = -num;
			num12 = 0;
			num13 = 0;
			num8 = num4;
			num9 = -num3;
			break;
		case 180:
			num10 = 0;
			num11 = 0;
			num12 = -num2;
			num13 = -num;
			num8 = -num3;
			num9 = -num4;
			break;
		case 270:
			num10 = -num2;
			num11 = num;
			num12 = 0;
			num13 = 0;
			num8 = -num4;
			num9 = num3;
			break;
		default:
		{
			double num5 = (double)newAngle * Math.PI / 180.0;
			float num6 = (float)Math.Cos(num5);
			float num7 = (float)Math.Sin(num5);
			num8 = (int)((float)num3 * num6 + (float)num4 * num7);
			num9 = (int)((float)num4 * num6 - (float)num3 * num7);
			num10 = (int)((float)num2 * num7);
			num11 = -(int)((float)num * num7);
			num12 = (int)((float)num2 * num6);
			num13 = (int)((float)num * num6);
			break;
		}
		}
		int num14 = num10 + num13;
		int num15 = num12 + num11;
		int val = newX - num8;
		int val2 = newY - num9;
		num10 += newX - num8;
		num12 += newY - num9;
		num14 += newX - num8;
		num15 += newY - num9;
		num13 += newX - num8;
		num11 += newY - num9;
		prc.left = Math.Min(val, num10);
		prc.left = Math.Min(prc.left, num14);
		prc.left = Math.Min(prc.left, num13);
		prc.right = Math.Max(val, num10);
		prc.right = Math.Max(prc.right, num14);
		prc.right = Math.Max(prc.right, num13);
		prc.right++;
		prc.top = Math.Min(val2, num12);
		prc.top = Math.Min(prc.top, num15);
		prc.top = Math.Min(prc.top, num11);
		prc.bottom = Math.Max(val2, num12);
		prc.bottom = Math.Max(prc.bottom, num15);
		prc.bottom = Math.Max(prc.bottom, num11);
		prc.bottom++;
	}

	private void draw(SpriteBatchEffect batch)
	{
	}
}
