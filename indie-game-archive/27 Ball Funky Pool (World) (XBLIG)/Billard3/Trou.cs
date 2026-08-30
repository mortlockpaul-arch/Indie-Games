using Microsoft.Xna.Framework;

namespace Billard3;

public class Trou
{
	public int number;

	public Vector2 pos;

	public float rayon;

	public string name;

	public Trou(int i, Vector2 v, float f, string s)
	{
		number = i;
		pos = v;
		rayon = f;
		name = s;
	}
}
