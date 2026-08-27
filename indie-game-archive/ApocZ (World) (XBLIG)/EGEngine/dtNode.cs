namespace EGEngine;

internal class dtNode
{
	public float cost;

	public float total;

	public uint id;

	public uint pidx;

	public uint flags;

	public int nodeIdx;

	public dtNode()
	{
	}

	public dtNode(int i, uint p, uint f)
	{
		pidx = p;
		flags = f;
		nodeIdx = i;
		cost = 0f;
		total = 0f;
		id = 0u;
	}
}
