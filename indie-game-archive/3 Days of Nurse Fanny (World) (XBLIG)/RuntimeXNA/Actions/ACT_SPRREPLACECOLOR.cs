using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RuntimeXNA.Banks;
using RuntimeXNA.OI;
using RuntimeXNA.Objects;
using RuntimeXNA.Params;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Services;

namespace RuntimeXNA.Actions;

public class ACT_SPRREPLACECOLOR : CAct, IEnum
{
	internal int mode;

	internal int dwMax;

	internal short[] pImages;

	internal CRun pRh;

	public override void execute(CRun rhPtr)
	{
		CObject cObject = rhPtr.rhEvtProg.get_ActionObjects(this);
		if (cObject == null)
		{
			return;
		}
		cObject.roa.animIn(0);
		int oldColor;
		if (evtParams[0].code == 24)
		{
			oldColor = ((PARAM_COLOUR)evtParams[0]).color;
		}
		else
		{
			oldColor = rhPtr.get_EventExpressionInt((CParamExpression)evtParams[0]);
			oldColor = CServices.swapRGB(oldColor);
		}
		int newColor;
		if (evtParams[1].code == 24)
		{
			newColor = ((PARAM_COLOUR)evtParams[1]).color;
		}
		else
		{
			newColor = rhPtr.get_EventExpressionInt((CParamExpression)evtParams[1]);
			newColor = CServices.swapRGB(newColor);
		}
		pRh = rhPtr;
		short hoOi = cObject.hoOi;
		COI oIFromHandle = rhPtr.rhApp.OIList.getOIFromHandle(hoOi);
		if (oIFromHandle == null)
		{
			return;
		}
		dwMax = -1;
		mode = 0;
		oIFromHandle.enumElements(this, null);
		CObject cObject2 = cObject;
		while ((cObject2.hoNumPrev & 0x8000) == 0)
		{
			cObject2 = rhPtr.rhObjectList[cObject2.hoNumPrev & 0x7FFF];
		}
		while (true)
		{
			if (cObject2.roc.rcImage != -1 && cObject2.roc.rcImage > dwMax)
			{
				dwMax = cObject2.roc.rcImage;
			}
			if (cObject2.roc.rcOldImage != -1 && cObject2.roc.rcOldImage > dwMax)
			{
				dwMax = cObject2.roc.rcOldImage;
			}
			if ((cObject2.hoNumNext & 0x8000) != 0)
			{
				break;
			}
			cObject2 = rhPtr.rhObjectList[cObject2.hoNumNext];
		}
		pImages = new short[dwMax + 1];
		for (int i = 0; i < dwMax + 1; i++)
		{
			pImages[i] = -1;
		}
		mode = 1;
		oIFromHandle.enumElements(this, null);
		Texture2D texture2D = null;
		for (int j = 0; j <= dwMax; j++)
		{
			if (pImages[j] != -1)
			{
				CImage imageFromHandle = rhPtr.rhApp.imageBank.getImageFromHandle((short)j);
				int width = imageFromHandle.width;
				int height = imageFromHandle.height;
				Color[] array = new Color[width * height];
				if (imageFromHandle.mosaic == 0)
				{
					imageFromHandle.image.GetData(array);
					CServices.replaceColor(rhPtr.rhApp, array, width, height, oldColor, newColor);
					Texture2D texture2D2 = new Texture2D(rhPtr.rhApp.spriteBatch.GraphicsDevice, width, height);
					texture2D2.SetData(array);
					short num = rhPtr.rhApp.imageBank.addImage(texture2D2, imageFromHandle.xSpot, imageFromHandle.ySpot, imageFromHandle.xAP, imageFromHandle.yAP, 0);
					pImages[j] = num;
				}
				else
				{
					texture2D = rhPtr.rhApp.imageBank.mosaics[imageFromHandle.mosaic];
					texture2D.GetData(0, imageFromHandle.mosaicRectangle, array, 0, width * height);
					CServices.replaceColor(rhPtr.rhApp, array, width, height, oldColor, newColor);
					Texture2D texture2D3 = new Texture2D(rhPtr.rhApp.spriteBatch.GraphicsDevice, width, height);
					texture2D3.SetData(array);
					short num = rhPtr.rhApp.imageBank.addImage(texture2D3, imageFromHandle.xSpot, imageFromHandle.ySpot, imageFromHandle.xAP, imageFromHandle.yAP, 0);
					pImages[j] = num;
				}
			}
		}
		cObject2 = cObject;
		while ((cObject2.hoNumPrev & 0x8000) == 0)
		{
			cObject2 = rhPtr.rhObjectList[cObject2.hoNumPrev & 0x7FFF];
		}
		while (true)
		{
			if (cObject2.roc.rcImage != -1 && pImages[cObject2.roc.rcImage] != -1)
			{
				cObject2.roc.rcImage = pImages[cObject2.roc.rcImage];
			}
			if (cObject2.roc.rcOldImage != -1 && pImages[cObject2.roc.rcOldImage] != -1)
			{
				cObject2.roc.rcOldImage = pImages[cObject2.roc.rcOldImage];
			}
			if (cObject2.roc.rcSprite != null)
			{
				rhPtr.rhApp.spriteGen.modifSprite(cObject2.roc.rcSprite, cObject2.hoX - rhPtr.rhWindowX, cObject2.hoY - rhPtr.rhWindowY, cObject2.roc.rcImage);
			}
			if ((cObject2.hoNumNext & 0x8000) != 0)
			{
				break;
			}
			cObject2 = rhPtr.rhObjectList[cObject2.hoNumNext];
		}
		mode = 2;
		oIFromHandle.enumElements(this, null);
		mode = 3;
		oIFromHandle.enumElements(this, null);
		oIFromHandle.oiLoadFlags |= 32;
		cObject.roc.rcChanged = true;
	}

	public virtual short enumerate(short num)
	{
		switch (mode)
		{
		case 0:
			if (num > dwMax)
			{
				dwMax = num;
			}
			return -1;
		case 1:
			pImages[num] = 1;
			return -1;
		case 2:
			if (pImages[num] >= 0)
			{
				pRh.rhApp.imageBank.delImage(num);
			}
			return -1;
		case 3:
			if (pImages[num] >= 0)
			{
				CImage imageFromHandle = pRh.rhApp.imageBank.getImageFromHandle(pImages[num]);
				imageFromHandle.useCount++;
				return pImages[num];
			}
			break;
		}
		return -1;
	}
}
