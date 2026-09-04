using RuntimeXNA.Banks;
using RuntimeXNA.Services;

namespace RuntimeXNA.Animations;

public class CAnim
{
	public const short ANIMID_STOP = 0;

	public const short ANIMID_WALK = 1;

	public const short ANIMID_RUN = 2;

	public const short ANIMID_APPEAR = 3;

	public const short ANIMID_DISAPPEAR = 4;

	public const short ANIMID_BOUNCE = 5;

	public const short ANIMID_SHOOT = 6;

	public const short ANIMID_JUMP = 7;

	public const short ANIMID_FALL = 8;

	public const short ANIMID_CLIMB = 9;

	public const short ANIMID_CROUCH = 10;

	public const short ANIMID_UNCROUCH = 11;

	public const short ANIMID_USER1 = 12;

	private static byte[] tableAnimTwoSpeeds = new byte[16]
	{
		0, 1, 1, 0, 0, 1, 0, 1, 1, 1,
		1, 1, 1, 1, 1, 1
	};

	public CAnimDir[] anDirs;

	public byte[] anTrigo;

	public byte[] anAntiTrigo;

	public void load(CFile file)
	{
		int filePointer = file.getFilePointer();
		short[] array = new short[32];
		for (int i = 0; i < 32; i++)
		{
			array[i] = file.readAShort();
		}
		anDirs = new CAnimDir[32];
		anTrigo = new byte[32];
		anAntiTrigo = new byte[32];
		for (int i = 0; i < 32; i++)
		{
			anDirs[i] = null;
			anTrigo[i] = 0;
			anAntiTrigo[i] = 0;
			if (array[i] != 0)
			{
				anDirs[i] = new CAnimDir();
				file.seek(filePointer + array[i]);
				anDirs[i].load(file);
			}
		}
	}

	public void enumElements(IEnum enumImages)
	{
		for (int i = 0; i < 32; i++)
		{
			if (anDirs[i] != null)
			{
				anDirs[i].enumElements(enumImages);
			}
		}
	}

	public void approximate(int nAnim)
	{
		for (int i = 0; i < 32; i++)
		{
			if (anDirs[i] == null)
			{
				int num = 0;
				int num2 = i + 1;
				while (num < 32)
				{
					num2 &= 0x1F;
					if (anDirs[num2] != null)
					{
						anTrigo[i] = (byte)num2;
						break;
					}
					num++;
					num2++;
				}
				int num3 = 0;
				int num4 = i - 1;
				while (num3 < 32)
				{
					num4 &= 0x1F;
					if (anDirs[num4] != null)
					{
						anAntiTrigo[i] = (byte)num4;
						break;
					}
					num3++;
					num4--;
				}
				if (num2 == num4 || num < num3)
				{
					anTrigo[i] |= 64;
				}
				else if (num3 < num)
				{
					anAntiTrigo[i] |= 64;
				}
			}
			else if (nAnim < 16 && tableAnimTwoSpeeds[nAnim] == 0)
			{
				anDirs[i].adMinSpeed = anDirs[i].adMaxSpeed;
			}
		}
	}
}
