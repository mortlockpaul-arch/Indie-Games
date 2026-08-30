using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Defenders;

internal class Blast : SimpleObject
{
	public int id;

	public float grow = 0f;

	public float maximunSize = 5f;

	public ushort type = 0;

	private float transp = 0f;

	private float angle2 = 0f;

	private float angle3 = 0f;

	public override void Update()
	{
		angle2 += 0.24f;
		angle3 -= 0.375f;
		grow = MathHelper.Lerp(grow, maximunSize * 1.5f, 0.18f);
		size = new Vector2(grow);
		origin = new Vector2(base.Width / 2, base.Height / 2);
		angle += 0.1f;
		if (grow < maximunSize / 2f)
		{
			transp = MathHelper.Lerp(transp, 1f, 0.75f);
		}
		if (grow > maximunSize * 0.9f)
		{
			transp = MathHelper.Lerp(transp, 0f, 0.3f);
		}
		color = new Color(transp * 0.8f, transp * 0.9f, transp * 1f, transp * 0.95f);
		if (grow > maximunSize * 0.9f && transp < 0.1f)
		{
			Active = false;
		}
	}

	public override void Draw(SpriteBatch spriteBatch)
	{
		spriteBatch.Draw(texture, position, null, color * 0.8f * (maximunSize * 0.3f), angle, origin, size, SpriteEffects.None, 1f);
		spriteBatch.Draw(texture, position, null, color * 0.8f * (maximunSize * 0.3f), angle * angle3, origin, size * 0.75f, SpriteEffects.FlipVertically, 1f);
		spriteBatch.Draw(texture, position, null, color * 0.6f * (maximunSize * 0.3f), angle * (0f - angle3), origin, size * 0.5f, SpriteEffects.FlipHorizontally, 1f);
		spriteBatch.Draw(texture, position, null, color * 0.4f * (maximunSize * 0.3f), angle + angle2, origin, size * 0.45f, SpriteEffects.FlipHorizontally, 1f);
		spriteBatch.Draw(texture, position, null, color * 0.2f * (maximunSize * 0.3f), angle - angle2, origin, size * 0.35f, SpriteEffects.FlipVertically, 1f);
		spriteBatch.Draw(texture, position, null, color * 0.1f * (maximunSize * 0.3f), angle / 2f, origin, size * 0.25f, SpriteEffects.None, 1f);
	}
}
