using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RuntimeXNA.Application;
using RuntimeXNA.Banks;
using RuntimeXNA.Services;
using RuntimeXNA.Sprites;

namespace RuntimeXNA.OI;

internal class COCQBackdrop : COC, IDrawing
{
	public const short FILLTYPE_NONE = 0;

	public const short FILLTYPE_SOLID = 1;

	public const short FILLTYPE_GRADIENT = 2;

	public const short FILLTYPE_MOTIF = 3;

	public const short SHAPE_NONE = 0;

	public const short SHAPE_LINE = 1;

	public const short SHAPE_RECTANGLE = 2;

	public const short SHAPE_ELLIPSE = 3;

	public const short LINEF_INVX = 1;

	public const short LINEF_INVY = 2;

	public short ocBorderSize;

	public int ocBorderColor;

	public short ocShape;

	public short ocFillType;

	public short ocLineFlags;

	public int ocColor1;

	public int ocColor2;

	public int ocGradientFlags;

	public short ocImage;

	public CRunApp app;

	public Texture2D texture;

	public COCQBackdrop()
	{
	}

	public COCQBackdrop(CRunApp a)
	{
		app = a;
	}

	public override void load(CFile file, short type)
	{
		file.skipBytes(4);
		ocObstacleType = file.readAShort();
		ocColMode = file.readAShort();
		ocCx = file.readAInt();
		ocCy = file.readAInt();
		ocBorderSize = file.readAShort();
		ocBorderColor = file.readAColor();
		ocShape = file.readAShort();
		ocFillType = file.readAShort();
		if (ocShape == 1)
		{
			ocLineFlags = file.readAShort();
			return;
		}
		switch (ocFillType)
		{
		case 1:
			ocColor1 = file.readAColor();
			break;
		case 2:
			ocColor1 = file.readAColor();
			ocColor2 = file.readAColor();
			ocGradientFlags = file.readAInt();
			break;
		case 3:
			ocImage = file.readAShort();
			break;
		}
	}

	public override void enumElements(IEnum enumImages, IEnum enumFonts)
	{
		if (ocFillType == 3 && enumImages != null)
		{
			short num = enumImages.enumerate(ocImage);
			if (num != -1)
			{
				ocImage = num;
			}
		}
	}

	public void drawableDraw(SpriteBatchEffect batch, CSprite sprite, CImageBank bank, int x, int y)
	{
		int num = ocBorderSize;
		int num2 = ocCx;
		int num3 = ocCy;
		bool bVertical = false;
		if (ocGradientFlags != 0)
		{
			bVertical = true;
		}
		switch (ocShape)
		{
		case 2:
			switch (ocFillType)
			{
			case 1:
				app.services.drawFilledRectangle(app, x, y, num2, num3, ocColor1, num, ocBorderColor, oi.oiInkEffect & 0xFFF, oi.oiInkEffectParam);
				break;
			case 2:
				if (texture == null)
				{
					texture = CServices.createGradientRectangle(app, num2, num3, ocColor1, ocColor2, bVertical, num, ocBorderColor);
				}
				break;
			case 3:
			{
				CImage imageFromHandle = app.imageBank.getImageFromHandle(ocImage);
				app.services.drawPatternRectangle(app.spriteBatch, imageFromHandle, x, y, num2, num3, num, ocBorderColor, oi.oiInkEffect & 0xFFF, oi.oiInkEffectParam);
				num2 = (num2 + imageFromHandle.width - 1) / imageFromHandle.width * imageFromHandle.width;
				num3 = (num3 + imageFromHandle.height - 1) / imageFromHandle.height * imageFromHandle.width;
				break;
			}
			}
			break;
		case 3:
			switch (ocFillType)
			{
			case 1:
				if (texture == null)
				{
					texture = CServices.createFilledEllipse(app, num2, num3, ocColor1, num, ocBorderColor);
				}
				break;
			case 2:
				if (texture == null)
				{
					texture = CServices.createGradientEllipse(app, num2, num3, ocColor1, ocColor2, bVertical, num, ocBorderColor);
				}
				break;
			case 3:
			{
				CImage imageFromHandle = app.imageBank.getImageFromHandle(ocImage);
				app.services.drawPatternRectangle(app.spriteBatch, imageFromHandle, x, y, num2, num3, num, ocBorderColor, oi.oiInkEffect & 0xFFF, oi.oiInkEffectParam);
				num2 = (num2 + imageFromHandle.width - 1) / imageFromHandle.width * imageFromHandle.width;
				num3 = (num3 + imageFromHandle.height - 1) / imageFromHandle.height * imageFromHandle.width;
				break;
			}
			}
			break;
		}
		if (ocShape == 1 && num > 0)
		{
			if ((ocLineFlags & 1) != 0)
			{
				x += num2;
				num2 = -num2;
			}
			if ((ocLineFlags & 2) != 0)
			{
				y += num3;
				num3 = -num3;
			}
			app.services.drawLine(app.spriteBatch, x, y, x + num2, y + num3, ocBorderColor, num, oi.oiInkEffect & 0xFFF, oi.oiInkEffectParam);
		}
		if (texture != null)
		{
			app.tempRect.X = x;
			app.tempRect.Y = y;
			app.tempRect.Width = texture.Width;
			app.tempRect.Height = texture.Height;
			batch.Draw(texture, app.tempRect, null, Color.White, oi.oiInkEffect & 0xFFF, oi.oiInkEffectParam);
		}
	}

	public void drawableKill()
	{
	}

	public CMask drawableGetMask(int flags)
	{
		return null;
	}
}
