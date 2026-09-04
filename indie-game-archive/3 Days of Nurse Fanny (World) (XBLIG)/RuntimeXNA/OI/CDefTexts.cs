using RuntimeXNA.Banks;
using RuntimeXNA.Services;

namespace RuntimeXNA.OI;

public class CDefTexts : CDefObject
{
	public int otCx;

	public int otCy;

	public int otNumberOfText;

	public CDefText[] otTexts;

	public override void load(CFile file)
	{
		int filePointer = file.getFilePointer();
		file.skipBytes(4);
		otCx = file.readAInt();
		otCy = file.readAInt();
		otNumberOfText = file.readAInt();
		otTexts = new CDefText[otNumberOfText];
		int[] array = new int[otNumberOfText];
		for (int i = 0; i < otNumberOfText; i++)
		{
			array[i] = file.readAInt();
		}
		for (int i = 0; i < otNumberOfText; i++)
		{
			otTexts[i] = new CDefText();
			file.seek(filePointer + array[i]);
			otTexts[i].load(file);
		}
	}

	public override void enumElements(IEnum enumImages, IEnum enumFonts)
	{
		for (int i = 0; i < otNumberOfText; i++)
		{
			otTexts[i].enumElements(enumImages, enumFonts);
		}
	}
}
