using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using RuntimeXNA.Application;
using RuntimeXNA.Services;

namespace RuntimeXNA.Banks;

public class CFont
{
	public short useCount;

	public short handle;

	public int lfHeight;

	public byte lfItalic;

	public int lfWeight;

	public string lfFaceName;

	public SpriteFont spriteFont;

	private ContentManager content;

	public void loadHandle(CFile file)
	{
		handle = file.readAShort();
		if (!file.bUnicode)
		{
			file.skipBytes(41);
		}
		else
		{
			file.skipBytes(73);
		}
	}

	public void load(CFile file, ContentManager c)
	{
		content = c;
		handle = file.readAShort();
		int filePointer = file.getFilePointer();
		lfHeight = file.readAInt();
		lfWeight = file.readAInt();
		lfItalic = file.readAByte();
		lfFaceName = file.readAString();
		if (!file.bUnicode)
		{
			file.seek(filePointer + 41);
		}
		else
		{
			file.seek(filePointer + 73);
		}
	}

	public CFontInfo getFontInfo()
	{
		CFontInfo cFontInfo = new CFontInfo();
		cFontInfo.lfHeight = lfHeight;
		cFontInfo.lfWeight = lfWeight;
		cFontInfo.lfItalic = lfItalic;
		cFontInfo.lfFaceName = lfFaceName;
		return cFontInfo;
	}

	public static CFont createFromFontInfo(CFontInfo info, CRunApp app)
	{
		CFont cFont = new CFont();
		cFont.content = app.content;
		cFont.lfHeight = info.lfHeight;
		cFont.lfWeight = info.lfWeight;
		cFont.lfItalic = info.lfItalic;
		cFont.lfFaceName = info.lfFaceName;
		return cFont;
	}

	public SpriteFont getFont()
	{
		if (spriteFont == null)
		{
			string text = lfFaceName;
			while (true)
			{
				int num = text.IndexOf(' ');
				if (num < 0)
				{
					break;
				}
				text = text.Substring(0, num) + text.Substring(num + 1);
			}
			text += lfHeight;
			if (lfWeight > 400)
			{
				text += "Bold";
			}
			if (lfItalic != 0)
			{
				text += "Italic";
			}
			spriteFont = content.Load<SpriteFont>(text);
		}
		return spriteFont;
	}
}
