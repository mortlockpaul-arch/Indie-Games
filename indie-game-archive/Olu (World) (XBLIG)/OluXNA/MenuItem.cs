using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OluXNA;

public class MenuItem
{
	public Menu parent;

	public string name;

	public bool selected;

	public Vector2 position;

	public Vector2 size;

	public Color normalCol;

	public Color selectCol;

	public MenuItem prev;

	public MenuItem next;

	public string command;

	public MenuItem(Menu _par, string _name, Vector2 _pos, Color _normal, Color _select, string _command)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		base._002Ector();
		parent = _par;
		name = _name;
		selected = false;
		position = _pos;
		normalCol = _normal;
		selectCol = _select;
		command = _command;
		size = BaseGame.Get().hud.HUDfont.MeasureString(name);
	}

	public virtual void Update(GameTime gametime)
	{
	}

	public virtual void Draw(GameTime gametime)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		BaseGame.Get().spriteBatch.DrawString(BaseGame.Get().hud.HUDfont, name, position, selected ? selectCol : normalCol, 0f, Vector2.Zero, 1f, (SpriteEffects)0, 0f);
	}
}
