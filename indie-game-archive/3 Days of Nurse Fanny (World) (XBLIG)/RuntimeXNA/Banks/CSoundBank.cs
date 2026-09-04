using RuntimeXNA.Application;

namespace RuntimeXNA.Banks;

public class CSoundBank : IEnum
{
	private CRunApp app;

	private CSound[] sounds;

	private int nHandlesReel;

	private int nHandlesTotal;

	private int nSounds;

	private int[] offsetsToSounds;

	private int[] handleToIndex;

	private int[] useCount;

	public void preLoad(CRunApp a)
	{
		app = a;
		nHandlesReel = app.file.readAShort();
		offsetsToSounds = new int[nHandlesReel];
		int num = app.file.readAShort();
		CSound cSound = new CSound(a);
		for (int i = 0; i < num; i++)
		{
			int filePointer = app.file.getFilePointer();
			cSound.loadHandle();
			offsetsToSounds[cSound.handle] = filePointer;
		}
		useCount = new int[nHandlesReel];
		resetToLoad();
		handleToIndex = null;
		nHandlesTotal = nHandlesReel;
		nSounds = 0;
		sounds = null;
	}

	public CSound getSoundFromHandle(short handle)
	{
		if (handle >= 0 && handle < nHandlesTotal && handleToIndex[handle] != -1)
		{
			return sounds[handleToIndex[handle]];
		}
		return null;
	}

	public CSound getSoundFromIndex(int index)
	{
		if (index >= 0 && index < nSounds)
		{
			return sounds[index];
		}
		return null;
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
		useCount[handle]++;
	}

	public short enumerate(short num)
	{
		setToLoad(num);
		return -1;
	}

	public void load()
	{
		nSounds = 0;
		for (int i = 0; i < nHandlesReel; i++)
		{
			if (useCount[i] != 0)
			{
				nSounds++;
			}
		}
		CSound[] array = new CSound[nSounds];
		int num = 0;
		for (int j = 0; j < nHandlesReel; j++)
		{
			if (useCount[j] != 0)
			{
				if (sounds != null && handleToIndex[j] != -1 && sounds[handleToIndex[j]] != null)
				{
					array[num] = sounds[handleToIndex[j]];
					array[num].useCount = useCount[j];
				}
				else
				{
					array[num] = new CSound(app);
					app.file.seek(offsetsToSounds[j]);
					array[num].load();
					array[num].useCount = useCount[j];
				}
				num++;
			}
		}
		sounds = array;
		handleToIndex = new int[nHandlesReel];
		for (int i = 0; i < nHandlesReel; i++)
		{
			handleToIndex[i] = -1;
		}
		for (int i = 0; i < nSounds; i++)
		{
			handleToIndex[sounds[i].handle] = i;
		}
		nHandlesTotal = nHandlesReel;
		resetToLoad();
	}
}
