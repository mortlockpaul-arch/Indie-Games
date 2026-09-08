using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Quasar.Global;

namespace Quasar.GUI.Controls;

public class ClickButton : InteractiveControl
{
	public delegate void ClickButtonHandler(ClickButton group);

	public class ClickButtonCreator : IControlCreator
	{
		private static ClickButtonCreator instance;

		public static ClickButtonCreator Instance
		{
			get
			{
				if (instance == null)
				{
					instance = new ClickButtonCreator();
				}
				return instance;
			}
		}

		public Control ParseXml(XElement xe, Layout layout)
		{
			ClickButton clickButton = new ClickButton(layout);
			clickButton.parseXml(xe);
			return clickButton;
		}
	}

	public const string Type = "ClickButton";

	private string text = "";

	private Vector2 position;

	private Vector2 size;

	private bool isDown;

	private bool enabled = true;

	public override string ControlType => "ClickButton";

	public string Text
	{
		get
		{
			return text;
		}
		set
		{
			text = value;
			if (OnTextChange != null)
			{
				OnTextChange(this);
			}
		}
	}

	public Vector2 Position
	{
		get
		{
			return position;
		}
		set
		{
			position = value;
		}
	}

	public Vector2 Size
	{
		get
		{
			return size;
		}
		set
		{
			size = value;
		}
	}

	public bool IsDown => isDown;

	public bool Enabled
	{
		get
		{
			return enabled;
		}
		set
		{
			enabled = value;
		}
	}

	public event ClickButtonHandler OnTextChange;

	public event ClickButtonHandler OnClick;

	public event ClickButtonHandler OnRelease;

	public ClickButton(Layout layout)
		: base(layout)
	{
	}

	public override bool CheckClick(ClickType type, Vector2 position)
	{
		if (!enabled)
		{
			return false;
		}
		if (type == ClickType.Release)
		{
			Release();
			isDown = false;
			return true;
		}
		bool flag = GameMath.InsideBox(Position, Size, position);
		if (flag)
		{
			isDown = true;
			Click();
		}
		return flag;
	}

	private void Click()
	{
		if (OnClick != null)
		{
			OnClick(this);
		}
	}

	private void Release()
	{
		if (OnRelease != null)
		{
			OnRelease(this);
		}
	}

	public override void Update()
	{
	}

	public override void parseXml(XElement xe)
	{
		base.parseXml(xe);
	}

	public override void Dispose()
	{
		OnClick = null;
		base.Dispose();
	}
}
