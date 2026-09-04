using RuntimeXNA.Banks;
using RuntimeXNA.Services;

namespace RuntimeXNA.Animations;

public class CAnimHeader
{
	private static short[] tableApprox = new short[64]
	{
		3, 1, 2, 0, 2, 0, 0, 0, 1, 0,
		0, 0, 0, 1, 2, 0, 0, 0, 0, 0,
		0, 1, 2, 0, 0, 1, 2, 0, 1, 2,
		0, 0, 0, 1, 2, 0, 1, 2, 0, 0,
		0, 1, 2, 0, 0, 1, 2, 0, 0, 0,
		0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
		0, 0, 0, 0
	};

	public short ahAnimMax;

	public CAnim[] ahAnims;

	public byte[] ahAnimExists;

	public void load(CFile file)
	{
		int filePointer = file.getFilePointer();
		file.skipBytes(2);
		ahAnimMax = file.readAShort();
		short[] array = new short[ahAnimMax];
		for (int i = 0; i < ahAnimMax; i++)
		{
			array[i] = file.readAShort();
		}
		ahAnims = new CAnim[ahAnimMax];
		ahAnimExists = new byte[ahAnimMax];
		for (int i = 0; i < ahAnimMax; i++)
		{
			ahAnims[i] = null;
			ahAnimExists[i] = 0;
			if (array[i] != 0)
			{
				ahAnims[i] = new CAnim();
				file.seek(filePointer + array[i]);
				ahAnims[i].load(file);
				ahAnimExists[i] = 1;
			}
		}
		for (int j = 0; j < ahAnimMax; j++)
		{
			if (ahAnimExists[j] == 0)
			{
				bool flag = false;
				if (j < 12)
				{
					for (int i = 0; i < 4; i++)
					{
						if (ahAnimExists[tableApprox[j * 4 + i]] != 0)
						{
							ahAnims[j] = ahAnims[tableApprox[j * 4 + i]];
							flag = true;
							break;
						}
					}
				}
				if (flag)
				{
					continue;
				}
				for (int i = 0; i < ahAnimMax; i++)
				{
					if (ahAnimExists[i] != 0)
					{
						ahAnims[j] = ahAnims[i];
						break;
					}
				}
			}
			else
			{
				ahAnims[j].approximate(j);
			}
		}
	}

	public void enumElements(IEnum enumImages)
	{
		for (int i = 0; i < ahAnimMax; i++)
		{
			if (ahAnimExists[i] != 0)
			{
				ahAnims[i].enumElements(enumImages);
			}
		}
	}
}
