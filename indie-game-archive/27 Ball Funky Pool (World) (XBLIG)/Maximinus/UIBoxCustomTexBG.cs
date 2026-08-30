using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Maximinus;

public class UIBoxCustomTexBG : UIBox
{
	private Texture2D BGTex;

	public SpriteEffects effects;

	public float Rotation;

	private Vector2 offset;

	public UIBoxCustomTexBG(Point startPos, float depth, Texture2D BGTex)
		: base(new Rectangle(startPos.X, startPos.Y, BGTex.Width + UIBox.border + 2, BGTex.Height + UIBox.border + 2), depth)
	{
		this.BGTex = BGTex;
		effects = SpriteEffects.None;
		Rotation = 0f;
		offset = new Vector2(BGTex.Width / 2, BGTex.Height / 2);
	}

	public void Draw(SpriteEffects effects)
	{
		this.effects = effects;
		Draw();
	}

	public override void Draw()
	{
		base.Draw();
		Rectangle value = new Rectangle(0, 0, insideRect.Width, insideRect.Height);
		Rectangle destinationRectangle = new Rectangle(insideRect.Center.X, insideRect.Center.Y, value.Width, insideRect.Height);
		UIBox.sb.Draw(BGTex, destinationRectangle, value, Color.White * transitionRatio, Rotation, offset, effects, depth + 0.01f);
	}
}
