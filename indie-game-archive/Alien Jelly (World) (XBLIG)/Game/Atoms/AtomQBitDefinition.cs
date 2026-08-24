using Game.QBits;

namespace Game.Atoms;

public class AtomQBitDefinition : AtomDefinition
{
	public QBit.QBitType qbitType = QBit.QBitType.Null;

	public AtomQBitDefinition(string xTitle, string xDesc, string xName, string xSurface, QBit.QBitType xQbitType)
		: base(xTitle, xDesc, xName, xSurface, "1x1x1 QBit", xInstanced: false, GameMain.RENDERSTACK_MANUAL, Type.QBit, 2u, xPlayGrid: false, new string[1] { "Jelly & Robots" }, xAutoRotate: false)
	{
		qbitType = xQbitType;
	}
}
