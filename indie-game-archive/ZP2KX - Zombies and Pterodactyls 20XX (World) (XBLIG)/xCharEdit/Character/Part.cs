using Microsoft.Xna.Framework;

namespace xCharEdit.Character;

public class Part
{
	public Vector2 location;

	public float rotation;

	public Vector2 scaling;

	public int idx;

	public int flip;

	public Part()
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		base._002Ector();
		idx = -1;
		scaling = new Vector2(1f, 1f);
	}
}
