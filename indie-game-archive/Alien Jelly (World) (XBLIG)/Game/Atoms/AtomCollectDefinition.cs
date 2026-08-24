namespace Game.Atoms;

public class AtomCollectDefinition : AtomDefinition
{
	public int value;

	public AtomCollectDefinition(string xTitle, string xDesc, string xName, string xSurface, string xShape, bool xInstanced, string xRanderStack, uint xCost, int xValue)
		: base(xTitle, xDesc, xName, xSurface, xShape, xInstanced, xRanderStack, Type.Collect, xCost, xPlayGrid: false, new string[1] { "Collects" }, xAutoRotate: false)
	{
		value = xValue;
	}
}
