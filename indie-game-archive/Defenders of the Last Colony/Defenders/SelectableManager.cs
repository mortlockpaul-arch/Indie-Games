using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Defenders;

internal class SelectableManager
{
	public List<Selectable> selectables;

	public SelectableManager(ushort capacity)
	{
		selectables = new List<Selectable>(capacity);
	}

	public void Add(Selectable selectable)
	{
		selectables.Add(selectable);
	}

	public int Selected(Vector2 mousePos, float mouseTransp)
	{
		int result = -1;
		if (mouseTransp > 0f)
		{
			for (int i = 0; i < selectables.Count; i++)
			{
				if (selectables[i].IsMouseOn(new Rectangle((int)(mousePos.X - 1f), (int)(mousePos.Y - 1f), 2, 2)))
				{
					result = i;
				}
			}
		}
		return result;
	}

	public void Draw(SpriteBatch sb)
	{
		Draw(sb, unlockable: false, -1);
	}

	public void Draw(SpriteBatch sb, bool unlockable, int sel)
	{
		for (int i = 0; i < selectables.Count; i++)
		{
			if (i == sel)
			{
				selectables[i].Draw(sb, unlockable, Color.White);
			}
			else
			{
				selectables[i].Draw(sb, unlockable, Color.Gray);
			}
		}
	}
}
