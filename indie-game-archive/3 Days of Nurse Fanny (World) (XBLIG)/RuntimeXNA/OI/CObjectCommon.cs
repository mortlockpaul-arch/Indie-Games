using RuntimeXNA.Animations;
using RuntimeXNA.Banks;
using RuntimeXNA.Movements;
using RuntimeXNA.Services;
using RuntimeXNA.Values;

namespace RuntimeXNA.OI;

public class CObjectCommon : COC
{
	public const int OEFLAG_DISPLAYINFRONT = 1;

	public const int OEFLAG_BACKGROUND = 2;

	public const int OEFLAG_BACKSAVE = 4;

	public const int OEFLAG_RUNBEFOREFADEIN = 8;

	public const int OEFLAG_MOVEMENTS = 16;

	public const int OEFLAG_ANIMATIONS = 32;

	public const int OEFLAG_TABSTOP = 64;

	public const int OEFLAG_WINDOWPROC = 128;

	public const int OEFLAG_VALUES = 256;

	public const int OEFLAG_SPRITES = 512;

	public const int OEFLAG_INTERNALBACKSAVE = 1024;

	public const int OEFLAG_SCROLLINGINDEPENDANT = 2048;

	public const int OEFLAG_QUICKDISPLAY = 4096;

	public const int OEFLAG_NEVERKILL = 8192;

	public const int OEFLAG_NEVERSLEEP = 16384;

	public const int OEFLAG_MANUALSLEEP = 32768;

	public const int OEFLAG_TEXT = 65536;

	public const int OEFLAG_DONTCREATEATSTART = 131072;

	public const short OCFLAGS2_DONTSAVEBKD = 1;

	public const short OCFLAGS2_SOLIDBKD = 2;

	public const short OCFLAGS2_COLBOX = 4;

	public const short OCFLAGS2_VISIBLEATSTART = 8;

	public const short OCFLAGS2_OBSTACLESHIFT = 4;

	public const short OCFLAGS2_OBSTACLEMASK = 48;

	public const short OCFLAGS2_OBSTACLE_SOLID = 16;

	public const short OCFLAGS2_OBSTACLE_PLATFORM = 32;

	public const short OCFLAGS2_OBSTACLE_LADDER = 48;

	public const short OCFLAGS2_AUTOMATICROTATION = 64;

	public const short OEPREFS_BACKSAVE = 1;

	public const short OEPREFS_SCROLLINGINDEPENDANT = 2;

	public const short OEPREFS_QUICKDISPLAY = 4;

	public const short OEPREFS_SLEEP = 8;

	public const short OEPREFS_LOADONCALL = 16;

	public const short OEPREFS_GLOBAL = 32;

	public const short OEPREFS_BACKEFFECTS = 64;

	public const short OEPREFS_KILL = 128;

	public const short OEPREFS_INKEFFECTS = 256;

	public const short OEPREFS_TRANSITIONS = 512;

	public const short OEPREFS_FINECOLLISIONS = 1024;

	public int ocOEFlags;

	public short[] ocQualifiers;

	public short ocFlags2;

	public short ocOEPrefs;

	public int ocIdentifier;

	public int ocBackColor;

	public CRect ocFadeIn;

	public CRect ocFadeOut;

	public CMoveDefList ocMovements;

	public CDefValues ocValues;

	public CDefStrings ocStrings;

	public CAnimHeader ocAnimations;

	public CDefCounters ocCounters;

	public CDefObject ocObject;

	public byte[] ocExtension;

	public int ocVersion;

	public int ocID;

	public int ocPrivate;

	public int ocFadeInLength;

	public int ocFadeOutLength;

	public override void load(CFile file, short type)
	{
		int filePointer = file.getFilePointer();
		ocQualifiers = new short[8];
		file.skipBytes(4);
		int num = file.readAShort();
		int num2 = file.readAShort();
		file.skipBytes(2);
		int num3 = file.readAShort();
		int num4 = file.readAShort();
		file.skipBytes(2);
		ocOEFlags = file.readAInt();
		for (int i = 0; i < 8; i++)
		{
			ocQualifiers[i] = file.readAShort();
		}
		int num5 = file.readAShort();
		int num6 = file.readAShort();
		int num7 = file.readAShort();
		ocFlags2 = file.readAShort();
		ocOEPrefs = file.readAShort();
		ocIdentifier = file.readAInt();
		ocBackColor = file.readAColor();
		int num8 = file.readAInt();
		int num9 = file.readAInt();
		if (num != 0)
		{
			file.seek(filePointer + num);
			ocMovements = new CMoveDefList();
			ocMovements.load(file);
		}
		if (num6 != 0)
		{
			file.seek(filePointer + num6);
			ocValues = new CDefValues();
			ocValues.load(file);
		}
		if (num7 != 0)
		{
			file.seek(filePointer + num7);
			ocStrings = new CDefStrings();
			ocStrings.load(file);
		}
		if (num2 != 0)
		{
			file.seek(filePointer + num2);
			ocAnimations = new CAnimHeader();
			ocAnimations.load(file);
		}
		if (num3 != 0)
		{
			file.seek(filePointer + num3);
			ocObject = new CDefCounter();
			ocObject.load(file);
		}
		if (num5 != 0)
		{
			file.seek(filePointer + num5);
			int num10 = file.readAInt();
			file.skipBytes(4);
			ocVersion = file.readAInt();
			ocID = file.readAInt();
			ocPrivate = file.readAInt();
			num10 -= 20;
			if (num10 != 0)
			{
				ocExtension = new byte[num10];
				file.read(ocExtension);
			}
		}
		if (num8 != 0)
		{
			file.seek(filePointer + num8);
			file.skipBytes(8);
			ocFadeInLength = file.readAInt();
		}
		if (num9 != 0)
		{
			file.seek(filePointer + num9);
			file.skipBytes(8);
			ocFadeOutLength = file.readAInt();
		}
		if (num4 != 0)
		{
			file.seek(filePointer + num4);
			switch (type)
			{
			case 3:
			case 4:
				ocObject = new CDefTexts();
				ocObject.load(file);
				break;
			case 5:
			case 6:
			case 7:
				ocCounters = new CDefCounters();
				ocCounters.load(file);
				break;
			case 9:
				ocObject = new CDefCCA();
				ocObject.load(file);
				break;
			case 8:
				break;
			}
		}
	}

	public override void enumElements(IEnum enumImages, IEnum enumFonts)
	{
		if (ocAnimations != null)
		{
			ocAnimations.enumElements(enumImages);
		}
		if (ocObject != null)
		{
			ocObject.enumElements(enumImages, enumFonts);
		}
		if (ocCounters != null)
		{
			ocCounters.enumElements(enumImages, enumFonts);
		}
	}
}
