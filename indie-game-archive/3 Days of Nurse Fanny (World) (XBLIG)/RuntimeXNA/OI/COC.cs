using RuntimeXNA.Banks;
using RuntimeXNA.Services;

namespace RuntimeXNA.OI;

public class COC
{
	public const short OBSTACLE_NONE = 0;

	public const short OBSTACLE_SOLID = 1;

	public const short OBSTACLE_PLATFORM = 2;

	public const short OBSTACLE_LADDER = 3;

	public const short OBSTACLE_TRANSPARENT = 4;

	public short ocObstacleType;

	public short ocColMode;

	public int ocCx;

	public int ocCy;

	public COI oi;

	public virtual void load(CFile file, short type)
	{
	}

	public virtual void enumElements(IEnum enumImages, IEnum enumFonts)
	{
	}
}
