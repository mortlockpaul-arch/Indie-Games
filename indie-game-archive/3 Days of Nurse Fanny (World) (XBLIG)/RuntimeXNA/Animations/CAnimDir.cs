using RuntimeXNA.Banks;
using RuntimeXNA.Services;

namespace RuntimeXNA.Animations;

public class CAnimDir
{
	public byte adMinSpeed;

	public byte adMaxSpeed;

	public short adRepeat;

	public short adRepeatFrame;

	public short adNumberOfFrame;

	public short[] adFrames;

	public void load(CFile file)
	{
		adMinSpeed = file.readAByte();
		adMaxSpeed = file.readAByte();
		adRepeat = file.readAShort();
		adRepeatFrame = file.readAShort();
		adNumberOfFrame = file.readAShort();
		adFrames = new short[adNumberOfFrame];
		for (int i = 0; i < adNumberOfFrame; i++)
		{
			adFrames[i] = file.readAShort();
		}
	}

	public void enumElements(IEnum enumImages)
	{
		for (int i = 0; i < adNumberOfFrame; i++)
		{
			if (enumImages != null)
			{
				short num = enumImages.enumerate(adFrames[i]);
				if (num != -1)
				{
					adFrames[i] = num;
				}
			}
		}
	}
}
