using System;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Quasar.Global;
using Quasar.Input;
using Quasar.Language;

namespace Quasar.GUI.Controls.GroupControls;

public class Button : GroupControl
{
	public const string Type = "Button";

	private string text;

	public override string ControlType => "Button";

	public string Text
	{
		get
		{
			return text;
		}
		set
		{
			text = value;
		}
	}

	public event Action<Button, PlayerIndex> OnInteraction;

	public Button(Layout layout, Group group)
		: base(layout, group)
	{
		size = TemplateManager.Templates[base.Layout.Template].GetButtonSize(this);
	}

	public override void SetId(string id)
	{
		base.SetId(id);
		size = TemplateManager.Templates[base.Layout.Template].GetButtonSize(this);
	}

	public override void Update()
	{
		if (OnInteraction != null)
		{
			PlayerIndex whoPressed = PlayerIndex.One;
			if (InputManager.MenuInteract(ref whoPressed))
			{
				OnInteraction(this, whoPressed);
			}
			else if (InputManager.MenuStart(ref whoPressed))
			{
				OnInteraction(this, whoPressed);
			}
		}
	}

	public override bool CheckClick(ClickType type, Vector2 position)
	{
		bool flag = GameMath.InsideBox(base.Position, base.Size, position);
		if (flag && type == ClickType.Press)
		{
			Click();
		}
		return flag;
	}

	protected virtual void Click()
	{
		if (OnInteraction != null)
		{
			OnInteraction(this, PlayerIndex.One);
		}
	}

	public override void parseXml(XElement xe)
	{
		base.parseXml(xe);
		text = LanguageManager.Texts[XDocHelper.GetAttribute(xe, "text")];
		size = TemplateManager.Templates[base.Layout.Template].GetButtonSize(this);
	}

	public override void Dispose()
	{
		OnInteraction = null;
		base.Dispose();
	}
}
