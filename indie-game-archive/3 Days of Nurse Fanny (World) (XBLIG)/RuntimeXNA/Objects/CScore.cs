using System;
using RuntimeXNA.Banks;
using RuntimeXNA.Expressions;
using RuntimeXNA.OI;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Services;
using RuntimeXNA.Sprites;

namespace RuntimeXNA.Objects;

internal class CScore : CObject, IDrawing
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
		int[] scores = hoAdRunHeader.rhApp.getScores();
		rsValue = new CValue(scores[rsPlayer - 1]);
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
		int value = rsValue.getInt();
		string text = CServices.intToString(value, displayFlags);
		switch (ocCounters.odDisplayType)
		{
		case 1:
		{
			int num3 = 0;
			int val = 0;
			foreach (char c in text)
			{
				short num4 = 0;
				switch (c)
				{
				case '-':
					num4 = ocCounters.frames[10];
					break;
				case '.':
					num4 = ocCounters.frames[12];
					break;
				case '+':
					num4 = ocCounters.frames[11];
					break;
				case 'E':
				case 'e':
					num4 = ocCounters.frames[13];
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
					num4 = ocCounters.frames[c - 48];
					break;
				}
				CImage imageFromHandle = hoAdRunHeader.rhApp.imageBank.getImageFromHandle(num4);
				num3 += imageFromHandle.width;
				val = Math.Max(val, imageFromHandle.height);
			}
			hoImgWidth = num3;
			hoImgHeight = val;
			hoImgXSpot = num3;
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
			int num = 0;
			short num2 = 38;
			int right = cRect.right;
			num = CServices.drawText(null, text, (short)(num2 | 0x400), cRect, 0, fontFromHandle, 0, 0);
			cRect.right = right;
			if (num != 0)
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
		int value = rsValue.getInt();
		string text = CServices.intToString(value, displayFlags);
		switch (ocCounters.odDisplayType)
		{
		case 1:
		{
			int num = hoRect.left;
			int top = hoRect.top;
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
				hoAdRunHeader.rhApp.spriteGen.pasteSpriteEffect(batch, iNum, num, top, 0, rsEffect, rsEffectParam);
				CImage imageFromHandle = hoAdRunHeader.rhApp.imageBank.getImageFromHandle(iNum);
				num += imageFromHandle.width;
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

	public override CMask drawableGetMask(int mask)
	{
		return null;
	}
}
