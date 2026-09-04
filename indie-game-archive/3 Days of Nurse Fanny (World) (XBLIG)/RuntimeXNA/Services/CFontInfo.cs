namespace RuntimeXNA.Services;

public class CFontInfo
{
	public int lfHeight;

	public int lfWeight;

	public byte lfItalic;

	public byte lfUnderline;

	public byte lfStrikeOut;

	public string lfFaceName;

	public void copy(CFontInfo f)
	{
		lfHeight = f.lfHeight;
		lfWeight = f.lfWeight;
		lfItalic = f.lfItalic;
		lfUnderline = f.lfUnderline;
		lfStrikeOut = f.lfStrikeOut;
		lfFaceName = f.lfFaceName;
	}
}
