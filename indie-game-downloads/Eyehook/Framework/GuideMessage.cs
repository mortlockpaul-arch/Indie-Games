using System;
using Microsoft.Xna.Framework;

namespace Eyehook.Framework;

public class GuideMessage : Screen
{
	private string title;

	private string message;

	private string[] options;

	private GuideMessageDelegate[] delegates;

	private GuideMessageDelegate defaultDelegate;

	private bool visible;

	public GuideMessage(string title, string message, string[] options, GuideMessageDelegate[] delegates, GuideMessageDelegate defaultDelegate)
		: base(modal: true)
	{
		this.title = title;
		this.message = message;
		this.options = options;
		this.delegates = delegates;
		this.defaultDelegate = defaultDelegate;
		visible = false;
	}

	private void onGuideMessage(IAsyncResult result)
	{
		int? num = null;
		if (num.HasValue)
		{
			delegates[num.Value]();
		}
		else
		{
			defaultDelegate();
		}
	}

	public override void update(GameTime gameTime)
	{
		if (visible)
		{
			return;
		}
		try
		{
			visible = true;
			MC.ScreenManager.removeScreen(this);
		}
		catch (Exception)
		{
		}
	}
}
