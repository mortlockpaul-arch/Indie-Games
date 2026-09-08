using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Quasar.Input;

public class InputList<T> : IInputList where T : IInputState, new()
{
	private int id;

	private List<BaseInput<T>> actions = new List<BaseInput<T>>(4);

	private T result;

	private PlayerIndex playerIndex;

	private bool isGlobal;

	private bool hasGlyph;

	private char glyph;

	public int Id => id;

	public T Result => result;

	public bool IsGlobal => isGlobal;

	public bool HasGlyph => hasGlyph;

	public char Glyph => glyph;

	IInputState IInputList.Result => result;

	public InputList(int id)
	{
		this.id = id;
		isGlobal = true;
		result = new T();
	}

	public InputList(int id, PlayerIndex playerIndex)
	{
		this.id = id;
		isGlobal = false;
		this.playerIndex = playerIndex;
		result = new T();
	}

	public void SetGlyph(char glyph)
	{
		hasGlyph = true;
		this.glyph = glyph;
	}

	public void Add(BaseInput<T> action)
	{
		actions.Add(action);
	}

	public void Update()
	{
		result.Reset();
		if (!InputManager.Enabled)
		{
			return;
		}
		foreach (BaseInput<T> action in actions)
		{
			if (isGlobal)
			{
				action.Update(InputManager.PlayerIndices, ref result);
			}
			else
			{
				action.Update(playerIndex, ref result);
			}
		}
	}
}
