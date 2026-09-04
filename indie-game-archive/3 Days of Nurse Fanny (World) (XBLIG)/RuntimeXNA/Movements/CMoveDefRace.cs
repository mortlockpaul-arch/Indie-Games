using RuntimeXNA.Services;

namespace RuntimeXNA.Movements;

internal class CMoveDefRace : CMoveDef
{
	public short mrSpeed;

	public short mrAcc;

	public short mrDec;

	public short mrRot;

	public short mrBounceMult;

	public short mrAngles;

	public short mrOkReverse;

	public override void load(CFile file, int length)
	{
		mrSpeed = file.readAShort();
		mrAcc = file.readAShort();
		mrDec = file.readAShort();
		mrRot = file.readAShort();
		mrBounceMult = file.readAShort();
		mrAngles = file.readAShort();
		mrOkReverse = file.readAShort();
	}
}
