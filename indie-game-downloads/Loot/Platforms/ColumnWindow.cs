using Microsoft.Xna.Framework;

namespace Loot.Platforms;

public class ColumnWindow
{
	private int mark;

	private Column[] cols;

	public readonly int Length;

	public int Mark => mark;

	public ColumnWindow(int length)
	{
		cols = new Column[length];
		Length = cols.Length;
		mark = 0;
	}

	public void SetMark(int mark)
	{
		this.mark = mark;
	}

	public void IncrementMark()
	{
		mark++;
		if (mark == Length)
		{
			mark = 0;
		}
	}

	private int idx(int windowIndex)
	{
		while (windowIndex < 0)
		{
			windowIndex += Length;
		}
		return (mark + windowIndex) % Length;
	}

	public Column Get(int windowIndex)
	{
		return cols[idx(windowIndex)];
	}

	public void Set(int windowIndex, Column col)
	{
		cols[idx(windowIndex)] = col;
	}

	public int Collect(int windowIndex)
	{
		int num = idx(windowIndex);
		cols[num].collected = true;
		return cols[num].gold;
	}

	public void Update(GameTime gameTime)
	{
		for (int i = 0; i < cols.Length; i++)
		{
			cols[i].Update(gameTime);
		}
	}
}
