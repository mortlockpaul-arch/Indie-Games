using Microsoft.Xna.Framework;

namespace yMapEdit.segdef;

public class SegDef
{
	public int texIdx;

	public Rectangle sRect;

	public string name;

	public Vector2 lockLoc;

	public Vector2 origLoc;

	public int flags;

	public int material;

	public void UpdateOrigLoc()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		origLoc = lockLoc + new Vector2((float)sRect.X, (float)sRect.Y);
	}
}
