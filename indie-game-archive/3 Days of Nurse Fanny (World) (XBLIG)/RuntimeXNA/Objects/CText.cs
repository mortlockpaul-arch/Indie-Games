using RuntimeXNA.Banks;
using RuntimeXNA.OI;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Services;
using RuntimeXNA.Sprites;

namespace RuntimeXNA.Objects;

internal class CText : CObject, IDrawing
{
	public short rsFlag;

	public int rsBoxCx;

	public int rsBoxCy;

	public int rsMaxi;

	public int rsMini;

	public byte rsHidden;

	public string rsTextBuffer;

	public short rsFont;

	public int rsTextColor;

	public int deltaY;

	public override void init(CObjectCommon ocPtr, CCreateObjectInfo cob)
	{
		rsFlag = 0;
		CDefTexts cDefTexts = (CDefTexts)ocPtr.ocObject;
		hoImgWidth = cDefTexts.otCx;
		hoImgHeight = cDefTexts.otCy;
		rsBoxCx = cDefTexts.otCx;
		rsBoxCy = cDefTexts.otCy;
		rsMaxi = cDefTexts.otNumberOfText;
		rsTextColor = 0;
		if (cDefTexts.otTexts.Length > 0)
		{
			rsTextColor = cDefTexts.otTexts[0].tsColor;
		}
		rsHidden = (byte)cob.cobFlags;
		rsTextBuffer = null;
		rsFont = -1;
		rsMini = 0;
		if ((rsHidden & 4) != 0)
		{
			if (cDefTexts.otTexts.Length > 0)
			{
				rsTextBuffer = cDefTexts.otTexts[0].tsText;
			}
			else
			{
				rsTextBuffer = "";
			}
		}
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
		CDefTexts cDefTexts = (CDefTexts)hoCommon.ocObject;
		short tsFlags = cDefTexts.otTexts[0].tsFlags;
		CRect cRect = new CRect();
		cRect.left = hoX - hoAdRunHeader.rhWindowX + 1;
		cRect.top = hoY - hoAdRunHeader.rhWindowY;
		cRect.right = cRect.left + rsBoxCx;
		cRect.bottom = cRect.top + rsBoxCy;
		hoImgWidth = (short)(cRect.right - cRect.left);
		hoImgHeight = (short)(cRect.bottom - cRect.top);
		hoImgXSpot = 0;
		hoImgYSpot = 0;
		short tsFont = rsFont;
		if (tsFont == -1 && cDefTexts.otTexts.Length > 0)
		{
			tsFont = cDefTexts.otTexts[0].tsFont;
		}
		CFont fontFromHandle = hoAdRunHeader.rhApp.fontBank.getFontFromHandle(tsFont);
		string text;
		if (rsMini >= 0)
		{
			text = cDefTexts.otTexts[rsMini].tsText;
		}
		else
		{
			text = rsTextBuffer;
			if (text == null)
			{
				text = "";
			}
		}
		short num = (short)(tsFlags & 0x2F);
		int num2 = 0;
		int right = cRect.right;
		num2 = CServices.drawText(null, text, (short)(num | 0x400), cRect, rsTextColor, fontFromHandle, 0, 0);
		cRect.right = right;
		if (num2 != 0)
		{
			deltaY = 0;
			if ((num & 8) != 0)
			{
				deltaY = hoImgHeight - num2;
				return;
			}
			if ((num & 4) != 0)
			{
				deltaY = hoImgHeight / 2 - num2 / 2;
				return;
			}
			hoImgWidth = (short)(cRect.right - cRect.left);
			hoImgHeight = (short)(cRect.bottom - cRect.top);
		}
	}

	public override void draw(SpriteBatchEffect batch)
	{
		int effect = ros.rsEffect & 0xFFF;
		int rsEffectParam = ros.rsEffectParam;
		CDefTexts cDefTexts = (CDefTexts)hoCommon.ocObject;
		short tsFlags = cDefTexts.otTexts[0].tsFlags;
		short tsFont = rsFont;
		if (tsFont == -1 && cDefTexts.otTexts.Length > 0)
		{
			tsFont = cDefTexts.otTexts[0].tsFont;
		}
		CFont fontFromHandle = hoAdRunHeader.rhApp.fontBank.getFontFromHandle(tsFont);
		string text = null;
		if (rsMini >= 0)
		{
			text = cDefTexts.otTexts[rsMini].tsText;
		}
		else
		{
			text = rsTextBuffer;
			if (text == null)
			{
				text = "";
			}
		}
		CRect cRect = new CRect();
		cRect.copyRect(hoRect);
		cRect.offsetRect(hoAdRunHeader.rhApp.xOffset, hoAdRunHeader.rhApp.yOffset);
		cRect.top += deltaY;
		cRect.left++;
		short num = (short)(tsFlags & 0x2F);
		CServices.drawText(batch, text, (short)(num & -13), cRect, rsTextColor, fontFromHandle, effect, rsEffectParam);
	}

	public CFontInfo getFont()
	{
		short tsFont = rsFont;
		if (tsFont == -1)
		{
			CDefTexts cDefTexts = (CDefTexts)hoCommon.ocObject;
			tsFont = cDefTexts.otTexts[0].tsFont;
		}
		return hoAdRunHeader.rhApp.fontBank.getFontInfoFromHandle(tsFont);
	}

	public void setFont(CFontInfo info, CRect pRc)
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

	public int getFontColor()
	{
		return rsTextColor;
	}

	public void setFontColor(int rgb)
	{
		rsTextColor = rgb;
		modif();
		roc.rcChanged = true;
	}

	public bool txtChange(int num)
	{
		if (num < -1)
		{
			num = -1;
		}
		if (num >= rsMaxi)
		{
			num = rsMaxi - 1;
		}
		if (num == rsMini)
		{
			return false;
		}
		rsMini = num;
		if (num >= 0)
		{
			CDefTexts cDefTexts = (CDefTexts)hoCommon.ocObject;
			txtSetString(cDefTexts.otTexts[rsMini].tsText);
		}
		if ((ros.rsFlags & 1) != 0)
		{
			return false;
		}
		return true;
	}

	public void txtSetString(string s)
	{
		rsTextBuffer = s;
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
