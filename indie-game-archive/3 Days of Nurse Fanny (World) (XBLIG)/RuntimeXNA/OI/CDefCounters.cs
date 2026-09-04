using RuntimeXNA.Banks;
using RuntimeXNA.Services;

namespace RuntimeXNA.OI;

public class CDefCounters
{
	public const short CTA_HIDDEN = 0;

	public const short CTA_DIGITS = 1;

	public const short CTA_VBAR = 2;

	public const short CTA_HBAR = 3;

	public const short CTA_ANIM = 4;

	public const short CTA_TEXT = 5;

	public const short BARFLAG_INVERSE = 256;

	public int odCx;

	public int odCy;

	public short odPlayer;

	public short odDisplayType;

	public short odDisplayFlags;

	public short odFont;

	public short ocBorderSize;

	public int ocBorderColor;

	public short ocShape;

	public short ocFillType;

	public short ocLineFlags;

	public int ocColor1;

	public int ocColor2;

	public int ocGradientFlags;

	public short nFrames;

	public short[] frames;

	public void load(CFile file)
	{
		file.skipBytes(4);
		odCx = file.readAInt();
		odCy = file.readAInt();
		odPlayer = file.readAShort();
		odDisplayType = file.readAShort();
		odDisplayFlags = file.readAShort();
		odFont = file.readAShort();
		switch (odDisplayType)
		{
		case 1:
		case 4:
		{
			nFrames = file.readAShort();
			frames = new short[nFrames];
			for (int i = 0; i < nFrames; i++)
			{
				frames[i] = file.readAShort();
			}
			break;
		}
		case 2:
		case 3:
		case 5:
			ocBorderSize = file.readAShort();
			ocBorderColor = file.readAColor();
			ocShape = file.readAShort();
			ocFillType = file.readAShort();
			if (ocShape == 1)
			{
				ocLineFlags = file.readAShort();
				break;
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
				break;
			}
			break;
		case 0:
			break;
		}
	}

	public void enumElements(IEnum enumImages, IEnum enumFonts)
	{
		switch (odDisplayType)
		{
		case 1:
		case 4:
		{
			for (int i = 0; i < nFrames; i++)
			{
				if (enumImages != null)
				{
					short num = enumImages.enumerate(frames[i]);
					if (num != -1)
					{
						frames[i] = num;
					}
				}
			}
			break;
		}
		case 5:
			if (enumFonts != null)
			{
				short num = enumFonts.enumerate(odFont);
				if (num != -1)
				{
					odFont = num;
				}
			}
			break;
		case 2:
		case 3:
			break;
		}
	}
}
