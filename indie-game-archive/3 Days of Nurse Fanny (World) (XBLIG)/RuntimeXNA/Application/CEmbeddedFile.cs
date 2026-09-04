using RuntimeXNA.Services;

namespace RuntimeXNA.Application;

public class CEmbeddedFile
{
	private CRunApp app;

	public string path;

	private int length;

	private int offset;

	private CFile data;

	public CEmbeddedFile(CRunApp a)
	{
		app = a;
	}

	private string cleanName(string name)
	{
		int num = name.LastIndexOf('\\');
		if (num < 0)
		{
			num = name.LastIndexOf('/');
		}
		if (num >= 0 && num + 1 < name.Length)
		{
			name = name.Substring(num + 1);
		}
		return name;
	}

	public void preLoad()
	{
		short size = app.file.readAShort();
		path = app.file.readAString(size);
		path = cleanName(path);
		length = app.file.readAInt();
		offset = app.file.getFilePointer();
		app.file.skipBytes(length);
	}

	public CFile open()
	{
		app.file.seek(offset);
		data = new CFile(app.file, length);
		return data;
	}
}
