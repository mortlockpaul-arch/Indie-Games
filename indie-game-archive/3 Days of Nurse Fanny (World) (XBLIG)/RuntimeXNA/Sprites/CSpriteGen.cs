using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RuntimeXNA.Application;
using RuntimeXNA.Banks;
using RuntimeXNA.Objects;
using RuntimeXNA.Services;

namespace RuntimeXNA.Sprites;

public class CSpriteGen
{
	public const int AS_DEACTIVATE = 0;

	public const int AS_REDRAW = 1;

	public const int AS_ACTIVATE = 2;

	public const int AS_ENABLE = 4;

	public const int AS_DISABLE = 8;

	public const int AS_REDRAW_RECT = 32;

	public const int GS_BACKGROUND = 1;

	public const int GS_SAMELAYER = 2;

	public const short CM_BOX = 0;

	public const short CM_BITMAP = 1;

	public const short PSCF_CURRENTSURFACE = 1;

	public const short PSCF_TEMPSURFACE = 2;

	public const short LAYER_ALL = -1;

	public const int BOP_COPY = 0;

	public const int BOP_BLEND = 1;

	public const int BOP_INVERT = 2;

	public const int BOP_XOR = 3;

	public const int BOP_AND = 4;

	public const int BOP_OR = 5;

	public const int BOP_BLEND_REPLACETRANSP = 6;

	public const int BOP_DWROP = 7;

	public const int BOP_ANDNOT = 8;

	public const int BOP_ADD = 9;

	public const int BOP_MONO = 10;

	public const int BOP_SUB = 11;

	public const int BOP_BLEND_DONTREPLACECOLOR = 12;

	public const int BOP_EFFECTEX = 13;

	public const int BOP_MAX = 14;

	public const int EFFECTFLAG_TRANSPARENT = 268435456;

	public const int EFFECTFLAG_ANTIALIAS = 536870912;

	public const int BOP_MASK = 4095;

	public const int BOP_RGBAFILTER = 4096;

	public const int SCF_OBSTACLE = 1;

	public const int SCF_PLATFORM = 2;

	public const int SCF_EVENNOCOL = 4;

	public const int SCF_BACKGROUND = 8;

	public const int PSF_HOTSPOT = 1;

	public const int PSF_NOTRANSP = 2;

	public CSprite firstSprite;

	public CSprite lastSprite;

	private CRunFrame frame;

	private CRunApp app;

	private CImageBank bank;

	public short colMode;

	public Rectangle tempRect = default(Rectangle);

	public int xOffset;

	public int yOffset;

	private Vector2 vector = default(Vector2);

	private Color bColor = new Color(255, 255, 255, 255);

	public CSpriteGen()
	{
		firstSprite = null;
		lastSprite = null;
	}

	public void setData(CImageBank b, CRunApp a, CRunFrame f)
	{
		bank = b;
		frame = f;
		app = a;
	}

	public void setOffsets(int x, int y)
	{
		xOffset = x;
		yOffset = y;
	}

	public CSprite addSprite(int xSpr, int ySpr, short iSpr, short wLayer, int nZOrder, int backSpr, uint sFlags, CObject extraInfo)
	{
		CSprite cSprite = null;
		cSprite = winAllocSprite();
		cSprite.bank = bank;
		cSprite.sprFlags = sFlags | 0x40;
		cSprite.sprFlags &= 4294938589u;
		cSprite.sprLayer = (short)(wLayer * 2);
		if ((sFlags & 0x80000) == 0)
		{
			cSprite.sprLayer++;
		}
		cSprite.sprZOrder = nZOrder;
		cSprite.sprX = (cSprite.sprXnew = xSpr);
		cSprite.sprY = (cSprite.sprYnew = ySpr);
		cSprite.sprImg = (cSprite.sprImgNew = iSpr);
		cSprite.sprExtraInfo = extraInfo;
		cSprite.sprEffect = 268435456;
		cSprite.sprEffectParam = 0;
		cSprite.sprScaleX = (cSprite.sprScaleY = (cSprite.sprScaleXnew = (cSprite.sprScaleYnew = 1f)));
		cSprite.sprAngle = (cSprite.sprAnglenew = 0);
		cSprite.sprX1z = (cSprite.sprY1z = -1);
		cSprite.sprBackColor = 0;
		if ((sFlags & 0x400) != 0)
		{
			cSprite.sprBackColor = backSpr;
		}
		cSprite.updateBoundingBox();
		cSprite.sprX1 = cSprite.sprX1new;
		cSprite.sprY1 = cSprite.sprY1new;
		cSprite.sprX2 = cSprite.sprX2new;
		cSprite.sprY2 = cSprite.sprY2new;
		sortLastSprite(cSprite);
		return cSprite;
	}

	public CSprite addOwnerDrawSprite(int x1, int y1, int x2, int y2, short wLayer, int nZOrder, int backSpr, uint sFlags, CObject extraInfo, IDrawing sprProc)
	{
		CSprite cSprite = winAllocSprite();
		cSprite.sprX = (cSprite.sprXnew = x1);
		cSprite.sprY = (cSprite.sprYnew = y1);
		cSprite.sprX1new = (cSprite.sprX1 = x1);
		cSprite.sprY1new = (cSprite.sprY1 = y1);
		cSprite.sprX2new = (cSprite.sprX2 = x2);
		cSprite.sprY2new = (cSprite.sprY2 = y2);
		cSprite.sprX1z = (cSprite.sprY1z = -1);
		cSprite.sprLayer = (short)(wLayer * 2);
		if ((sFlags & 0x80000) == 0)
		{
			cSprite.sprLayer++;
		}
		cSprite.sprZOrder = nZOrder;
		cSprite.sprExtraInfo = extraInfo;
		cSprite.sprRout = sprProc;
		cSprite.sprFlags = sFlags | 0x2000;
		cSprite.sprFlags &= 4294963165u;
		cSprite.sprEffect = 268435456;
		cSprite.sprEffectParam = 0;
		cSprite.sprScaleX = (cSprite.sprScaleY = (cSprite.sprScaleXnew = (cSprite.sprScaleYnew = 1f)));
		cSprite.sprAngle = (cSprite.sprAnglenew = 0);
		cSprite.sprBackColor = 0;
		if ((sFlags & 0x400) != 0)
		{
			cSprite.sprBackColor = backSpr;
		}
		sortLastSprite(cSprite);
		return cSprite;
	}

	public CSprite modifSprite(CSprite ptSpr, int xSpr, int ySpr, short iSpr)
	{
		if (ptSpr != null && (ptSpr.sprXnew != xSpr || ptSpr.sprYnew != ySpr || ptSpr.sprImgNew != iSpr))
		{
			ptSpr.sprXnew = xSpr;
			ptSpr.sprYnew = ySpr;
			ptSpr.sprImgNew = iSpr;
			ptSpr.updateBoundingBox();
			ptSpr.sprFlags |= 64u;
		}
		return ptSpr;
	}

	public CSprite modifSpriteEx(CSprite ptSpr, int xSpr, int ySpr, short iSpr, float fScaleX, float fScaleY, bool bResample, int nAngle, bool bAntiA)
	{
		if (ptSpr != null)
		{
			if (fScaleX < 0f)
			{
				fScaleX = 0f;
			}
			if (fScaleY < 0f)
			{
				fScaleY = 0f;
			}
			nAngle %= 360;
			if (nAngle < 0)
			{
				nAngle += 360;
			}
			if (ptSpr.sprXnew != xSpr || ptSpr.sprYnew != ySpr || ptSpr.sprImgNew != iSpr || fScaleX != ptSpr.sprScaleX || fScaleY != ptSpr.sprScaleY || nAngle != ptSpr.sprAngle)
			{
				ptSpr.sprXnew = xSpr;
				ptSpr.sprYnew = ySpr;
				ptSpr.sprImgNew = iSpr;
				ptSpr.sprScaleXnew = fScaleX;
				ptSpr.sprScaleYnew = fScaleY;
				ptSpr.sprAnglenew = (short)nAngle;
				ptSpr.updateBoundingBox();
				ptSpr.sprFlags |= 64u;
			}
		}
		return ptSpr;
	}

	public CSprite modifSpriteEffect(CSprite ptSpr, int eff, int effectParam)
	{
		if (ptSpr != null)
		{
			ptSpr.sprEffect = eff & 0xFFF;
			ptSpr.sprEffectParam = effectParam;
			ptSpr.rgb = Color.White;
			float num = 1f;
			if ((eff & 0x1000) != 0)
			{
				ptSpr.rgb = CServices.getColorAlpha(effectParam & 0xFFFFFF);
				num = (float)((double)((effectParam >> 24) & 0xFF) / 255.0);
			}
			else if (ptSpr.sprEffect == 1)
			{
				num = (float)((double)(128 - ptSpr.sprEffectParam) / 128.0);
			}
			ptSpr.rgb *= num;
			ptSpr.sprFlags |= 64u;
		}
		return ptSpr;
	}

	public CSprite modifOwnerDrawSprite(CSprite ptSprModif, int x1, int y1, int x2, int y2)
	{
		if (ptSprModif != null)
		{
			ptSprModif.sprX1new = x1;
			ptSprModif.sprY1new = y1;
			ptSprModif.sprX2new = x2;
			ptSprModif.sprY2new = y2;
			ptSprModif.sprFlags |= 64u;
		}
		return ptSprModif;
	}

	public void setSpriteLayer(CSprite ptSpr, int nLayer)
	{
		if (ptSpr == null)
		{
			return;
		}
		int num = nLayer * 2;
		if ((ptSpr.sprFlags & 0x80000) == 0)
		{
			num++;
		}
		if (ptSpr.sprLayer == (short)num)
		{
			return;
		}
		int sprLayer = ptSpr.sprLayer;
		ptSpr.sprLayer = (short)num;
		CSprite objPrev;
		if (sprLayer < num)
		{
			if (lastSprite != null)
			{
				while (ptSpr != lastSprite)
				{
					CSprite objNext = ptSpr.objNext;
					if (objNext == null || objNext.sprLayer > (short)num)
					{
						break;
					}
					int sprZOrder = ptSpr.sprZOrder;
					int sprZOrder2 = objNext.sprZOrder;
					swapSprites(ptSpr, objNext);
					ptSpr.sprZOrder = sprZOrder;
					objNext.sprZOrder = sprZOrder2;
				}
			}
		}
		else if (firstSprite != null)
		{
			while (ptSpr != firstSprite)
			{
				objPrev = ptSpr.objPrev;
				if (objPrev == null || objPrev.sprLayer <= (short)num)
				{
					break;
				}
				int sprZOrder3 = ptSpr.sprZOrder;
				int sprZOrder4 = objPrev.sprZOrder;
				swapSprites(objPrev, ptSpr);
				ptSpr.sprZOrder = sprZOrder3;
				objPrev.sprZOrder = sprZOrder4;
			}
		}
		objPrev = ptSpr.objPrev;
		if (objPrev == null || objPrev.sprLayer != ptSpr.sprLayer)
		{
			ptSpr.sprZOrder = 1;
		}
		else
		{
			ptSpr.sprZOrder = objPrev.sprZOrder + 1;
		}
	}

	public void setSpriteScale(CSprite ptSpr, float fScaleX, float fScaleY, bool bResample)
	{
		if (ptSpr != null)
		{
			if (fScaleX < 0f)
			{
				fScaleX = 0f;
			}
			if (fScaleY < 0f)
			{
				fScaleY = 0f;
			}
			bool flag = (ptSpr.sprFlags & 0x100000) != 0;
			if (ptSpr.sprScaleX != fScaleX || ptSpr.sprScaleY != fScaleY || bResample != flag)
			{
				ptSpr.sprScaleXnew = fScaleX;
				ptSpr.sprScaleYnew = fScaleY;
				ptSpr.sprFlags |= 64u;
				ptSpr.sprFlags &= 4293918719u;
				ptSpr.updateBoundingBox();
			}
		}
	}

	public void setSpriteAngle(CSprite ptSpr, int nAngle, bool bAntiA)
	{
		if (ptSpr != null)
		{
			nAngle %= 360;
			if (nAngle < 0)
			{
				nAngle += 360;
			}
			if (ptSpr.sprAngle != nAngle)
			{
				ptSpr.sprAnglenew = (short)nAngle;
				ptSpr.sprFlags &= 4292870143u;
				ptSpr.sprFlags |= 64u;
				ptSpr.updateBoundingBox();
			}
		}
	}

	public void sortLastSprite(CSprite ptSprOrg)
	{
		short sprLayer = ptSprOrg.sprLayer;
		CSprite objPrev = ptSprOrg.objPrev;
		while (objPrev != null && sprLayer < objPrev.sprLayer)
		{
			CSprite objPrev2 = objPrev.objPrev;
			if (objPrev2 == null)
			{
				firstSprite = ptSprOrg;
			}
			else
			{
				objPrev2.objNext = ptSprOrg;
			}
			CSprite objNext = ptSprOrg.objNext;
			if (objNext == null)
			{
				lastSprite = objPrev;
			}
			else
			{
				objNext.objPrev = objPrev;
			}
			ptSprOrg.objPrev = objPrev.objPrev;
			objPrev.objPrev = ptSprOrg;
			objPrev.objNext = ptSprOrg.objNext;
			ptSprOrg.objNext = objPrev;
			objPrev = ptSprOrg;
			objPrev = objPrev.objPrev;
		}
		if (objPrev == null || sprLayer != objPrev.sprLayer)
		{
			return;
		}
		int sprZOrder = ptSprOrg.sprZOrder;
		while (objPrev != null && sprLayer == objPrev.sprLayer && sprZOrder < objPrev.sprZOrder)
		{
			CSprite objPrev2 = objPrev.objPrev;
			if (objPrev2 == null)
			{
				firstSprite = ptSprOrg;
			}
			else
			{
				objPrev2.objNext = ptSprOrg;
			}
			CSprite objNext = ptSprOrg.objNext;
			if (objNext == null)
			{
				lastSprite = objPrev;
			}
			else
			{
				objNext.objPrev = objPrev;
			}
			ptSprOrg.objPrev = objPrev.objPrev;
			objPrev.objPrev = ptSprOrg;
			objPrev.objNext = ptSprOrg.objNext;
			ptSprOrg.objNext = objPrev;
			objPrev = ptSprOrg;
			objPrev = objPrev.objPrev;
		}
	}

	public void swapSprites(CSprite sp1, CSprite sp2)
	{
		if (sp1 == sp2)
		{
			return;
		}
		CSprite objPrev = sp1.objPrev;
		CSprite objNext = sp1.objNext;
		CSprite objPrev2 = sp2.objPrev;
		CSprite objNext2 = sp2.objNext;
		int sprZOrder = sp1.sprZOrder;
		sp1.sprZOrder = sp2.sprZOrder;
		sp2.sprZOrder = sprZOrder;
		if (objNext == sp2)
		{
			if (objPrev != null)
			{
				objPrev.objNext = sp2;
			}
			sp2.objPrev = objPrev;
			sp2.objNext = sp1;
			sp1.objPrev = sp2;
			sp1.objNext = objNext2;
			if (objNext2 != null)
			{
				objNext2.objPrev = sp1;
			}
			if (objPrev == null)
			{
				firstSprite = sp2;
			}
			if (objNext2 == null)
			{
				lastSprite = sp1;
			}
			return;
		}
		if (objNext2 == sp1)
		{
			if (objPrev2 != null)
			{
				objPrev2.objNext = sp1;
			}
			sp1.objPrev = objPrev2;
			sp1.objNext = sp2;
			sp2.objPrev = sp1;
			sp2.objNext = objNext;
			if (objNext != null)
			{
				objNext.objPrev = sp2;
			}
			if (objPrev2 == null)
			{
				firstSprite = sp1;
			}
			if (objNext == null)
			{
				lastSprite = sp2;
			}
			return;
		}
		if (objPrev != null)
		{
			objPrev.objNext = sp2;
		}
		if (objNext != null)
		{
			objNext.objPrev = sp2;
		}
		sp1.objPrev = objPrev2;
		sp1.objNext = objNext2;
		if (objPrev2 != null)
		{
			objPrev2.objNext = sp1;
		}
		if (objNext2 != null)
		{
			objNext2.objPrev = sp1;
		}
		sp2.objPrev = objPrev;
		sp2.objNext = objNext;
		if (objPrev == null)
		{
			firstSprite = sp2;
		}
		if (objPrev2 == null)
		{
			firstSprite = sp1;
		}
		if (objNext == null)
		{
			lastSprite = sp2;
		}
		if (objNext2 == null)
		{
			lastSprite = sp1;
		}
	}

	public void moveSpriteToFront(CSprite pSpr)
	{
		if (lastSprite == null)
		{
			return;
		}
		int sprLayer = pSpr.sprLayer;
		while (pSpr != lastSprite)
		{
			CSprite objNext = pSpr.objNext;
			if (objNext == null || objNext.sprLayer > sprLayer)
			{
				break;
			}
			swapSprites(pSpr, objNext);
		}
	}

	public void moveSpriteToBack(CSprite pSpr)
	{
		if (lastSprite == null)
		{
			return;
		}
		int sprLayer = pSpr.sprLayer;
		while (pSpr != firstSprite)
		{
			CSprite objPrev = pSpr.objPrev;
			if (objPrev == null || objPrev.sprLayer < sprLayer)
			{
				break;
			}
			swapSprites(objPrev, pSpr);
		}
	}

	public void moveSpriteBefore(CSprite pSprToMove, CSprite pSprDest)
	{
		if (pSprToMove.sprLayer != pSprDest.sprLayer)
		{
			return;
		}
		CSprite objPrev = pSprToMove.objPrev;
		while (objPrev != null && objPrev != pSprDest)
		{
			objPrev = objPrev.objPrev;
		}
		if (objPrev == null)
		{
			return;
		}
		CSprite cSprite = pSprToMove;
		do
		{
			cSprite = pSprToMove.objPrev;
			if (cSprite == null)
			{
				break;
			}
			swapSprites(pSprToMove, cSprite);
		}
		while (cSprite != pSprDest);
	}

	public void moveSpriteAfter(CSprite pSprToMove, CSprite pSprDest)
	{
		if (pSprToMove.sprLayer != pSprDest.sprLayer)
		{
			return;
		}
		CSprite objNext = pSprToMove.objNext;
		while (objNext != null && objNext != pSprDest)
		{
			objNext = objNext.objNext;
		}
		if (objNext == null)
		{
			return;
		}
		CSprite objNext2;
		do
		{
			objNext2 = pSprToMove.objNext;
			if (objNext2 == null)
			{
				break;
			}
			swapSprites(pSprToMove, objNext2);
		}
		while (objNext2 != pSprDest);
	}

	public bool isSpriteBefore(CSprite pSpr, CSprite pSprDest)
	{
		if (pSpr.sprLayer < pSprDest.sprLayer)
		{
			return true;
		}
		if (pSpr.sprLayer > pSprDest.sprLayer)
		{
			return false;
		}
		if (pSpr.sprZOrder < pSprDest.sprZOrder)
		{
			return true;
		}
		return false;
	}

	public bool isSpriteAfter(CSprite pSpr, CSprite pSprDest)
	{
		if (pSpr.sprLayer > pSprDest.sprLayer)
		{
			return true;
		}
		if (pSpr.sprLayer < pSprDest.sprLayer)
		{
			return false;
		}
		if (pSpr.sprZOrder > pSprDest.sprZOrder)
		{
			return true;
		}
		return false;
	}

	public CSprite getFirstSprite(int nLayer, int dwFlags)
	{
		CSprite cSprite = null;
		cSprite = firstSprite;
		int num = nLayer;
		if (nLayer != -1)
		{
			num *= 2;
			if ((dwFlags & 1) == 0)
			{
				num++;
			}
		}
		while (cSprite != null && num != -1 && cSprite.sprLayer != num)
		{
			if (cSprite.sprLayer > num)
			{
				cSprite = null;
				break;
			}
			cSprite = cSprite.objNext;
		}
		return cSprite;
	}

	public CSprite getNextSprite(CSprite pSpr, int dwFlags)
	{
		if (pSpr != null)
		{
			int sprLayer = pSpr.sprLayer;
			if ((dwFlags & 1) != 0)
			{
				while ((pSpr = pSpr.objNext) != null)
				{
					if ((pSpr.sprFlags & 0x80000) == 0)
					{
						if ((dwFlags & 2) != 0)
						{
							pSpr = null;
							break;
						}
						continue;
					}
					if ((dwFlags & 2) != 0 && pSpr.sprLayer != sprLayer)
					{
						pSpr = null;
					}
					break;
				}
			}
			else
			{
				while ((pSpr = pSpr.objNext) != null)
				{
					if ((pSpr.sprFlags & 0x80000) != 0)
					{
						if ((dwFlags & 2) != 0)
						{
							pSpr = null;
							break;
						}
						continue;
					}
					if ((dwFlags & 2) != 0 && pSpr.sprLayer != sprLayer)
					{
						pSpr = null;
					}
					break;
				}
			}
		}
		return pSpr;
	}

	public CSprite getPrevSprite(CSprite pSpr, int dwFlags)
	{
		if (pSpr != null)
		{
			int sprLayer = pSpr.sprLayer;
			if ((dwFlags & 1) != 0)
			{
				while ((pSpr = pSpr.objPrev) != null)
				{
					if ((pSpr.sprFlags & 0x80000) == 0)
					{
						if ((dwFlags & 2) != 0)
						{
							pSpr = null;
							break;
						}
						continue;
					}
					if ((dwFlags & 2) != 0 && pSpr.sprLayer != sprLayer)
					{
						pSpr = null;
					}
					break;
				}
			}
			else
			{
				while ((pSpr = pSpr.objPrev) != null)
				{
					if ((pSpr.sprFlags & 0x80000) != 0)
					{
						if ((dwFlags & 2) != 0)
						{
							pSpr = null;
							break;
						}
						continue;
					}
					if ((dwFlags & 2) != 0 && pSpr.sprLayer != sprLayer)
					{
						pSpr = null;
					}
					break;
				}
			}
		}
		return pSpr;
	}

	public void showSprite(CSprite ptSpr, bool showFlag)
	{
		if (ptSpr == null)
		{
			return;
		}
		if (showFlag)
		{
			if ((ptSpr.sprFlags & 0x80) != 0)
			{
				ptSpr.sprFlags &= 4294967167u;
				ptSpr.sprFlags |= 64u;
			}
		}
		else if ((ptSpr.sprFlags & 0x80) == 0)
		{
			ptSpr.sprFlags |= 128u;
			ptSpr.sprFlags |= 64u;
		}
	}

	public void killSprite(CSprite ptSprToKill)
	{
		if ((ptSprToKill.sprFlags & 0x2000) != 0)
		{
			ptSprToKill.sprRout.drawableKill();
		}
		winFreeSprite(ptSprToKill);
	}

	public void activeSprite(CSprite ptSpr, int activeFlag, CRect reafRect)
	{
		if (ptSpr != null)
		{
			switch (activeFlag)
			{
			case 0:
				ptSpr.sprFlags |= 72u;
				break;
			case 1:
				ptSpr.sprFlags |= 64u;
				break;
			case 2:
				ptSpr.sprFlags &= 4294967287u;
				break;
			case 4:
				ptSpr.sprFlags &= 4294965247u;
				break;
			case 8:
				ptSpr.sprFlags |= 2048u;
				break;
			case 3:
			case 5:
			case 6:
			case 7:
				break;
			}
			return;
		}
		for (ptSpr = firstSprite; ptSpr != null; ptSpr = ptSpr.objNext)
		{
			switch (activeFlag)
			{
			case 0:
				ptSpr.sprFlags |= 72u;
				break;
			case 1:
				ptSpr.sprFlags |= 64u;
				break;
			case 17:
				if ((ptSpr.sprFlags & 0x80080) == 0)
				{
					ptSpr.sprFlags |= 64u;
				}
				break;
			case 2:
				ptSpr.sprFlags &= 4294967287u;
				break;
			case 4:
				ptSpr.sprFlags &= 4294965247u;
				break;
			case 8:
				ptSpr.sprFlags |= 2048u;
				break;
			default:
				ptSpr.sprFlags &= 4294967287u;
				break;
			}
		}
	}

	public void delSprite(CSprite ptSprToDel)
	{
		killSprite(ptSprToDel);
	}

	public void delSpriteFast(CSprite ptSpr)
	{
		killSprite(ptSpr);
	}

	public CMask getSpriteMask(CSprite ptSpr, short newImg, int nFlags, int newAngle, float newScaleX, float newScaleY)
	{
		if (ptSpr != null)
		{
			if ((ptSpr.sprFlags & 0x2000) != 0)
			{
				return ptSpr.sprRout.drawableGetMask(nFlags);
			}
			short num = newImg;
			if (num == -1)
			{
				num = ptSpr.sprImg;
			}
			if (num != -1)
			{
				CImage imageFromHandle = bank.getImageFromHandle(num);
				return imageFromHandle.getMask(nFlags, newAngle, newScaleX, newScaleY);
			}
		}
		return null;
	}

	public void spriteUpdate()
	{
		for (CSprite objNext = firstSprite; objNext != null; objNext = objNext.objNext)
		{
			if ((objNext.sprFlags & 0x40) != 0)
			{
				objNext.sprX = objNext.sprXnew;
				objNext.sprY = objNext.sprYnew;
				objNext.sprX1 = objNext.sprX1new;
				objNext.sprY1 = objNext.sprY1new;
				objNext.sprX2 = objNext.sprX2new;
				objNext.sprY2 = objNext.sprY2new;
				objNext.sprScaleX = objNext.sprScaleXnew;
				objNext.sprScaleY = objNext.sprScaleYnew;
				objNext.sprAngle = objNext.sprAnglenew;
				if ((objNext.sprFlags & 0x2000) == 0)
				{
					objNext.sprImg = objNext.sprImgNew;
				}
			}
		}
	}

	public void pasteSpriteEffect(SpriteBatchEffect batch, short iNum, int iX, int iY, int flags, int effect, int effectParam)
	{
		CImage imageFromHandle = bank.getImageFromHandle(iNum);
		if (imageFromHandle != null)
		{
			int num = iX;
			if ((flags & 1) != 0)
			{
				num -= imageFromHandle.xSpot;
			}
			int num2 = iY;
			if ((flags & 1) != 0)
			{
				num2 -= imageFromHandle.ySpot;
			}
			tempRect.X = num + xOffset;
			tempRect.Y = num2 + yOffset;
			tempRect.Width = imageFromHandle.width;
			tempRect.Height = imageFromHandle.height;
			Color color = Color.White;
			float num3 = 1f;
			if ((effect & 0x1000) != 0)
			{
				color = CServices.getColorAlpha(effectParam & 0xFFFFFF);
				num3 = (float)((double)((effectParam >> 24) & 0xFF) / 255.0);
			}
			else if ((effect & 0xFFF) == 1)
			{
				num3 = (float)((double)(128 - effectParam) / 128.0);
			}
			color *= num3;
			Texture2D texture = imageFromHandle.image;
			Rectangle? sourceRectangle = null;
			if (imageFromHandle.mosaic != 0)
			{
				texture = app.imageBank.mosaics[imageFromHandle.mosaic];
				sourceRectangle = imageFromHandle.mosaicRectangle;
			}
			batch.Draw(texture, tempRect, sourceRectangle, color, effect & 0xFFF, effectParam);
		}
	}

	public void spriteDraw(SpriteBatchEffect batch)
	{
		CSprite cSprite = firstSprite;
		if (cSprite == null)
		{
			app.run.draw_QuickDisplay(batch);
			return;
		}
		bool flag = true;
		for (CSprite cSprite2 = cSprite; cSprite2 != null; cSprite2 = cSprite2.objNext)
		{
			if (flag && (cSprite2.sprFlags & 0x20000000) != 0)
			{
				app.run.draw_QuickDisplay(batch);
				flag = false;
			}
			if ((cSprite2.sprFlags & 0x880) == 0)
			{
				if ((cSprite2.sprFlags & 0x2000) != 0 && cSprite2.sprRout != null)
				{
					cSprite2.sprRout.drawableDraw(batch, cSprite2, bank, cSprite2.sprX1 + xOffset, cSprite2.sprY1 + yOffset);
				}
				else
				{
					CImage imageFromHandle = bank.getImageFromHandle(cSprite2.sprImg);
					if (imageFromHandle != null)
					{
						int num = 0;
						int num2 = 0;
						if ((cSprite2.sprFlags & 0x400000) == 0)
						{
							num = imageFromHandle.xSpot;
							num2 = imageFromHandle.ySpot;
						}
						tempRect.X = cSprite2.sprX + xOffset;
						tempRect.Y = cSprite2.sprY + yOffset;
						tempRect.Width = (int)((float)imageFromHandle.width * cSprite2.sprScaleX);
						tempRect.Height = (int)((float)imageFromHandle.height * cSprite2.sprScaleY);
						vector.X = num;
						vector.Y = num2;
						Texture2D texture = imageFromHandle.image;
						Rectangle? sourceRectangle = null;
						if (imageFromHandle.mosaic != 0)
						{
							texture = app.imageBank.mosaics[imageFromHandle.mosaic];
							sourceRectangle = imageFromHandle.mosaicRectangle;
						}
						batch.Draw(texture, tempRect, sourceRectangle, cSprite2.rgb, (float)((double)(-cSprite2.sprAngle) * Math.PI / 180.0), vector, cSprite2.sprEffect);
					}
				}
				cSprite2.sprFlags &= 4294963135u;
			}
		}
		if (flag)
		{
			app.run.draw_QuickDisplay(batch);
		}
	}

	public CSprite getLastSprite(int nLayer, int dwFlags)
	{
		CSprite cSprite = lastSprite;
		int num = nLayer;
		if (nLayer != -1)
		{
			num *= 2;
			if ((dwFlags & 1) == 0)
			{
				num++;
			}
		}
		while (cSprite != null && num != -1 && cSprite.sprLayer != num)
		{
			if (cSprite.sprLayer < num)
			{
				cSprite = null;
				break;
			}
			cSprite = cSprite.objPrev;
		}
		return cSprite;
	}

	public CSprite winAllocSprite()
	{
		CSprite cSprite = new CSprite(bank);
		if (firstSprite == null)
		{
			firstSprite = cSprite;
			lastSprite = cSprite;
			cSprite.objPrev = null;
			cSprite.objNext = null;
			return cSprite;
		}
		CSprite cSprite2 = lastSprite;
		cSprite2.objNext = cSprite;
		cSprite.objPrev = cSprite2;
		cSprite.objNext = null;
		lastSprite = cSprite;
		return cSprite;
	}

	public void winFreeSprite(CSprite spr)
	{
		if (spr.objPrev == null)
		{
			firstSprite = spr.objNext;
		}
		else
		{
			spr.objPrev.objNext = spr.objNext;
		}
		if (spr.objNext != null)
		{
			spr.objNext.objPrev = spr.objPrev;
		}
		else
		{
			lastSprite = spr.objPrev;
		}
	}

	public void winSetColMode(short c)
	{
		colMode = c;
	}

	public CSprite spriteCol_TestPoint(CSprite firstSpr, short nLayer, int xp, int yp, int dwFlags)
	{
		CSprite cSprite = firstSpr;
		cSprite = ((cSprite != null) ? cSprite.objNext : firstSprite);
		bool flag = nLayer == -1;
		bool flag2 = (dwFlags & 4) != 0;
		short num;
		if ((dwFlags & 8) != 0)
		{
			num = 0;
			if (nLayer != -1)
			{
				nLayer *= 2;
			}
		}
		else
		{
			num = 1;
			if (nLayer != -1)
			{
				nLayer = (short)(nLayer * 2 + 1);
			}
		}
		for (; cSprite != null; cSprite = cSprite.objNext)
		{
			if (!flag)
			{
				if (cSprite.sprLayer < nLayer)
				{
					continue;
				}
				if (cSprite.sprLayer > nLayer)
				{
					break;
				}
			}
			else if ((cSprite.sprLayer & 1) != num)
			{
				continue;
			}
			if ((!flag2 && (cSprite.sprFlags & 1) == 0) || xp < cSprite.sprX1 || xp >= cSprite.sprX2 || yp < cSprite.sprY1 || yp >= cSprite.sprY2)
			{
				continue;
			}
			int nFlags = 0;
			if ((dwFlags & 8) != 0 && (cSprite.sprFlags & 0x20000) != 0)
			{
				if ((dwFlags & 1) != 0)
				{
					continue;
				}
				nFlags = 1;
			}
			if (colMode == 0 || (cSprite.sprFlags & 0x100) != 0)
			{
				return cSprite;
			}
			CMask spriteMask = getSpriteMask(cSprite, -1, nFlags, cSprite.sprAngle, cSprite.sprScaleX, cSprite.sprScaleY);
			if (spriteMask == null)
			{
				continue;
			}
			int num2 = yp - cSprite.sprY1;
			if (num2 >= spriteMask.height)
			{
				continue;
			}
			int num3 = num2 * spriteMask.lineWidth;
			int num4 = xp - cSprite.sprX1;
			if (num4 < spriteMask.width)
			{
				num3 += num4 / 16;
				short num5 = (short)(32768 >> (num4 & 0xF));
				if ((spriteMask.mask[num3] & num5) != 0)
				{
					return cSprite;
				}
			}
		}
		return null;
	}

	public CSprite spriteCol_TestPointOne(CSprite firstSpr, short nLayer, int xp, int yp, int dwFlags)
	{
		CSprite cSprite = firstSpr;
		bool flag = nLayer == -1;
		bool flag2 = true;
		short num;
		if ((dwFlags & 8) != 0)
		{
			num = 0;
			if (nLayer != -1)
			{
				nLayer *= 2;
			}
		}
		else
		{
			num = 1;
			if (nLayer != -1)
			{
				nLayer = (short)(nLayer * 2 + 1);
			}
		}
		for (; cSprite != null; cSprite = cSprite.objNext)
		{
			if (!flag)
			{
				if (cSprite.sprLayer < nLayer)
				{
					continue;
				}
				if (cSprite.sprLayer > nLayer)
				{
					break;
				}
			}
			else if ((cSprite.sprLayer & 1) != num)
			{
				continue;
			}
			if ((!flag2 && (cSprite.sprFlags & 1) == 0) || xp < cSprite.sprX1 || xp >= cSprite.sprX2 || yp < cSprite.sprY1 || yp >= cSprite.sprY2)
			{
				continue;
			}
			int nFlags = 0;
			if (colMode == 0 || (cSprite.sprFlags & 0x100) != 0)
			{
				return cSprite;
			}
			CMask spriteMask = getSpriteMask(cSprite, -1, nFlags, cSprite.sprAngle, cSprite.sprScaleX, cSprite.sprScaleY);
			if (spriteMask == null)
			{
				continue;
			}
			int num2 = yp - cSprite.sprY1;
			if (num2 >= spriteMask.height)
			{
				continue;
			}
			int num3 = num2 * spriteMask.lineWidth;
			int num4 = xp - cSprite.sprX1;
			if (num4 < spriteMask.width)
			{
				num3 += num4 / 16;
				short num5 = (short)(32768 >> (num4 & 0xF));
				if ((spriteMask.mask[num3] & num5) != 0)
				{
					return cSprite;
				}
			}
		}
		return null;
	}

	public CArrayList spriteCol_TestSprite_All(CSprite ptSpr, short newImg, int newX, int newY, int newAngle, float newScaleX, float newScaleY, int dwFlags)
	{
		int num = colMode;
		CArrayList cArrayList = null;
		if (ptSpr == null || newImg < 0)
		{
			return null;
		}
		if ((ptSpr.sprFlags & 0x100) != 0)
		{
			num = 0;
		}
		int sprLayer = ptSpr.sprLayer;
		if ((dwFlags & 8) != 0)
		{
			sprLayer &= -2;
		}
		else
		{
			if ((ptSpr.sprFlags & 1) == 0)
			{
				return null;
			}
			sprLayer |= 1;
		}
		int num2 = newX;
		int num3 = newY;
		int num4 = num2;
		int num5 = num3;
		CMask cMask = null;
		if ((ptSpr.sprFlags & 0x2000) != 0)
		{
			num4 += ptSpr.sprX2 - ptSpr.sprX1;
			num5 += ptSpr.sprY2 - ptSpr.sprY1;
		}
		else
		{
			CImage imageInfoEx = bank.getImageInfoEx(newImg, newAngle, newScaleX, newScaleY);
			if ((ptSpr.sprFlags & 0x400000) == 0)
			{
				num2 -= imageInfoEx.xSpot;
				num3 -= imageInfoEx.ySpot;
			}
			num4 = num2 + imageInfoEx.width;
			num5 = num3 + imageInfoEx.height;
		}
		for (CSprite objNext = firstSprite; objNext != null; objNext = objNext.objNext)
		{
			if (objNext.sprLayer < sprLayer)
			{
				continue;
			}
			if (objNext.sprLayer > sprLayer)
			{
				break;
			}
			if ((objNext.sprFlags & 1) == 0 || num2 >= objNext.sprX2 || num4 <= objNext.sprX1 || num3 >= objNext.sprY2 || num5 <= objNext.sprY1 || objNext == ptSpr)
			{
				continue;
			}
			int nFlags = 0;
			if ((dwFlags & 8) != 0 && (objNext.sprFlags & 0x20000) != 0)
			{
				if ((dwFlags & 1) != 0)
				{
					continue;
				}
				nFlags = 1;
			}
			if (num == 0 || (objNext.sprFlags & 0x100) != 0)
			{
				if (cArrayList == null)
				{
					cArrayList = new CArrayList();
				}
				cArrayList.add(objNext.sprExtraInfo);
				continue;
			}
			if (cMask == null)
			{
				cMask = getSpriteMask(ptSpr, newImg, 0, newAngle, newScaleX, newScaleY);
				if (cMask == null)
				{
					if (cArrayList == null)
					{
						cArrayList = new CArrayList();
					}
					cArrayList.add(objNext.sprExtraInfo);
					continue;
				}
			}
			CMask spriteMask = getSpriteMask(objNext, -1, nFlags, objNext.sprAngle, objNext.sprScaleX, objNext.sprScaleY);
			if (spriteMask != null && cMask.testMask(0, num2, num3, spriteMask, 0, objNext.sprX1, objNext.sprY1))
			{
				if (cArrayList == null)
				{
					cArrayList = new CArrayList();
				}
				cArrayList.add(objNext.sprExtraInfo);
			}
		}
		return cArrayList;
	}

	public CSprite spriteCol_TestSprite(CSprite ptSpr, short newImg, int newX, int newY, int newAngle, float newScaleX, float newScaleY, int subHt, uint dwFlags)
	{
		if (ptSpr == null)
		{
			return null;
		}
		if ((ptSpr.sprFlags & 0x100) != 0)
		{
			colMode = 0;
		}
		int sprLayer = ptSpr.sprLayer;
		if ((dwFlags & 8) != 0)
		{
			sprLayer &= -2;
		}
		else
		{
			if ((ptSpr.sprFlags & 1) == 0)
			{
				return null;
			}
			sprLayer |= 1;
		}
		int num = newX;
		int num2 = newY;
		int num3 = num;
		int num4 = num2;
		CMask cMask = null;
		if ((ptSpr.sprFlags & 0x2000) != 0)
		{
			num3 += ptSpr.sprX2 - ptSpr.sprX1;
			num4 += ptSpr.sprY2 - ptSpr.sprY1;
		}
		else
		{
			CImage imageInfoEx = bank.getImageInfoEx(newImg, newAngle, newScaleX, newScaleY);
			if ((ptSpr.sprFlags & 0x400000) == 0)
			{
				num -= imageInfoEx.xSpot;
				num2 -= imageInfoEx.ySpot;
			}
			num3 = num + imageInfoEx.width;
			num4 = num2 + imageInfoEx.height;
		}
		if (subHt != 0)
		{
			int num5 = num4 - num2;
			if (subHt > num5)
			{
				subHt = num5;
			}
			num2 += num5 - subHt;
		}
		for (CSprite objNext = firstSprite; objNext != null; objNext = objNext.objNext)
		{
			if (objNext.sprLayer < sprLayer)
			{
				continue;
			}
			if (objNext.sprLayer > sprLayer)
			{
				break;
			}
			if ((objNext.sprFlags & 1) == 0 || num >= objNext.sprX2 || num3 <= objNext.sprX1 || num2 >= objNext.sprY2 || num4 <= objNext.sprY1 || objNext == ptSpr || (objNext.sprFlags & 0x20) != 0)
			{
				continue;
			}
			int nFlags = 0;
			if ((dwFlags & 8) != 0 && (objNext.sprFlags & 0x20000) != 0)
			{
				if ((dwFlags & 1) != 0)
				{
					continue;
				}
				nFlags = 1;
			}
			if (colMode == 0 || (objNext.sprFlags & 0x100) != 0)
			{
				return objNext;
			}
			if (cMask == null)
			{
				cMask = getSpriteMask(ptSpr, newImg, 0, newAngle, newScaleX, newScaleY);
				if (cMask == null)
				{
					return objNext;
				}
			}
			int yBase = 0;
			int height = cMask.height;
			if (subHt != 0)
			{
				if (subHt > height)
				{
					subHt = height;
				}
				yBase = height - subHt;
				height = subHt;
			}
			CMask spriteMask = getSpriteMask(objNext, -1, nFlags, objNext.sprAngle, objNext.sprScaleX, objNext.sprScaleY);
			if (spriteMask != null && cMask.testMask(yBase, num, num2, spriteMask, 0, objNext.sprX1, objNext.sprY1))
			{
				return objNext;
			}
		}
		return null;
	}

	public CSprite spriteCol_TestRect(CSprite firstSpr, int nLayer, int xp, int yp, int wp, int hp, int dwFlags)
	{
		CSprite cSprite = firstSpr;
		cSprite = ((cSprite != null) ? cSprite.objNext : firstSprite);
		bool flag = nLayer == -1;
		bool flag2 = (dwFlags & 4) != 0;
		short num;
		if ((dwFlags & 8) != 0)
		{
			num = 0;
			if (nLayer != -1)
			{
				nLayer *= 2;
			}
		}
		else
		{
			num = 1;
			if (nLayer != -1)
			{
				nLayer = nLayer * 2 + 1;
			}
		}
		for (; cSprite != null; cSprite = cSprite.objNext)
		{
			if (!flag)
			{
				if (cSprite.sprLayer < nLayer)
				{
					continue;
				}
				if (cSprite.sprLayer > nLayer)
				{
					break;
				}
			}
			else if ((cSprite.sprLayer & 1) != num)
			{
				continue;
			}
			if ((!flag2 && (cSprite.sprFlags & 1) == 0) || xp > cSprite.sprX2 || xp + wp <= cSprite.sprX1 || yp > cSprite.sprY2 || yp + hp <= cSprite.sprY1 || (cSprite.sprFlags & 0x20) != 0)
			{
				continue;
			}
			int nFlags = 0;
			if ((dwFlags & 8) != 0 && (cSprite.sprFlags & 0x20000) != 0)
			{
				if ((dwFlags & 1) != 0)
				{
					continue;
				}
				nFlags = 1;
			}
			if (colMode == 0 || (cSprite.sprFlags & 0x100) != 0)
			{
				return cSprite;
			}
			CMask spriteMask = getSpriteMask(cSprite, -1, nFlags, cSprite.sprAngle, cSprite.sprScaleX, cSprite.sprScaleY);
			if (spriteMask != null && spriteMask.testRect(0, xp - cSprite.sprX1, yp - cSprite.sprY1, wp, hp))
			{
				return cSprite;
			}
		}
		return null;
	}
}
