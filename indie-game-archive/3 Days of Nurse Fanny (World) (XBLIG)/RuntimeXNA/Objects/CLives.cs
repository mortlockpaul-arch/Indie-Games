using System;
using RuntimeXNA.Banks;
using RuntimeXNA.Expressions;
using RuntimeXNA.OI;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Services;
using RuntimeXNA.Sprites;

namespace RuntimeXNA.Objects;

internal class CLives : CObject, IDrawing
{
	public short rsPlayer;

	public CValue rsValue;

	public int rsBoxCx;

	public int rsBoxCy;

	public short rsFont;

	public int rsColor1;

	public int displayFlags;

	public CRect tempRc;

	public override void init(CObjectCommon ocPtr, CCreateObjectInfo cob)
	{
		rsFont = -1;
		rsColor1 = 0;
		hoImgWidth = (hoImgHeight = 1);
		CDefCounters ocCounters = hoCommon.ocCounters;
		hoImgWidth = (rsBoxCx = ocCounters.odCx);
		hoImgHeight = (rsBoxCy = ocCounters.odCy);
		rsColor1 = ocCounters.ocColor1;
		rsPlayer = ocCounters.odPlayer;
		displayFlags = ocCounters.odDisplayFlags;
		rsValue = new CValue(hoAdRunHeader.rhApp.getLives()[rsPlayer - 1]);
	}

	public override void handle()
	{
		int[] lives = hoAdRunHeader.rhApp.getLives();
		if (rsPlayer > 0 && rsValue.getInt() != lives[rsPlayer - 1])
		{
			rsValue.forceInt(lives[rsPlayer - 1]);
			roc.rcChanged = true;
		}
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
		int num = rsValue.getInt();
		string text = CServices.intToString(num, displayFlags);
		switch (ocCounters.odDisplayType)
		{
		case 4:
			if (num != 0)
			{
				CImage imageFromHandle = hoAdRunHeader.rhApp.imageBank.getImageFromHandle(ocCounters.frames[0]);
				int num4 = num * imageFromHandle.width;
				if (num4 <= rsBoxCx)
				{
					hoImgWidth = (short)num4;
					hoImgHeight = imageFromHandle.height;
				}
				else
				{
					hoImgWidth = rsBoxCx;
					hoImgHeight = (rsBoxCx / imageFromHandle.width + num - 1) * imageFromHandle.height;
				}
			}
			else
			{
				hoImgWidth = (hoImgHeight = 1);
			}
			break;
		case 1:
		{
			int num5 = 0;
			int val = 0;
			foreach (char c in text)
			{
				short num6 = 0;
				switch (c)
				{
				case '-':
					num6 = ocCounters.frames[10];
					break;
				case '.':
					num6 = ocCounters.frames[12];
					break;
				case '+':
					num6 = ocCounters.frames[11];
					break;
				case 'E':
				case 'e':
					num6 = ocCounters.frames[13];
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
					num6 = ocCounters.frames[c - 48];
					break;
				}
				CImage imageFromHandle = hoAdRunHeader.rhApp.imageBank.getImageFromHandle(num6);
				num5 += imageFromHandle.width;
				val = Math.Max(val, imageFromHandle.height);
			}
			hoImgWidth = num5;
			hoImgHeight = val;
			hoImgXSpot = num5;
			hoImgYSpot = val;
			break;
		}
		case 5:
		{
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
			int num2 = 0;
			short num3 = 38;
			int right = cRect.right;
			num2 = CServices.drawText(null, text, (short)(num3 | 0x400), cRect, 0, fontFromHandle, 0, 0);
			cRect.right = right;
			if (num2 != 0)
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
		case 2:
		case 3:
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
		int num = rsValue.getInt();
		string text = CServices.intToString(num, displayFlags);
		switch (ocCounters.odDisplayType)
		{
		case 4:
		{
			if (num == 0)
			{
				break;
			}
			CImage imageFromHandle = hoAdRunHeader.rhApp.imageBank.getImageFromHandle(ocCounters.frames[0]);
			int left = hoRect.left;
			int top = hoRect.top;
			int right = hoRect.right;
			int bottom = hoRect.bottom;
			for (int i = top; i < bottom; i += imageFromHandle.height)
			{
				if (num <= 0)
				{
					break;
				}
				int num2 = left;
				while (num2 < right && num > 0)
				{
					hoAdRunHeader.rhApp.spriteGen.pasteSpriteEffect(batch, ocCounters.frames[0], num2, i, 0, rsEffect, rsEffectParam);
					num2 += imageFromHandle.width;
					num--;
				}
			}
			break;
		}
		case 1:
		{
			int num2 = hoRect.left;
			int i = hoRect.top;
			foreach (char c in text)
			{
				short iNum = 0;
				switch (c)
				{
				case '-':
					iNum = ocCounters.frames[10];
					break;
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
				hoAdRunHeader.rhApp.spriteGen.pasteSpriteEffect(batch, iNum, num2, i, 0, rsEffect, rsEffectParam);
				CImage imageFromHandle = hoAdRunHeader.rhApp.imageBank.getImageFromHandle(iNum);
				num2 += imageFromHandle.width;
			}
			break;
		}
		case 5:
		{
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
		case 2:
		case 3:
			break;
		}
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
