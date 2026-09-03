using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OluXNA;

public class MenuItemOption : MenuItem
{
	public List<string> options;

	public int currentChoice;

	public event OptionChangedEvent OnOptionChange;

	public MenuItemOption(Menu _par, string _name, Vector2 _pos, Color _normal, Color _select, string _command, OptionChangedEvent changeFunc)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		base._002Ector(_par, _name, _pos, _normal, _select, _command);
		OnOptionChange = (OptionChangedEvent)Delegate.Combine(OnOptionChange, new OptionChangedEvent(changeFunc.Invoke));
	}

	public override void Update(GameTime gametime)
	{
		if (BaseGame.Get().input.DirectionLeft() && selected)
		{
			currentChoice += options.Count - 1;
			currentChoice %= options.Count;
			OnOptionChange(command, options[currentChoice]);
		}
		else if (BaseGame.Get().input.DirectionRight() && selected)
		{
			currentChoice++;
			currentChoice %= options.Count;
			OnOptionChange(command, options[currentChoice]);
		}
		base.Update(gametime);
	}

	public override void Draw(GameTime gametime)
	{
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		base.Draw(gametime);
		BaseGame.Get().spriteBatch.DrawString(BaseGame.Get().hud.HUDfont, selected ? ("  < " + options[currentChoice] + " >") : ("    " + options[currentChoice]), position + new Vector2(size.X, 0f), selected ? selectCol : normalCol, 0f, Vector2.Zero, 1f, (SpriteEffects)0, 0f);
	}

	public void SetChoice(string toChoose)
	{
		currentChoice = options.FindIndex((string s) => s == toChoose);
	}
}
