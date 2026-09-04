using RuntimeXNA.Services;
using RuntimeXNA.Sprites;

namespace RuntimeXNA.Frame;

public class CLO
{
	public const short PARENT_NONE = 0;

	public const short PARENT_FRAME = 1;

	public const short PARENT_FRAMEITEM = 2;

	public const short PARENT_QUALIFIER = 3;

	public short loHandle;

	public short loOiHandle;

	public int loX;

	public int loY;

	public short loParentType;

	public short loOiParentHandle;

	public short loLayer;

	public short loType;

	public CSprite[] loSpr;

	public CLO()
	{
		loSpr = new CSprite[4];
		for (int i = 0; i < 4; i++)
		{
			loSpr[i] = null;
		}
	}

	public void load(CFile file)
	{
		loHandle = file.readAShort();
		loOiHandle = file.readAShort();
		loX = file.readAInt();
		loY = file.readAInt();
		loParentType = file.readAShort();
		loOiParentHandle = file.readAShort();
		loLayer = file.readAShort();
		file.skipBytes(2);
	}
}
