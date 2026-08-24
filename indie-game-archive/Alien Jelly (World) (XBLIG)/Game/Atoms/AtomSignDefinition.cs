namespace Game.Atoms;

public class AtomSignDefinition : AtomDefinition
{
	public AtomSignDefinition(string xTitle, string xDesc, string xName, string xSurface)
		: base(xTitle, xDesc, xName, xSurface, "1x1x1 Sign 0", xInstanced: false, GameMain.RENDERSTACK_ADD, Type.Sign, 2u, xPlayGrid: false, new string[1] { "Props" }, xAutoRotate: false)
	{
		camCull = false;
		propertiesDesc = AtomSign.PROPERTIES_DESCRIPTION;
		properties = AtomSign.PROPERTIES;
		propertiesDefault = AtomSign.PROPERTIES_DEFAULT;
	}
}
