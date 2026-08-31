using Microsoft.Xna.Framework;

namespace BureauNewPDA;

public class RectangleActionData
{
	public enum RectCollisionType
	{
		Action,
		Up,
		Left,
		Right,
		Down,
		Spin,
		Info
	}

	public Rectangle rect = new Rectangle(0, 0, 0, 0);

	public RectCollisionType collisionType;

	public string id = "";

	public int nextRefId = -1;

	public string chapterRef = "NA";

	public string SFXString = "";

	public string displayText = "";
}
