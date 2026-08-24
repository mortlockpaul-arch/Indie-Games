using Game.QBits;

namespace Game.Atoms;

public class AtomFilterDefinition : AtomDefinition
{
	public QBit.QBitType qbit;

	public AtomFilterDefinition(string xTitle, string xName, string xSurface, string xShape, QBit.QBitType xQBit)
		: base(xTitle, AtomFilter.DESCRIPTION, xName, xSurface, xShape, xInstanced: false, GameMain.RENDERSTACK_SOLID, Type.Filter, 2u, xPlayGrid: true, new string[1] { "Crates & Special" }, xAutoRotate: false)
	{
		qbit = xQBit;
		propertiesDesc = AtomFilter.PROPERTIES_DESCRIPTION;
		properties = AtomFilter.PROPERTIES;
		propertiesDefault = AtomFilter.PROPERTIES_DEFAULT;
	}
}
