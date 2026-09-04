using RuntimeXNA.Application;

namespace RuntimeXNA.Params;

public class PARAM_SHOOT : CCreate
{
	public short shtSpeed;

	public override void load(CRunApp app)
	{
		posOINUMParent = app.file.readAShort();
		posFlags = app.file.readAShort();
		posX = app.file.readAShort();
		posY = app.file.readAShort();
		posSlope = app.file.readAShort();
		posAngle = app.file.readAShort();
		posDir = app.file.readAInt();
		posTypeParent = app.file.readAShort();
		posOiList = app.file.readAShort();
		posLayer = app.file.readAShort();
		cdpHFII = app.file.readAShort();
		cdpOi = app.file.readAShort();
		app.file.skipBytes(4);
		shtSpeed = app.file.readAShort();
	}
}
