using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RuntimeXNA.Banks;
using RuntimeXNA.Expressions;
using RuntimeXNA.OI;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Services;
using RuntimeXNA.Sprites;

namespace RuntimeXNA.Objects;

internal class CCounter : CObject, IDrawing
{
	public short rsFlags;

	public int rsMini;

	public int rsMaxi;

	public CValue rsValue;

	public int rsBoxCx;

	public int rsBoxCy;

	public double rsMiniDouble;

	public double rsMaxiDouble;

	public short rsOldFrame;

	public byte rsHidden;

	public short rsFont;

	public int rsColor1;

	public int rsColor2;

	public int displayFlags;

	public Texture2D texture;

	public CRect tempRc;

	public override void init(CObjectCommon ocPtr, CCreateObjectInfo cob)
	{
		rsFlags = 0;
		rsFont = -1;
		rsColor1 = 0;
		rsColor2 = 0;
		hoImgWidth = (hoImgHeight = 1);
		if (hoCommon.ocCounters == null)
		{
			hoImgWidth = (rsBoxCx = 1);
			hoImgHeight = (rsBoxCy = 1);
		}
		else
		{
			CDefCounters ocCounters = hoCommon.ocCounters;
			hoImgWidth = (rsBoxCx = ocCounters.odCx);
			hoImgHeight = (rsBoxCy = ocCounters.odCy);
			displayFlags = ocCounters.odDisplayFlags;
			switch (ocCounters.odDisplayType)
			{
			case 5:
				rsColor1 = ocCounters.ocColor1;
				break;
			case 2:
			case 3:
				rsColor1 = ocCounters.ocColor1;
				rsColor2 = ocCounters.ocColor2;
				break;
			}
		}
		CDefCounter cDefCounter = (CDefCounter)hoCommon.ocObject;
		rsMini = cDefCounter.ctMini;
		rsMaxi = cDefCounter.ctMaxi;
		rsMiniDouble = rsMini;
		rsMaxiDouble = rsMaxi;
		rsValue = new CValue(cDefCounter.ctInit);
		rsOldFrame = -1;
	}

	public override void handle()
	{
		ros.handle();
		if (roc.rcChanged)
		{
			roc.rcChanged = false;
			modif();
		}
	}

	public override void modif()
	{
		ros.modifRoutine();
	}

	public override void display()
	{
		ros.displayRoutine();
	}

	public override void getZoneInfos()
	{
		hoImgWidth = (hoImgHeight = 1);
		if (hoCommon.ocCounters == null)
		{
			return;
		}
		CDefCounters ocCounters = hoCommon.ocCounters;
		int num = 0;
		double num2 = 0.0;
		if (rsValue.getType() == 0)
		{
			num = rsValue.getInt();
		}
		else
		{
			num2 = rsValue.getDouble();
			num = (int)num2;
		}
		string text = "";
		switch (ocCounters.odDisplayType)
		{
		case 4:
		{
			int nFrames = ocCounters.nFrames;
			nFrames--;
			if (rsMaxi <= rsMini)
			{
				rsOldFrame = 0;
			}
			else
			{
				rsOldFrame = (short)Math.Min((num - rsMini) * nFrames / (rsMaxi - rsMini), ocCounters.nFrames - 1);
			}
			short num5 = ocCounters.frames[Math.Max(rsOldFrame - 1, 0)];
			CImage imageFromHandle = hoAdRunHeader.rhApp.imageBank.getImageFromHandle(num5);
			rsBoxCx = (hoImgWidth = imageFromHandle.width);
			rsBoxCy = (hoImgHeight = imageFromHandle.height);
			hoImgXSpot = imageFromHandle.xSpot;
			hoImgYSpot = imageFromHandle.ySpot;
			break;
		}
		case 2:
		case 3:
		{
			int nFrames = rsBoxCx;
			if (ocCounters.odDisplayType == 2)
			{
				nFrames = rsBoxCy;
			}
			if (rsMaxi <= rsMini)
			{
				rsOldFrame = 0;
			}
			else
			{
				rsOldFrame = (short)((num - rsMini) * nFrames / (rsMaxi - rsMini));
			}
			if (ocCounters.odDisplayType == 3)
			{
				hoImgYSpot = 0;
				hoImgHeight = rsBoxCy;
				hoImgWidth = rsOldFrame;
				if ((ocCounters.odDisplayFlags & 0x100) != 0)
				{
					hoImgXSpot = rsOldFrame - rsBoxCx;
				}
				else
				{
					hoImgXSpot = 0;
				}
			}
			else
			{
				hoImgXSpot = 0;
				hoImgWidth = rsBoxCx;
				hoImgHeight = rsOldFrame;
				if ((ocCounters.odDisplayFlags & 0x100) != 0)
				{
					hoImgYSpot = rsOldFrame - rsBoxCy;
				}
				else
				{
					hoImgYSpot = 0;
				}
			}
			break;
		}
		case 1:
		{
			text = ((rsValue.getType() != 0) ? CServices.doubleToString(num2, displayFlags) : CServices.intToString(num, displayFlags));
			int num6 = 0;
			int val = 0;
			foreach (char c in text)
			{
				short num5 = 0;
				switch (c)
				{
				case '-':
					num5 = ocCounters.frames[10];
					break;
				case ',':
				case '.':
					num5 = ocCounters.frames[12];
					break;
				case '+':
					num5 = ocCounters.frames[11];
					break;
				case 'E':
				case 'e':
					num5 = ocCounters.frames[13];
					break;
				case '0':
				case '1':
				case '2':
				case '3':
				case '4':
				case '5':
				case '6':
				case '7':
				case '8':
				case '9':
					num5 = ocCounters.frames[c - 48];
					break;
				}
				CImage imageFromHandle = hoAdRunHeader.rhApp.imageBank.getImageFromHandle(num5);
				num6 += imageFromHandle.width;
				val = Math.Max(val, imageFromHandle.height);
			}
			hoImgWidth = num6;
			hoImgHeight = val;
			hoImgXSpot = num6;
			hoImgYSpot = val;
			break;
		}
		case 5:
		{
			text = ((rsValue.getType() != 0) ? CServices.doubleToString(num2, displayFlags) : CServices.intToString(num, displayFlags));
			CRect cRect = new CRect();
			cRect.left = hoX - hoAdRunHeader.rhWindowX;
			cRect.top = hoY - hoAdRunHeader.rhWindowY;
			cRect.right = cRect.left + rsBoxCx;
			cRect.bottom = cRect.top + rsBoxCy;
			hoImgWidth = (short)(cRect.right - cRect.left);
			hoImgHeight = (short)(cRect.bottom - cRect.top);
			hoImgXSpot = (hoImgYSpot = 0);
			short odFont = rsFont;
			if (odFont == -1)
			{
				odFont = ocCounters.odFont;
			}
			CFont fontFromHandle = hoAdRunHeader.rhApp.fontBank.getFontFromHandle(odFont);
			int num3 = 0;
			short num4 = 38;
			int right = cRect.right;
			num3 = CServices.drawText(null, text, (short)(num4 | 0x400), cRect, 0, fontFromHandle, 0, 0);
			cRect.right = right;
			if (num3 != 0)
			{
				hoImgXSpot = (hoImgWidth = (short)(cRect.right - cRect.left));
				if (hoImgHeight < cRect.bottom - cRect.top)
				{
					hoImgHeight = (short)(cRect.bottom - cRect.top);
				}
				hoImgYSpot = hoImgHeight;
			}
			break;
		}
		default:
			hoImgWidth = (hoImgHeight = 1);
			break;
		}
	}

	public override void draw(SpriteBatchEffect batch)
	{
		if (hoCommon.ocCounters == null)
		{
			return;
		}
		CDefCounters ocCounters = hoCommon.ocCounters;
		int rsEffect = ros.rsEffect;
		int rsEffectParam = ros.rsEffectParam;
		int num = 0;
		double num2 = 0.0;
		if (rsValue.getType() == 0)
		{
			num = rsValue.getInt();
		}
		else
		{
			num2 = rsValue.getDouble();
			num = (int)num2;
		}
		string text = "";
		int rgb = rsColor1;
		int num3 = 0;
		switch (ocCounters.odDisplayType)
		{
		case 4:
			hoAdRunHeader.rhApp.spriteGen.pasteSpriteEffect(batch, ocCounters.frames[Math.Max(rsOldFrame - 1, 0)], hoRect.left, hoRect.top, 0, rsEffect, rsEffectParam);
			break;
		case 2:
		case 3:
		{
			int num4 = rsBoxCx;
			if (ocCounters.odDisplayType == 2)
			{
				num4 = rsBoxCy;
			}
			int width = hoRect.right - hoRect.left;
			int height = hoRect.bottom - hoRect.top;
			int left = hoRect.left;
			int top = hoRect.top;
			switch (ocCounters.ocFillType)
			{
			case 1:
			{
				Color color = CServices.getColor(rgb);
				hoAdRunHeader.rhApp.services.drawFilledRectangleSub(batch, left + hoAdRunHeader.rhApp.xOffset, top + hoAdRunHeader.rhApp.yOffset, width, height, color, 0, 0);
				break;
			}
			case 2:
				if (texture == null)
				{
					rgb = rsColor1;
					num3 = rsColor2;
					int num5 = CServices.getRValueJava(num3) - CServices.getRValueJava(rgb);
					int r = (num5 * rsOldFrame / num4 + CServices.getRValueJava(rgb)) & 0xFF;
					num5 = CServices.getGValueJava(num3) - CServices.getGValueJava(rgb);
					int g = (num5 * rsOldFrame / num4 + CServices.getGValueJava(rgb)) & 0xFF;
					num5 = CServices.getBValueJava(num3) - CServices.getBValueJava(rgb);
					int b = (num5 * rsOldFrame / num4 + CServices.getBValueJava(rgb)) & 0xFF;
					num3 = CServices.RGBJava(r, g, b);
					if ((ocCounters.odDisplayFlags & 0x100) != 0)
					{
						num5 = rgb;
						rgb = num3;
						num3 = num5;
					}
					bool bVertical = ocCounters.ocGradientFlags != 0;
					texture = CServices.createGradientRectangle(hoAdRunHeader.rhApp, width, height, rgb, num3, bVertical, 0, 0);
				}
				if (texture != null)
				{
					hoAdRunHeader.rhApp.tempRect.X = left + hoAdRunHeader.rhApp.xOffset;
					hoAdRunHeader.rhApp.tempRect.Y = top + hoAdRunHeader.rhApp.yOffset;
					hoAdRunHeader.rhApp.tempRect.Width = texture.Width;
					hoAdRunHeader.rhApp.tempRect.Height = texture.Height;
					batch.Draw(texture, hoAdRunHeader.rhApp.tempRect, null, Color.White);
				}
				break;
			}
			break;
		}
		case 1:
		{
			text = ((rsValue.getType() != 0) ? CServices.doubleToString(num2, displayFlags) : CServices.intToString(num, displayFlags));
			int left = hoRect.left;
			int top = hoRect.top;
			foreach (char c in text)
			{
				short iNum = 0;
				switch (c)
				{
				case '-':
					iNum = ocCounters.frames[10];
					break;
				case ',':
				case '.':
					iNum = ocCounters.frames[12];
					break;
				case '+':
					iNum = ocCounters.frames[11];
					break;
				case 'E':
				case 'e':
					iNum = ocCounters.frames[13];
					break;
				case '0':
				case '1':
				case '2':
				case '3':
				case '4':
				case '5':
				case '6':
				case '7':
				case '8':
				case '9':
					iNum = ocCounters.frames[c - 48];
					break;
				}
				hoAdRunHeader.rhApp.spriteGen.pasteSpriteEffect(batch, iNum, left, top, 0, rsEffect, rsEffectParam);
				CImage imageFromHandle = hoAdRunHeader.rhApp.imageBank.getImageFromHandle(iNum);
				left += imageFromHandle.width;
			}
			break;
		}
		case 5:
		{
			text = ((rsValue.getType() != 0) ? CServices.doubleToString(num2, displayFlags) : CServices.intToString(num, displayFlags));
			short odFont = rsFont;
			if (odFont == -1)
			{
				odFont = ocCounters.odFont;
			}
			CFont fontFromHandle = hoAdRunHeader.rhApp.fontBank.getFontFromHandle(odFont);
			short flags = 38;
			if (hoRect.bottom - hoRect.top != 0)
			{
				if (tempRc == null)
				{
					tempRc = new CRect();
				}
				tempRc.copyRect(hoRect);
				tempRc.offsetRect(hoAdRunHeader.rhApp.xOffset, hoAdRunHeader.rhApp.yOffset);
				CServices.drawText(batch, text, flags, tempRc, rsColor1, fontFromHandle, rsEffect, rsEffectParam);
			}
			break;
		}
		}
	}

	public void cpt_ToFloat(CValue pValue)
	{
		if (rsValue.getType() == 0)
		{
			if (pValue.getType() != 0)
			{
				rsValue.forceDouble(rsValue.getInt());
				display();
				roc.rcChanged = true;
			}
		}
		else
		{
			pValue.convertToDouble();
		}
	}

	public void cpt_Change(CValue pValue)
	{
		if (rsValue.getType() == 0)
		{
			int num = pValue.getInt();
			if (num < rsMini)
			{
				num = rsMini;
			}
			if (num > rsMaxi)
			{
				num = rsMaxi;
			}
			if (num != rsValue.getInt())
			{
				rsValue.forceInt(num);
				texture = null;
				modif();
			}
		}
		else
		{
			double num2 = pValue.getDouble();
			if (num2 < rsMiniDouble)
			{
				num2 = rsMiniDouble;
			}
			if (num2 > rsMaxiDouble)
			{
				num2 = rsMaxiDouble;
			}
			if (num2 != rsValue.getDouble())
			{
				rsValue.forceDouble(num2);
				texture = null;
				modif();
			}
		}
	}

	public void cpt_Add(CValue pValue)
	{
		cpt_ToFloat(pValue);
		CValue cValue = new CValue(rsValue);
		cValue.add(pValue);
		cpt_Change(cValue);
	}

	public void cpt_Sub(CValue pValue)
	{
		cpt_ToFloat(pValue);
		CValue cValue = new CValue(rsValue);
		cValue.sub(pValue);
		cpt_Change(cValue);
	}

	public void cpt_SetMin(CValue value)
	{
		rsMini = value.getInt();
		rsMiniDouble = value.getDouble();
		CValue pValue = new CValue(rsValue);
		cpt_Change(pValue);
	}

	public void cpt_SetMax(CValue value)
	{
		rsMaxi = value.getInt();
		rsMaxiDouble = value.getDouble();
		CValue pValue = new CValue(rsValue);
		cpt_Change(pValue);
	}

	public void cpt_SetColor1(int rgb)
	{
		rsColor1 = rgb;
		display();
		roc.rcChanged = true;
	}

	public void cpt_SetColor2(int rgb)
	{
		rsColor2 = rgb;
		display();
		roc.rcChanged = true;
	}

	public CValue cpt_GetValue()
	{
		return rsValue;
	}

	public CValue cpt_GetMin()
	{
		CValue cValue = new CValue();
		if (rsValue.type == 0)
		{
			cValue.forceInt(rsMini);
		}
		else
		{
			cValue.forceDouble(rsMiniDouble);
		}
		return cValue;
	}

	public CValue cpt_GetMax()
	{
		CValue cValue = new CValue();
		if (rsValue.type == 0)
		{
			cValue.forceInt(rsMaxi);
		}
		else
		{
			cValue.forceDouble(rsMaxiDouble);
		}
		return cValue;
	}

	public int cpt_GetColor1()
	{
		return rsColor1;
	}

	public int cpt_GetColor2()
	{
		return rsColor2;
	}

	public CFontInfo getFont()
	{
		CDefCounters ocCounters = hoCommon.ocCounters;
		if (ocCounters.odDisplayType == 5)
		{
			short odFont = rsFont;
			if (odFont == -1)
			{
				odFont = ocCounters.odFont;
			}
			return hoAdRunHeader.rhApp.fontBank.getFontInfoFromHandle(odFont);
		}
		return null;
	}

	public void setFont(CFontInfo info, CRect pRc)
	{
		CDefCounters ocCounters = hoCommon.ocCounters;
		if (ocCounters.odDisplayType == 5)
		{
			rsFont = hoAdRunHeader.rhApp.fontBank.addFont(info);
			if (pRc != null)
			{
				hoImgWidth = (rsBoxCx = pRc.right - pRc.left);
				hoImgHeight = (rsBoxCy = pRc.bottom - pRc.top);
			}
			modif();
			roc.rcChanged = true;
		}
	}

	public int getFontColor()
	{
		return rsColor1;
	}

	public void setFontColor(int rgb)
	{
		rsColor1 = rgb;
		modif();
		roc.rcChanged = true;
	}

	public override void drawableDraw(SpriteBatchEffect batch, CSprite sprite, CImageBank bank, int x, int y)
	{
		draw(batch);
	}

	public override void drawableKill()
	{
	}

	public override CMask drawableGetMask(int flags)
	{
		return null;
	}
}
