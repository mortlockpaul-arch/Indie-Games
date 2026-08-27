namespace EGEngine;

public struct TerainLevelPacket(string n, string hm, int x, int y, int s, float h, float d)
{
	public string kname = n;

	public string heightMap = hm;

	public int sizex = x;

	public int sizey = y;

	public int scale = s;

	public float maxHeight = h;

	public float seaDepth = d;
}
