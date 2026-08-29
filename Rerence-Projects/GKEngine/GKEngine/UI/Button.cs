using System.Collections.Generic;
using GKEngine.Entities;
using GKEngine.Scenes;
using Microsoft.Xna.Framework;

namespace GKEngine.UI;

public class Button : Sequence
{
	public delegate void ClickEvent();

	private bool active;

	public bool IsOver;

	public Dictionary<string, bool> IsDown = new Dictionary<string, bool>();

	public ClickEvent OnPress;

	public ClickEvent OnRelease;

	public bool Active
	{
		get
		{
			return active;
		}
		set
		{
			active = value;
			if (active)
			{
				playCurrentFrame = 0;
				Action_Add();
			}
			else
			{
				playCurrentFrame = 3;
			}
		}
	}

	public Button(Scene oScene, string xAssetBase, int xStart, int xEnd, int xDigits)
		: base(oScene, xAssetBase, xStart, xEnd, xDigits)
	{
		IsDown.Add("Left", value: false);
		IsDown.Add("Middle", value: false);
		IsDown.Add("Right", value: false);
	}

	public override void Load()
	{
		base.Load();
		Active = true;
	}

	public void Action_Add()
	{
		GameEngine.instance.updateStack.Add(Action_Listener);
	}

	public bool Action_Listener(GameTime oGameTime)
	{
		return !active;
	}
}
