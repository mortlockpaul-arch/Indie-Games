using Microsoft.Xna.Framework;

namespace Defenders;

public class Logo
{
	public Vector2 posIni;

	public Vector2 posFin;

	public Rectangle rec;

	public float sizeIni;

	public float sizeFin;

	public void Update()
	{
		posIni.X = MathHelper.Lerp(posIni.X, posFin.X, 0.1f);
		posIni.Y = MathHelper.Lerp(posIni.Y, posFin.Y, 0.1f);
		sizeIni = MathHelper.Lerp(sizeIni, sizeFin, 0.1f);
	}
}
