using Game.QBits;

namespace Game.Atoms;

public class AtomSwitchDefinition : AtomDefinition
{
	public QBit.QBitType qbitType = QBit.QBitType.Null;

	public AtomSwitch.Types switchType;

	public int value;

	public AtomSwitchDefinition(string xTitle, string xDesc, string xName, string xSurface, string xShape, QBit.QBitType xQbitType, AtomSwitch.Types xType, int xValue)
		: base(xTitle, xDesc, xName, xSurface, xShape, xInstanced: false, GameMain.RENDERSTACK_SOLID, Type.Switch, 3u, xPlayGrid: false, new string[1] { "Switches" }, xAutoRotate: false)
	{
		qbitType = xQbitType;
		switchType = xType;
		value = xValue;
		hueable = false;
	}
}
