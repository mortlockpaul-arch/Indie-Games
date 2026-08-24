using Microsoft.Xna.Framework;

namespace ZP2K9;

public class Scroll
{
	public static Vector2 scroll;

	public static float zoom = 1f;

	public static Vector2 GetLoc(Vector2 loc)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		return (loc - scroll) * zoom + new Vector2(640f, 360f);
	}
}
