using System;
using RuntimeXNA.Application;
using RuntimeXNA.Services;

namespace RuntimeXNA.Banks;

public class CFontBank : IEnum
{
	private CRunApp app;

	private CFile file;

	public CFont[] fonts;

	private int[] offsetsToFonts;

	private int nFonts;

	private short[] handleToIndex;

	public int nHandlesReel;

	public int nHandlesTotal;

	private short[] useCount;

	public CFontBank()
	{
	}

	public CFontBank(CRunApp a)
	{
		app = a;
		file = app.file;
	}

	public void preLoad()
	{
		int num = file.readAShort();
		nHandlesReel = file.readAShort();
		offsetsToFonts = new int[nHandlesReel];
		int filePointer = file.getFilePointer();
		CFont cFont = new CFont();
		for (int i = 0; i < num; i++)
		{
			filePointer = file.getFilePointer();
			cFont.loadHandle(file);
			offsetsToFonts[cFont.handle] = filePointer;
		}
		useCount = new short[nHandlesReel];
		resetToLoad();
		handleToIndex = null;
		nHandlesTotal = nHandlesReel;
		num = 0;
		fonts = null;
	}

	public void load()
	{
		nFonts = 0;
		for (int i = 0; i < nHandlesTotal; i++)
		{
			if (useCount[i] != 0)
			{
				nFonts++;
			}
		}
		CFont[] array = new CFont[nFonts];
		int num = 0;
		for (int j = 0; j < nHandlesReel; j++)
		{
			if (useCount[j] != 0)
			{
				if (fonts != null && handleToIndex[j] != -1 && fonts[handleToIndex[j]] != null)
				{
					array[num] = fonts[handleToIndex[j]];
					array[num].useCount = useCount[j];
				}
				else
				{
					array[num] = new CFont();
					file.seek(offsetsToFonts[j]);
					array[num].load(file, app.content);
					array[num].useCount = useCount[j];
				}
				num++;
			}
		}
		fonts = array;
		handleToIndex = new short[nHandlesReel];
		for (int i = 0; i < nHandlesReel; i++)
		{
			handleToIndex[i] = -1;
		}
		for (int i = 0; i < nFonts; i++)
		{
			handleToIndex[fonts[i].handle] = (short)i;
		}
		nHandlesTotal = nHandlesReel;
		resetToLoad();
	}

	public CFont getFontFromHandle(short handle)
	{
		if (handle == -1)
		{
			return fonts[0];
		}
		if (handle >= 0 && handle < nHandlesTotal && handleToIndex[handle] != -1)
		{
			return fonts[handleToIndex[handle]];
		}
		return null;
	}

	public CFont getFontFromIndex(short index)
	{
		if (index >= 0 && index < nFonts)
		{
			return fonts[index];
		}
		return null;
	}

	public CFontInfo getFontInfoFromHandle(short handle)
	{
		if (handle < 0)
		{
			return fonts[0].getFontInfo();
		}
		CFont fontFromHandle = getFontFromHandle(handle);
		return fontFromHandle.getFontInfo();
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
		if (handle == -1)
		{
			_ = fonts;
		}
		else
		{
			useCount[handle]++;
		}
	}

	public short enumerate(short num)
	{
		setToLoad(num);
		return -1;
	}

	public short addFont(CFontInfo info)
	{
		int i;
		for (i = 0; i < nFonts && (fonts[i] == null || fonts[i].lfHeight != info.lfHeight || fonts[i].lfWeight != info.lfWeight || fonts[i].lfItalic != info.lfItalic || string.Compare(fonts[i].lfFaceName, info.lfFaceName, StringComparison.OrdinalIgnoreCase) != 0); i++)
		{
		}
		if (i < nFonts)
		{
			return fonts[i].handle;
		}
		short num = -1;
		for (int j = nHandlesReel; j < nHandlesTotal; j++)
		{
			if (handleToIndex[j] == -1)
			{
				num = (short)j;
				break;
			}
		}
		if (num == -1)
		{
			short[] array = new short[nHandlesTotal + 10];
			int j;
			for (j = 0; j < nHandlesTotal; j++)
			{
				array[j] = handleToIndex[j];
			}
			for (; j < nHandlesTotal + 10; j++)
			{
				array[j] = -1;
			}
			num = (short)nHandlesTotal;
			nHandlesTotal += 10;
			handleToIndex = array;
		}
		int num2 = -1;
		for (int k = 0; k < nFonts; k++)
		{
			if (fonts[k] == null)
			{
				num2 = k;
				break;
			}
		}
		if (num2 == -1)
		{
			CFont[] array2 = new CFont[nFonts + 10];
			int k;
			for (k = 0; k < nFonts; k++)
			{
				array2[k] = fonts[k];
			}
			for (; k < nFonts + 10; k++)
			{
				array2[k] = null;
			}
			num2 = nFonts;
			nFonts += 10;
			fonts = array2;
		}
		handleToIndex[num] = (short)num2;
		fonts[num2] = new CFont();
		fonts[num2].handle = num;
		fonts[num2].lfHeight = info.lfHeight;
		fonts[num2].lfWeight = info.lfWeight;
		fonts[num2].lfItalic = info.lfItalic;
		fonts[num2].lfFaceName = info.lfFaceName;
		return num;
	}
}
