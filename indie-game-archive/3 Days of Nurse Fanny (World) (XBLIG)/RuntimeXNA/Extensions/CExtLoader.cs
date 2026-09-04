using RuntimeXNA.Application;
using RuntimeXNA.Services;

namespace RuntimeXNA.Extensions;

public class CExtLoader
{
	public const int KPX_BASE = 32;

	private CRunApp app;

	private CExtLoad[] extensions;

	private short[] numOfConditions;

	public CExtLoader(CRunApp a)
	{
		app = a;
	}

	public void loadList(CFile file)
	{
		int num = file.readAShort();
		int num2 = file.readAShort();
		extensions = new CExtLoad[num2];
		numOfConditions = new short[num2];
		for (int i = 0; i < num2; i++)
		{
			extensions[i] = null;
		}
		for (int i = 0; i < num; i++)
		{
			CExtLoad cExtLoad = new CExtLoad();
			cExtLoad.loadInfo(file);
			CRunExtension cRunExtension = cExtLoad.loadRunObject();
			if (cRunExtension != null)
			{
				extensions[cExtLoad.handle] = cExtLoad;
				numOfConditions[cExtLoad.handle] = (short)cRunExtension.getNumberOfConditions();
			}
		}
	}

	public CRunExtension loadRunObject(int type)
	{
		type -= 32;
		CRunExtension result = null;
		if (type < extensions.Length && extensions[type] != null)
		{
			result = extensions[type].loadRunObject();
		}
		return result;
	}

	public int getNumberOfConditions(int type)
	{
		type -= 32;
		if (type < extensions.Length)
		{
			return numOfConditions[type];
		}
		return 0;
	}
}
