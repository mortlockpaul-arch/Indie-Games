using RuntimeXNA.Sprites;

namespace RuntimeXNA.Objects;

public class CRCom
{
	public int rcPlayer;

	public int rcMovementType;

	public CSprite rcSprite;

	public int rcAnim;

	public short rcImage = -1;

	public float rcScaleX = 1f;

	public float rcScaleY = 1f;

	public int rcAngle;

	public int rcDir;

	public int rcSpeed;

	public int rcMinSpeed;

	public int rcMaxSpeed;

	public bool rcChanged;

	public bool rcCheckCollides;

	public int rcOldX;

	public int rcOldY;

	public short rcOldImage = -1;

	public int rcOldAngle;

	public int rcOldDir;

	public int rcOldX1;

	public int rcOldY1;

	public int rcOldX2;

	public int rcOldY2;

	public void init()
	{
		rcScaleX = 1f;
		rcScaleY = 1f;
		rcAngle = 0;
		rcMovementType = -1;
	}

	public void kill(bool bFast)
	{
	}
}
