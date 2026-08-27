namespace MaxScriptDefines;

public struct PredefinedObjectStruct(string n, int i, bool materialE, bool opacityE, bool cullingE, bool collisionE)
{
	public string name = n;

	public int typeIndex = i;

	public bool materialEnabled = materialE;

	public bool opacityEnabled = opacityE;

	public bool cullingEnabled = cullingE;

	public bool collisionEnabled = collisionE;

	public int materialType = 1;

	public int opacityType = 1;

	public int cullingType = 1;

	public int collisionType = 1;

	public void SetParameters(int material, int opacity, int culling, int collision)
	{
		materialType = material;
		opacityType = opacity;
		cullingType = culling;
		collisionType = collision;
	}
}
