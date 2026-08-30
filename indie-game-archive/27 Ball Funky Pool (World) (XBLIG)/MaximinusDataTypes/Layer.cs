using Microsoft.Xna.Framework.Graphics;

namespace MaximinusDataTypes;

public class Layer
{
	public Texture2D Tex;

	public readonly string Name;

	public bool Visible;

	public Layer(Texture2D t, string n, bool v)
	{
		Tex = t;
		Name = n;
		Visible = v;
	}
}
