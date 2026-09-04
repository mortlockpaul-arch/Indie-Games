using System;
using Microsoft.Xna.Framework.Graphics;
using RuntimeXNA.Application;
using RuntimeXNA.Services;

namespace RuntimeXNA.Banks;

public class CImageBank : IEnum
{
	public CRunApp app;

	public CFile file;

	public CImage[] images;

	public int nHandlesReel;

	public int nHandlesTotal;

	public int nImages;

	private int[] offsetsToImage;

	private short[] handleToIndex;

	private byte[] useCount;

	private CRect rcInfo;

	private CPoint hsInfo;

	private CPoint apInfo;

	public Texture2D[] mosaics;

	public Texture2D[] oldMosaics;

	public CImageBank()
	{
	}

	public CImageBank(CRunApp a)
	{
		app = a;
		file = app.file;
	}

	public void preLoad()
	{
		nHandlesReel = file.readAShort();
		offsetsToImage = new int[nHandlesReel];
		int num = file.readAShort();
		CImage cImage = new CImage();
		for (int i = 0; i < num; i++)
		{
			int filePointer = file.getFilePointer();
			cImage.loadHandle(app.file);
			offsetsToImage[cImage.handle] = filePointer;
		}
		useCount = new byte[nHandlesReel];
		resetToLoad();
		handleToIndex = null;
		nHandlesTotal = nHandlesReel;
		nImages = 0;
		images = null;
	}

	public CImage getImageFromHandle(short handle)
	{
		if (handle >= 0 && handle < nHandlesTotal && handleToIndex[handle] != -1)
		{
			return images[handleToIndex[handle]];
		}
		return null;
	}

	public CImage getImageFromIndex(short index)
	{
		if (index >= 0 && index < nImages)
		{
			return images[index];
		}
		return null;
	}

	public void resetToLoad()
	{
		for (int i = 0; i < nHandlesReel; i++)
		{
			useCount[i] = 0;
		}
	}

	public void setToLoad(short handle)
	{
		useCount[handle]++;
	}

	public short enumerate(short num)
	{
		setToLoad(num);
		return -1;
	}

	public void loadMosaic(short handle)
	{
		if (mosaics[handle] == null)
		{
			if (oldMosaics != null && handle < oldMosaics.Length && oldMosaics[handle] != null)
			{
				mosaics[handle] = oldMosaics[handle];
				return;
			}
			string text = handle.ToString("D4");
			text = "ImgM" + text;
			mosaics[handle] = app.content.Load<Texture2D>(text);
			LoadUpfront.BuildLoadInfo_Texture2D(text);
		}
	}

	public void load()
	{
		if (app.frame.mosaicMaxHandle > 0)
		{
			int num = app.frame.mosaicMaxHandle;
			if (mosaics != null)
			{
				oldMosaics = new Texture2D[mosaics.Length];
				for (int i = 0; i < mosaics.Length; i++)
				{
					oldMosaics[i] = mosaics[i];
				}
				num = Math.Max(num, mosaics.Length);
			}
			mosaics = new Texture2D[num];
			for (int i = 0; i < num; i++)
			{
				mosaics[i] = null;
			}
		}
		nImages = 0;
		for (int i = 0; i < nHandlesReel; i++)
		{
			if (useCount[i] != 0)
			{
				nImages++;
			}
		}
		CImage[] array = new CImage[nImages];
		int num2 = 0;
		for (int j = 0; j < nHandlesReel; j++)
		{
			if (useCount[j] == 0)
			{
				continue;
			}
			if (images != null && handleToIndex[j] != -1 && images[handleToIndex[j]] != null)
			{
				array[num2] = images[handleToIndex[j]];
				array[num2].useCount = useCount[j];
				if (mosaics != null && oldMosaics != null)
				{
					short mosaic = array[num2].mosaic;
					if (mosaic > 0)
					{
						mosaics[mosaic] = oldMosaics[mosaic];
					}
				}
			}
			else
			{
				array[num2] = new CImage();
				file.seek(offsetsToImage[j]);
				array[num2].load(app);
				array[num2].useCount = useCount[j];
			}
			num2++;
		}
		images = array;
		handleToIndex = new short[nHandlesReel];
		for (int i = 0; i < nHandlesReel; i++)
		{
			handleToIndex[i] = -1;
		}
		for (int i = 0; i < nImages; i++)
		{
			handleToIndex[images[i].handle] = (short)i;
		}
		nHandlesTotal = nHandlesReel;
		resetToLoad();
		oldMosaics = null;
	}

	public CImage getImageInfoEx(short nImage, int nAngle, float fScaleX, float fScaleY)
	{
		CImage cImage = new CImage();
		CImage imageFromHandle = getImageFromHandle(nImage);
		if (imageFromHandle != null)
		{
			int num = imageFromHandle.width;
			int num2 = imageFromHandle.height;
			int num3 = imageFromHandle.xSpot;
			int num4 = imageFromHandle.ySpot;
			int num5 = imageFromHandle.xAP;
			int num6 = imageFromHandle.yAP;
			if (nAngle == 0)
			{
				if ((double)fScaleX != 1.0)
				{
					num3 = (int)((float)num3 * fScaleX);
					num5 = (int)((float)num5 * fScaleX);
					num = (int)((float)num * fScaleX);
				}
				if ((double)fScaleY != 1.0)
				{
					num4 = (int)((float)num4 * fScaleY);
					num6 = (int)((float)num6 * fScaleY);
					num2 = (int)((float)num2 * fScaleY);
				}
			}
			else
			{
				if ((double)fScaleX != 1.0)
				{
					num3 = (int)((float)num3 * fScaleX);
					num5 = (int)((float)num5 * fScaleX);
					num = (int)((float)num * fScaleX);
				}
				if ((double)fScaleY != 1.0)
				{
					num4 = (int)((float)num4 * fScaleY);
					num6 = (int)((float)num6 * fScaleY);
					num2 = (int)((float)num2 * fScaleY);
				}
				if (rcInfo == null)
				{
					rcInfo = new CRect();
				}
				if (hsInfo == null)
				{
					hsInfo = new CPoint();
				}
				if (apInfo == null)
				{
					apInfo = new CPoint();
				}
				hsInfo.x = num3;
				hsInfo.y = num4;
				apInfo.x = num5;
				apInfo.y = num6;
				rcInfo.left = (rcInfo.top = 0);
				rcInfo.right = num;
				rcInfo.bottom = num2;
				doRotateRect(rcInfo, hsInfo, apInfo, nAngle);
				num = rcInfo.right;
				num2 = rcInfo.bottom;
				num3 = hsInfo.x;
				num4 = hsInfo.y;
				num5 = apInfo.x;
				num6 = apInfo.y;
			}
			cImage.width = (short)num;
			cImage.height = (short)num2;
			cImage.xSpot = (short)num3;
			cImage.ySpot = (short)num4;
			cImage.xAP = (short)num5;
			cImage.yAP = (short)num6;
			return cImage;
		}
		return null;
	}

	private void doRotateRect(CRect prc, CPoint pHotSpot, CPoint pActionPoint, double fAngle)
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
			double num3 = fAngle * Math.PI / 180.0;
			num = Math.Cos(num3);
			num2 = Math.Sin(num3);
		}
		double num7;
		double num8;
		double num9;
		double num4;
		double num5;
		double num6;
		if (pHotSpot == null)
		{
			num4 = (num5 = (num6 = 0.0));
			num7 = (num8 = 0.0);
		}
		else
		{
			num9 = (double)(-pHotSpot.x) * num;
			num4 = (double)(-pHotSpot.x) * num2;
			num5 = (double)(-pHotSpot.y) * num;
			num6 = (double)(-pHotSpot.y) * num2;
			num7 = num9 + num6;
			num8 = num5 - num4;
		}
		double num10 = ((pHotSpot != null) ? ((double)(prc.right - pHotSpot.x)) : ((double)prc.right));
		num9 = num10 * num;
		num4 = num10 * num2;
		double num11 = num9 + num6;
		double num12 = num5 - num4;
		double num13 = ((pHotSpot != null) ? ((double)(prc.bottom - pHotSpot.y)) : ((double)prc.bottom));
		num5 = num13 * num;
		num6 = num13 * num2;
		double num14 = num9 + num6;
		double num15 = num5 - num4;
		double val = num7 + num14 - num11;
		double val2 = num8 + num15 - num12;
		double num16 = Math.Min(num7, Math.Min(num11, Math.Min(num14, val)));
		double num17 = Math.Min(num8, Math.Min(num12, Math.Min(num15, val2)));
		double num18 = Math.Max(num7, Math.Max(num11, Math.Max(num14, val)));
		double num19 = Math.Max(num8, Math.Max(num12, Math.Max(num15, val2)));
		if (pActionPoint != null)
		{
			if (pHotSpot == null)
			{
				num10 = pActionPoint.x;
				num13 = pActionPoint.y;
			}
			else
			{
				num10 = pActionPoint.x - pHotSpot.x;
				num13 = pActionPoint.y - pHotSpot.y;
			}
			pActionPoint.x = (int)(num10 * num + num13 * num2 - num16);
			pActionPoint.y = (int)(num13 * num - num10 * num2 - num17);
		}
		if (pHotSpot != null)
		{
			pHotSpot.x = (int)(0.0 - num16);
			pHotSpot.y = (int)(0.0 - num17);
		}
		prc.right = (int)(num18 - num16);
		prc.bottom = (int)(num19 - num17);
	}

	public short addImage(Texture2D img, short xSpot, short ySpot, short xAP, short yAP, short count)
	{
		short num = -1;
		for (int i = nHandlesReel; i < nHandlesTotal; i++)
		{
			if (handleToIndex[i] == -1)
			{
				num = (short)i;
				break;
			}
		}
		if (num == -1)
		{
			short[] array = new short[nHandlesTotal + 10];
			int i;
			for (i = 0; i < nHandlesTotal; i++)
			{
				array[i] = handleToIndex[i];
			}
			for (; i < nHandlesTotal + 10; i++)
			{
				array[i] = -1;
			}
			num = (short)nHandlesTotal;
			nHandlesTotal += 10;
			handleToIndex = array;
		}
		int num2 = -1;
		for (int j = 0; j < nImages; j++)
		{
			if (images[j] == null)
			{
				num2 = j;
				break;
			}
		}
		if (num2 == -1)
		{
			CImage[] array2 = new CImage[nImages + 10];
			int j;
			for (j = 0; j < nImages; j++)
			{
				array2[j] = images[j];
			}
			for (; j < nImages + 10; j++)
			{
				array2[j] = null;
			}
			num2 = nImages;
			nImages += 10;
			images = array2;
		}
		handleToIndex[num] = (short)num2;
		images[num2] = new CImage();
		images[num2].handle = num;
		images[num2].image = img;
		images[num2].xSpot = xSpot;
		images[num2].ySpot = ySpot;
		images[num2].xAP = xAP;
		images[num2].yAP = yAP;
		images[num2].useCount = count;
		images[num2].width = (short)img.Width;
		images[num2].height = (short)img.Height;
		return num;
	}

	public void delImage(short handle)
	{
		CImage imageFromHandle = getImageFromHandle(handle);
		if (imageFromHandle == null)
		{
			return;
		}
		imageFromHandle.useCount--;
		if (imageFromHandle.useCount > 0)
		{
			return;
		}
		for (int i = 0; i < nImages; i++)
		{
			if (images[i] == imageFromHandle)
			{
				images[i] = null;
				handleToIndex[handle] = -1;
				break;
			}
		}
	}

	public void loadImageList(short[] handles)
	{
		for (int i = 0; i < handles.Length; i++)
		{
			if (handles[i] < 0 || handles[i] >= nHandlesTotal || offsetsToImage[handles[i]] == 0 || getImageFromHandle(handles[i]) != null)
			{
				continue;
			}
			int num = -1;
			for (int j = 0; j < nImages; j++)
			{
				if (images[j] == null)
				{
					num = j;
					break;
				}
			}
			if (num == -1)
			{
				CImage[] array = new CImage[nImages + 10];
				int j;
				for (j = 0; j < nImages; j++)
				{
					array[j] = images[j];
				}
				for (; j < nImages + 10; j++)
				{
					array[j] = null;
				}
				num = nImages;
				nImages += 10;
				images = array;
			}
			handleToIndex[handles[i]] = (short)num;
			images[num] = new CImage();
			images[num].useCount = 1;
			file.seek(offsetsToImage[handles[i]]);
			images[num].load(app);
		}
	}
}
