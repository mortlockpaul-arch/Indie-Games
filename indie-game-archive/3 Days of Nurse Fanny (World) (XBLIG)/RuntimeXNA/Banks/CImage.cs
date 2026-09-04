using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RuntimeXNA.Application;
using RuntimeXNA.Services;
using RuntimeXNA.Sprites;

namespace RuntimeXNA.Banks;

public class CImage
{
	public const int maxRotatedMasks = 10;

	public CRunApp app;

	public short handle;

	public short width;

	public short height;

	public short xSpot;

	public short ySpot;

	public short xAP;

	public short yAP;

	public short useCount;

	public Texture2D image;

	public CMask maskNormal;

	public CMask maskPlatform;

	public CArrayList maskRotation;

	public short mosaic;

	public Rectangle mosaicRectangle;

	public void loadHandle(CFile file)
	{
		handle = file.readAShort();
		file.skipBytes(12);
	}

	public void load(CRunApp a)
	{
		app = a;
		handle = app.file.readAShort();
		width = app.file.readAShort();
		height = app.file.readAShort();
		xSpot = app.file.readAShort();
		ySpot = app.file.readAShort();
		xAP = app.file.readAShort();
		yAP = app.file.readAShort();
		mosaic = 0;
		if (app.frame.mosaicHandles != null && app.frame.mosaicHandles[handle] != 0)
		{
			mosaic = app.frame.mosaicHandles[handle];
			app.imageBank.loadMosaic(mosaic);
			mosaicRectangle.X = app.frame.mosaicX[handle];
			mosaicRectangle.Y = app.frame.mosaicY[handle];
			mosaicRectangle.Width = width;
			mosaicRectangle.Height = height;
		}
		else
		{
			string text = handle.ToString("D4");
			text = "Img" + text;
			image = app.content.Load<Texture2D>(text);
			LoadUpfront.BuildLoadInfo_Texture2D(text);
		}
	}

	public CMask getMask(int flags, int angle, float scaleX, float scaleY)
	{
		if ((flags & 1) == 0)
		{
			if (maskNormal == null)
			{
				maskNormal = new CMask();
				maskNormal.createMask(this, flags);
			}
			if (angle == 0 && (double)scaleX == 1.0 && (double)scaleY == 1.0)
			{
				return maskNormal;
			}
			if (maskRotation == null)
			{
				maskRotation = new CArrayList();
			}
			int num = int.MaxValue;
			int num2 = -1;
			CRotatedMask cRotatedMask;
			for (int i = 0; i < maskRotation.size(); i++)
			{
				cRotatedMask = (CRotatedMask)maskRotation.get(i);
				if (angle == cRotatedMask.angle && scaleX == cRotatedMask.scaleX && scaleY == cRotatedMask.scaleY)
				{
					return cRotatedMask.mask;
				}
				if (cRotatedMask.tick < num)
				{
					num = cRotatedMask.tick;
					num2 = i;
				}
			}
			if (maskRotation.size() < 10)
			{
				num2 = -1;
			}
			cRotatedMask = new CRotatedMask();
			cRotatedMask.mask = new CMask();
			cRotatedMask.mask.createRotatedMask(maskNormal, angle, scaleX, scaleY);
			cRotatedMask.angle = angle;
			cRotatedMask.scaleX = scaleX;
			cRotatedMask.scaleY = scaleY;
			cRotatedMask.tick = (int)app.timer;
			if (num2 < 0)
			{
				maskRotation.add(cRotatedMask);
			}
			else
			{
				maskRotation.set(num2, cRotatedMask);
			}
			return cRotatedMask.mask;
		}
		if (maskPlatform == null)
		{
			maskPlatform = new CMask();
			maskPlatform.createMask(this, flags);
		}
		return maskPlatform;
	}
}
