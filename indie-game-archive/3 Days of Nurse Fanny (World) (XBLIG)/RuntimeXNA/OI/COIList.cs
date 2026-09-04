using RuntimeXNA.Application;
using RuntimeXNA.Banks;
using RuntimeXNA.Services;

namespace RuntimeXNA.OI;

public class COIList
{
	public short oiMaxIndex;

	public COI[] ois;

	public short oiMaxHandle;

	public short[] oiHandleToIndex;

	public byte[] oiToLoad;

	public byte[] oiLoaded;

	private int currentOI;

	public void preLoad(CFile file)
	{
		oiMaxIndex = (short)file.readAInt();
		ois = new COI[oiMaxIndex];
		oiMaxHandle = 0;
		for (int i = 0; i < oiMaxIndex; i++)
		{
			CChunk cChunk = new CChunk();
			while (cChunk.chID != 32639)
			{
				cChunk.readHeader(file);
				if (cChunk.chSize == 0)
				{
					continue;
				}
				int pos = file.getFilePointer() + cChunk.chSize;
				switch (cChunk.chID)
				{
				case 17476:
					ois[i] = new COI();
					ois[i].loadHeader(file);
					if (ois[i].oiHandle >= oiMaxHandle)
					{
						oiMaxHandle = (short)(ois[i].oiHandle + 1);
					}
					break;
				case 17477:
					ois[i].oiName = file.readAString();
					break;
				case 17478:
					ois[i].oiFileOffset = file.getFilePointer();
					break;
				}
				file.seek(pos);
			}
		}
		oiHandleToIndex = new short[oiMaxHandle];
		for (int i = 0; i < oiMaxIndex; i++)
		{
			oiHandleToIndex[ois[i].oiHandle] = (short)i;
		}
		oiToLoad = new byte[oiMaxHandle];
		oiLoaded = new byte[oiMaxHandle];
		for (int j = 0; j < oiMaxHandle; j++)
		{
			oiToLoad[j] = 0;
			oiLoaded[j] = 0;
		}
	}

	public COI getOIFromHandle(short handle)
	{
		return ois[oiHandleToIndex[handle]];
	}

	public COI getOIFromIndex(short index)
	{
		return ois[index];
	}

	public void resetOICurrent()
	{
		for (int i = 0; i < oiMaxIndex; i++)
		{
			ois[i].oiFlags &= -17;
		}
	}

	public void setOICurrent(int handle)
	{
		ois[oiHandleToIndex[handle]].oiFlags |= 16;
	}

	public COI getFirstOI()
	{
		for (int i = 0; i < oiMaxIndex; i++)
		{
			if ((ois[i].oiFlags & 0x10) != 0)
			{
				currentOI = i;
				return ois[i];
			}
		}
		return null;
	}

	public COI getNextOI()
	{
		if (currentOI < oiMaxIndex)
		{
			for (int i = currentOI + 1; i < oiMaxIndex; i++)
			{
				if ((ois[i].oiFlags & 0x10) != 0)
				{
					currentOI = i;
					return ois[i];
				}
			}
		}
		return null;
	}

	public void resetToLoad()
	{
		for (int i = 0; i < oiMaxHandle; i++)
		{
			oiToLoad[i] = 0;
		}
	}

	public void setToLoad(int n)
	{
		oiToLoad[n] = 1;
	}

	public void load(CFile file, CRunApp app)
	{
		for (int i = 0; i < oiMaxHandle; i++)
		{
			if (oiToLoad[i] != 0)
			{
				if (oiLoaded[i] == 0 || (oiLoaded[i] != 0 && (ois[oiHandleToIndex[i]].oiLoadFlags & 0x20) != 0))
				{
					ois[oiHandleToIndex[i]].load(file, app);
					oiLoaded[i] = 1;
				}
			}
			else if (oiLoaded[i] != 0)
			{
				ois[oiHandleToIndex[i]].unLoad();
				oiLoaded[i] = 0;
			}
		}
		resetToLoad();
	}

	public void enumElements(IEnum enumImages, IEnum enumFonts)
	{
		for (int i = 0; i < oiMaxHandle; i++)
		{
			if (oiLoaded[i] != 0)
			{
				ois[oiHandleToIndex[i]].enumElements(enumImages, enumFonts);
			}
		}
	}
}
