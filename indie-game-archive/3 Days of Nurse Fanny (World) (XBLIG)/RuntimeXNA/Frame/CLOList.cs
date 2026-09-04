using RuntimeXNA.Application;
using RuntimeXNA.OI;

namespace RuntimeXNA.Frame;

public class CLOList
{
	public CLO[] list;

	public short[] handleToIndex;

	public int nIndex;

	private int loFranIndex;

	public void load(CRunApp app)
	{
		nIndex = app.file.readAInt();
		list = new CLO[nIndex];
		short num = 0;
		for (int i = 0; i < nIndex; i++)
		{
			list[i] = new CLO();
			list[i].load(app.file);
			if (list[i].loHandle + 1 > num)
			{
				num = (short)(list[i].loHandle + 1);
			}
			COI oIFromHandle = app.OIList.getOIFromHandle(list[i].loOiHandle);
			list[i].loType = oIFromHandle.oiType;
		}
		handleToIndex = new short[num];
		for (int i = 0; i < nIndex; i++)
		{
			handleToIndex[list[i].loHandle] = (short)i;
		}
	}

	public CLO getLOFromIndex(short index)
	{
		return list[index];
	}

	public CLO getLOFromHandle(short handle)
	{
		if (handle < handleToIndex.Length)
		{
			return list[handleToIndex[handle]];
		}
		return null;
	}

	public CLO next_LevObj()
	{
		if (loFranIndex < nIndex)
		{
			do
			{
				CLO cLO = list[loFranIndex++];
				if (cLO.loType >= 2)
				{
					return cLO;
				}
			}
			while (loFranIndex < nIndex);
		}
		return null;
	}

	public CLO first_LevObj()
	{
		loFranIndex = 0;
		return next_LevObj();
	}
}
