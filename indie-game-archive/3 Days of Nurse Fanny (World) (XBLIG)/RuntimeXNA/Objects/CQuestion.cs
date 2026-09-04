using System;
using RuntimeXNA.Banks;
using RuntimeXNA.OI;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Services;
using RuntimeXNA.Sprites;

namespace RuntimeXNA.Objects;

public class CQuestion : CObject
{
	private int rsBoxCx;

	private int rsBoxCy;

	private CRect[] rcA;

	private int currentDown;

	private int xMouse;

	private int yMouse;

	public override void init(CObjectCommon ocPtr, CCreateObjectInfo cob)
	{
	}

	public override void handle()
	{
		hoAdRunHeader.pause();
		hoAdRunHeader.questionObjectOn = this;
	}

	public void handleQuestion()
	{
		xMouse = hoAdRunHeader.rh2MouseX;
		yMouse = hoAdRunHeader.rh2MouseX;
		if (currentDown == 0)
		{
			if ((hoAdRunHeader.rh2MouseKeys & 1) != 0)
			{
				int question = getQuestion();
				if (question != 0)
				{
					currentDown = question;
				}
			}
		}
		else
		{
			if ((hoAdRunHeader.rh2MouseKeys & 1) != 0)
			{
				return;
			}
			if (getQuestion() == currentDown)
			{
				hoAdRunHeader.rhEvtProg.rhCurParam0 = currentDown;
				hoAdRunHeader.rhEvtProg.handle_Event(this, -5439484);
				CDefTexts cDefTexts = (CDefTexts)hoCommon.ocObject;
				CDefText cDefText = cDefTexts.otTexts[currentDown];
				if ((cDefText.tsFlags & 0x100) != 0)
				{
					hoAdRunHeader.rhEvtProg.handle_Event(this, -5308412);
				}
				else
				{
					hoAdRunHeader.rhEvtProg.handle_Event(this, -5373948);
				}
				hoAdRunHeader.questionObjectOn = null;
				hoAdRunHeader.resume();
				hoAdRunHeader.f_KillObject(hoNumber, bFast: true);
			}
			else
			{
				currentDown = 0;
			}
		}
	}

	public int getQuestion()
	{
		if (rcA != null)
		{
			for (int i = 1; i < rcA.Length; i++)
			{
				if (xMouse >= rcA[i].left && xMouse < rcA[i].right && yMouse > rcA[i].top && yMouse < rcA[i].bottom)
				{
					return i;
				}
			}
		}
		return 0;
	}

	public virtual void border3D(SpriteBatchEffect batch, CRect rc, bool state)
	{
		int rgb;
		int rgb2;
		if (state)
		{
			rgb = CServices.RGBJava(128, 128, 128);
			rgb2 = CServices.RGBJava(255, 255, 255);
		}
		else
		{
			rgb2 = CServices.RGBJava(128, 128, 128);
			rgb = CServices.RGBJava(255, 255, 255);
		}
		hoAdRunHeader.rhApp.services.drawRect(batch, rc, 0, 0, 0);
		CPoint[] array = new CPoint[3];
		for (int i = 0; i < 3; i++)
		{
			array[i] = new CPoint();
		}
		array[0].x = rc.right - 1;
		if (!state)
		{
			array[0].x--;
		}
		array[0].y = rc.top + 1;
		array[1].y = rc.top + 1;
		array[1].x = rc.left + 1;
		array[2].x = rc.left + 1;
		array[2].y = rc.bottom;
		if (!state)
		{
			array[2].y--;
		}
		hoAdRunHeader.rhApp.services.drawLine(hoAdRunHeader.rhApp.spriteBatch, array[0].x, array[0].y, array[1].x, array[1].y, rgb, 1, 0, 0);
		hoAdRunHeader.rhApp.services.drawLine(hoAdRunHeader.rhApp.spriteBatch, array[1].x, array[1].y, array[2].x, array[2].y, rgb, 1, 0, 0);
		if (!state)
		{
			array[0].x--;
		}
		array[0].y++;
		array[1].x++;
		array[1].y++;
		array[2].x++;
		if (!state)
		{
			array[2].y--;
		}
		hoAdRunHeader.rhApp.services.drawLine(hoAdRunHeader.rhApp.spriteBatch, array[0].x, array[0].y, array[1].x, array[1].y, rgb, 1, 0, 0);
		hoAdRunHeader.rhApp.services.drawLine(hoAdRunHeader.rhApp.spriteBatch, array[1].x, array[1].y, array[2].x, array[2].y, rgb, 1, 0, 0);
		if (!state)
		{
			array[0].x += 2;
			array[1].x = rc.right - 1;
			array[1].y = rc.bottom - 1;
			array[2].y = rc.bottom - 1;
			array[2].x--;
			hoAdRunHeader.rhApp.services.drawLine(hoAdRunHeader.rhApp.spriteBatch, array[0].x, array[0].y, array[1].x, array[1].y, rgb2, 1, 0, 0);
			hoAdRunHeader.rhApp.services.drawLine(hoAdRunHeader.rhApp.spriteBatch, array[1].x, array[1].y, array[2].x, array[2].y, rgb2, 1, 0, 0);
			array[0].x--;
			array[0].y++;
			array[1].x--;
			array[1].y--;
			array[2].x++;
			array[2].y--;
			hoAdRunHeader.rhApp.services.drawLine(hoAdRunHeader.rhApp.spriteBatch, array[0].x, array[0].y, array[1].x, array[1].y, rgb2, 1, 0, 0);
			hoAdRunHeader.rhApp.services.drawLine(hoAdRunHeader.rhApp.spriteBatch, array[1].x, array[1].y, array[2].x, array[2].y, rgb2, 1, 0, 0);
		}
	}

	public void redraw_Answer(SpriteBatchEffect batch, CDefText ptts, CRect lpRc, int color, bool flgRelief, CFont font, bool state)
	{
		CRect cRect = new CRect();
		cRect.copyRect(lpRc);
		border3D(batch, lpRc, state);
		cRect.left += 2;
		cRect.top += 2;
		cRect.right -= 4;
		cRect.bottom -= 4;
		if (state)
		{
			cRect.left += 2;
			cRect.top += 2;
		}
		if (flgRelief)
		{
			cRect.left += 2;
			cRect.top += 2;
			CServices.drawText(batch, ptts.tsText, 37, cRect, 16777215, font, 0, 0);
			cRect.left -= 2;
			cRect.top -= 2;
		}
		CServices.drawText(batch, ptts.tsText, 37, cRect, color, font, 0, 0);
	}

	public override void draw(SpriteBatchEffect batch)
	{
		CDefTexts cDefTexts = (CDefTexts)hoCommon.ocObject;
		CRun cRun = hoAdRunHeader;
		int num = hoX - cRun.rhWindowX;
		int num2 = hoY - cRun.rhWindowY;
		CDefText cDefText = cDefTexts.otTexts[1];
		int tsColor = cDefText.tsColor;
		bool flgRelief = (cDefText.tsFlags & 0x200) != 0;
		CFont fontFromHandle = cRun.rhApp.fontBank.getFontFromHandle(cDefText.tsFont);
		CRect cRect = new CRect();
		cRect.right = 2000;
		CServices.drawText(null, "X", 1024, cRect, tsColor, fontFromHandle, 0, 0);
		int num3 = cRect.right * 3 / 2;
		int num4 = 4;
		int num5 = 64;
		for (int i = 1; i < cDefTexts.otTexts.Length; i++)
		{
			cDefText = cDefTexts.otTexts[i];
			if (cDefText.tsText.Length > 0)
			{
				cRect.right = 2000;
				cRect.bottom = 0;
				CServices.drawText(null, cDefText.tsText, 1024, cRect, tsColor, fontFromHandle, 0, 0);
				num5 = Math.Max(num5, cRect.right + num3 * 2 + 4);
				num4 = Math.Max(num4, cRect.bottom * 3 / 2);
			}
		}
		int num6 = Math.Max(num4 / 4, 2);
		num5 += num3 * 2 + 4;
		CDefText cDefText2 = cDefTexts.otTexts[0];
		CFont fontFromHandle2 = cRun.rhApp.fontBank.getFontFromHandle(cDefText2.tsFont);
		cRect.right = 2000;
		cRect.bottom = 0;
		CServices.drawText(null, "X", 1024, cRect, tsColor, fontFromHandle2, 0, 0);
		int num7 = cRect.right * 3 / 2;
		cRect.right = 2000;
		cRect.bottom = 0;
		CServices.drawText(null, cDefText2.tsText, 1024, cRect, tsColor, fontFromHandle2, 0, 0);
		int num8 = cRect.bottom * 3 / 2;
		num5 = Math.Max(num5, cRect.right + num7 * 2 + 4);
		if (num5 > cRun.rhApp.gaCxWin)
		{
			num += (num5 - cRun.rhApp.gaCxWin) / 2;
			num5 = cRun.rhApp.gaCxWin;
		}
		else if (num5 > cRun.rhFrame.leWidth)
		{
			num += (num5 - cRun.rhFrame.leWidth) / 2;
			num5 = cRun.rhFrame.leWidth;
		}
		short num9 = 1;
		if (cRect.right + num7 * 2 + 4 > Math.Min(cRun.rhApp.gaCxWin, cRun.rhFrame.leWidth))
		{
			num9 = 0;
		}
		CRect cRect2 = new CRect();
		cRect2.left = num;
		cRect2.top = num2;
		rsBoxCx = num5;
		rsBoxCy = num8 + 1 + (num4 + num6) * (cDefTexts.otTexts.Length - 1) + num6 + 4;
		cRect2.right = num + rsBoxCx;
		cRect2.bottom = num2 + rsBoxCy;
		cRun.rhApp.services.fillRect(batch, cRect2, 12632256, 0, 0);
		border3D(batch, cRect2, state: false);
		cRect2.left += 2;
		cRect2.top += 2;
		cRect2.right -= 2;
		cRect2.bottom = cRect2.top + num8;
		if ((cDefText2.tsFlags & 0x200) != 0)
		{
			cRect2.left += 2;
			cRect2.top += 2;
			CServices.drawText(batch, cDefText2.tsText, (short)(0x20 | num9 | 4), cRect2, 16777215, fontFromHandle2, 0, 0);
			cRect2.left -= 2;
			cRect2.top -= 2;
		}
		CServices.drawText(batch, cDefText2.tsText, (short)(0x20 | num9 | 4), cRect2, cDefText2.tsColor, fontFromHandle2, 0, 0);
		cRect2.top = cRect2.bottom;
		cRun.rhApp.services.drawLine(hoAdRunHeader.rhApp.spriteBatch, cRect2.left, cRect2.top, cRect2.right, cRect2.bottom, 8421504, 1, 0, 0);
		cRect2.top++;
		cRect2.bottom++;
		cRun.rhApp.services.drawLine(hoAdRunHeader.rhApp.spriteBatch, cRect2.left, cRect2.top, cRect2.right, cRect2.bottom, 16777215, 1, 0, 0);
		if (rcA == null)
		{
			rcA = new CRect[cDefTexts.otTexts.Length];
			for (int i = 1; i < cDefTexts.otTexts.Length; i++)
			{
				rcA[i] = new CRect();
				rcA[i].left = num + 2 + num3;
				rcA[i].right = num + num5 - 2 - num3;
				rcA[i].top = num2 + 2 + num8 + 1 + num6 + (num4 + num6) * (i - 1);
				rcA[i].bottom = rcA[i].top + num4;
			}
		}
		for (int i = 1; i < cDefTexts.otTexts.Length; i++)
		{
			cDefText2 = cDefTexts.otTexts[i];
			bool state = currentDown == i;
			redraw_Answer(batch, cDefText2, rcA[i], tsColor, flgRelief, fontFromHandle, state);
		}
	}
}
