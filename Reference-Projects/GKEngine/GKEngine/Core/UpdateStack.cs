using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace GKEngine.Core;

public class UpdateStack
{
	public delegate bool UpdateStackItem(GameTime oGameTime);

	public delegate void Callback();

	public bool active = true;

	public bool activeChanged = true;

	public List<UpdateStackItem> stack = new List<UpdateStackItem>();

	public List<UpdateStackItem> add = new List<UpdateStackItem>();

	public Callback changed;

	public Callback done;

	private int _i;

	private int _countA;

	public void Update(GameTime oGameTime)
	{
		if (active != activeChanged)
		{
			active = activeChanged;
			if (changed != null)
			{
				changed();
				changed = null;
			}
		}
		if (active)
		{
			_countA = add.Count;
			if (_countA > 0)
			{
				for (_i = 0; _i < _countA; _i++)
				{
					stack.Add(add[_i]);
				}
			}
			add.Clear();
			if (stack.Count > 0)
			{
				for (_i = 0; _i < stack.Count; _i++)
				{
					if (stack[_i](oGameTime))
					{
						stack.Remove(stack[_i]);
						_i--;
					}
				}
			}
		}
		if (done != null)
		{
			done();
			done = null;
		}
	}

	public void Stop(Callback oCallback)
	{
		changed = oCallback;
		activeChanged = false;
	}

	public void Start()
	{
		activeChanged = true;
	}

	public void Add(UpdateStackItem oItem)
	{
		add.Add(oItem);
	}

	public void Clear()
	{
		stack.Clear();
		add.Clear();
	}

	public UpdateStack Copy()
	{
		UpdateStack updateStack = new UpdateStack();
		foreach (UpdateStackItem item in stack)
		{
			updateStack.stack.Add(item);
		}
		return updateStack;
	}
}
