using RuntimeXNA.Services;

namespace RuntimeXNA.Movements;

internal class CMoveDefBall : CMoveDef
{
	public short mbSpeed;

	public short mbBounce;

	public short mbAngles;

	public short mbSecurity;

	public short mbDecelerate;

	public override void load(CFile file, int length)
	{
		mbSpeed = file.readAShort();
		mbBounce = file.readAShort();
		mbAngles = file.readAShort();
		mbSecurity = file.readAShort();
		mbDecelerate = file.readAShort();
	}
}
