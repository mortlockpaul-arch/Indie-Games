using RuntimeXNA.Application;

namespace RuntimeXNA.Params;

public class PARAM_PROGRAM : CParam
{
	public const short PRGFLAGS_WAIT = 1;

	public const short PRGFLAGS_HIDE = 2;

	public short flags;

	public string filename;

	public string command;

	public override void load(CRunApp app)
	{
		flags = app.file.readAShort();
		int filePointer = app.file.getFilePointer();
		filename = app.file.readAString();
		app.file.seek(filePointer + 260);
		command = app.file.readAString();
	}
}
