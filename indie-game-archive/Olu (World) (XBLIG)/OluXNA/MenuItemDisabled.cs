using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OluXNA;

public class MenuItemDisabled : MenuItem
{
	public MenuItemDisabled(Menu _par, string _name, Vector2 _pos, Color _normal, Color _select, string _command)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		base._002Ector(_par, _name, _pos, _normal, _select, _command);
	}

	public override void Update(GameTime gametime)
	{
	}
}
