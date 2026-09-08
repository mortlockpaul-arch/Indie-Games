using Microsoft.Xna.Framework;

namespace Quasar.GUI.Controls;

public abstract class InteractiveControl : Control
{
	public InteractiveControl(Layout layout)
		: base(layout)
	{
	}

	public abstract bool CheckClick(ClickType type, Vector2 position);

	public abstract void Update();
}
