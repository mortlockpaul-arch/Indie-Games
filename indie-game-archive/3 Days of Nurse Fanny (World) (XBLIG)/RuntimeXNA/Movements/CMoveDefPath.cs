using RuntimeXNA.Services;

namespace RuntimeXNA.Movements;

internal class CMoveDefPath : CMoveDef
{
	public short mtNumber;

	public short mtMinSpeed;

	public short mtMaxSpeed;

	public byte mtLoop;

	public byte mtRepos;

	public byte mtReverse;

	public CPathStep[] steps;

	public override void load(CFile file, int length)
	{
		mtNumber = file.readAShort();
		mtMinSpeed = file.readAShort();
		mtMaxSpeed = file.readAShort();
		mtLoop = file.readByte();
		mtRepos = file.readByte();
		mtReverse = file.readByte();
		file.skipBytes(1);
		steps = new CPathStep[mtNumber];
		for (int i = 0; i < mtNumber; i++)
		{
			int filePointer = file.getFilePointer();
			steps[i] = new CPathStep();
			file.readUnsignedByte();
			int num = file.readUnsignedByte();
			steps[i].load(file);
			file.seek(filePointer + num);
		}
	}
}
