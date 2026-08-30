using Microsoft.Xna.Framework;

namespace Maximinus;

public class UIBoxRatio_DoubleSided
{
	private UIBoxRatio boxLeft;

	private UIBoxRatio boxRight;

	private UIBoxRatio.Direction direction;

	public UIBoxRatio_DoubleSided(string name, Point Center, Point Size, float depth, UIBoxRatio.Direction direction, Color bgCol)
	{
		this.direction = direction;
		switch (direction)
		{
		case UIBoxRatio.Direction.Left:
		case UIBoxRatio.Direction.Right:
			boxLeft = new UIBoxRatio(name, new Rectangle(Center.X - Size.X / 2, Center.Y - Size.Y / 2, Size.X / 2, Size.Y), depth, 0f, UIBoxRatio.Direction.Left, bgCol);
			boxRight = new UIBoxRatio("", new Rectangle(Center.X, Center.Y - Size.Y / 2, Size.X / 2, Size.Y), depth, 0f, UIBoxRatio.Direction.Right, bgCol);
			break;
		}
	}

	public void DrawThisRatio(float ratio)
	{
		boxLeft.DrawThisRatio((ratio < 0f) ? (ratio * -1f) : 0f);
		boxRight.DrawThisRatio((ratio > 0f) ? ratio : 0f);
	}
}
