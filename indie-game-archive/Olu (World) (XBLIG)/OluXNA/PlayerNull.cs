using Microsoft.Xna.Framework;

namespace OluXNA;

internal class PlayerNull : Enemy
{
	public override Matrix Transformation()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		return Matrix.Identity;
	}

	public override void draw(GameTime gametime)
	{
	}

	public override void act(GameTime gametime)
	{
	}

	public override void start()
	{
	}
}
