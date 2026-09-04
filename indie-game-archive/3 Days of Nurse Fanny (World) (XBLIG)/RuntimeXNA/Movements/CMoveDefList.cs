using RuntimeXNA.Services;

namespace RuntimeXNA.Movements;

public class CMoveDefList
{
	public int nMovements;

	public CMoveDef[] moveList;

	public void load(CFile file)
	{
		int filePointer = file.getFilePointer();
		nMovements = file.readAInt();
		moveList = new CMoveDef[nMovements];
		for (int i = 0; i < nMovements; i++)
		{
			file.seek(filePointer + 4 + 16 * i);
			int num = file.readAInt();
			int id = file.readAInt();
			int num2 = file.readAInt();
			int num3 = file.readAInt();
			file.seek(filePointer + num2);
			short c = file.readAShort();
			short num4 = file.readAShort();
			byte m = file.readByte();
			byte mo = file.readByte();
			file.skipBytes(2);
			int d = file.readAInt();
			switch (num4)
			{
			case 0:
				moveList[i] = new CMoveDefStatic();
				break;
			case 1:
				moveList[i] = new CMoveDefMouse();
				break;
			case 2:
				moveList[i] = new CMoveDefRace();
				break;
			case 3:
				moveList[i] = new CMoveDefGeneric();
				break;
			case 4:
				moveList[i] = new CMoveDefBall();
				break;
			case 5:
				moveList[i] = new CMoveDefPath();
				break;
			case 9:
				moveList[i] = new CMoveDefPlatform();
				break;
			case 14:
				moveList[i] = new CMoveDefExtension();
				break;
			}
			moveList[i].setData(num4, c, m, d, mo);
			moveList[i].load(file, num3 - 12);
			if (num4 == 14)
			{
				file.seek(filePointer + num);
				string text = file.readAString();
				text = text.Substring(0, text.Length - 4);
				((CMoveDefExtension)moveList[i]).setModuleName(text, id);
			}
		}
	}
}
