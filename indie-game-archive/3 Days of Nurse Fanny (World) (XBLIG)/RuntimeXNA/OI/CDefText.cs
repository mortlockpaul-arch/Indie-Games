using RuntimeXNA.Banks;
using RuntimeXNA.Services;

namespace RuntimeXNA.OI;

public class CDefText
{
	public const short TSF_LEFT = 0;

	public const short TSF_HCENTER = 1;

	public const short TSF_RIGHT = 2;

	public const short TSF_VCENTER = 4;

	public const short TSF_HALIGN = 15;

	public const short TSF_CORRECT = 256;

	public const short TSF_RELIEF = 512;

	public short tsFont;

	public short tsFlags;

	public int tsColor;

	public string tsText;

	public void load(CFile file)
	{
		tsFont = file.readAShort();
		tsFlags = file.readAShort();
		tsColor = file.readAColor();
		tsText = file.readAString();
	}

	public void enumElements(IEnum enumImages, IEnum enumFonts)
	{
		if (enumFonts != null)
		{
			short num = enumFonts.enumerate(tsFont);
			if (num != -1)
			{
				tsFont = num;
			}
		}
	}
}
