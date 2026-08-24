using System.Collections.Generic;
using Game.Scenes.Play;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game.Interactable;

public class InteractableManager
{
	public PlayUniverse universe;

	public List<Interactable> stack;

	public InteractableManager(PlayUniverse oUniverse)
	{
		universe = oUniverse;
		Init();
	}

	public void Init()
	{
		stack = new List<Interactable>();
	}

	public void Update(GameTime elapsed)
	{
		for (int i = 0; i < stack.Count; i++)
		{
			stack[i].Update(elapsed);
		}
	}

	public void RenderEffect(ref Effect effect)
	{
		for (int i = 0; i < stack.Count; i++)
		{
			stack[i].RenderEffect(ref effect);
		}
	}

	public void Add(Interactable oItem)
	{
		stack.Add(oItem);
	}

	public void Remove(Interactable oItem)
	{
		stack.Remove(oItem);
	}

	public void Flush()
	{
		while (stack.Count > 0)
		{
			Remove(stack[0]);
		}
	}

	public void Dispose()
	{
		for (int i = 0; i < stack.Count; i++)
		{
			stack[i].Dispose();
			stack[i] = null;
		}
		stack.Clear();
	}
}
