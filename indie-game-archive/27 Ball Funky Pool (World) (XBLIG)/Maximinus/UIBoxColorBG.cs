using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Maximinus;

public class UIBoxColorBG : UIBox
{
	private Color bgCol;

	public UIBoxColorBG(Rectangle rect, float depth, Color bgCol)
		: base(rect, depth)
	{
		this.bgCol = bgCol;
	}

	public override void Draw()
	{
		base.Draw();
		UIBox.sb.Draw(UIBox.blankTex, insideRect, null, Utils.ColorWithAlpha(bgCol, transitionRatio), 0f, Vector2.Zero, SpriteEffects.None, depth + 0.01f);
	}
}
