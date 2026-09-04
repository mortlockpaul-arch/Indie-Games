using System;
using RuntimeXNA.Services;

namespace RuntimeXNA.Extensions;

internal class CExtLoad
{
	public string name;

	public string subType;

	public short handle;

	public void loadInfo(CFile file)
	{
		int filePointer = file.getFilePointer();
		short num = Math.Abs(file.readAShort());
		handle = file.readAShort();
		file.skipBytes(12);
		name = file.readAString();
		int length = name.LastIndexOf('.');
		name = name.Substring(0, length);
		subType = file.readAString();
		file.seek(filePointer + num);
	}

	public CRunExtension loadRunObject()
	{
		CRunExtension result = null;
		if (string.Compare(name, "XNA") == 0)
		{
			result = new CRunXNA();
		}
		if (string.Compare(name, "XBOXGamepad") == 0)
		{
			result = new CRunXBOXGamepad();
		}
		if (string.Compare(name, "XNAGamerServices") == 0)
		{
			result = new CRunXNAGamerServices();
		}
		if (string.Compare(name, "kcini") == 0)
		{
			result = new CRunkcini();
		}
		return result;
	}
}
